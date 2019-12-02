using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine.Android;

[RequireComponent(typeof(AudioSource))]
public class SceneControllerScript : MonoBehaviourPunCallbacks, IMatchmakingCallbacks, IOnEventCallback, IConnectionCallbacks
{
    string _room = "AudioVisualScene";

    public const byte InstantiateVrAvatarEventCode = 1; 
    public const byte PlayMusicEventCode = 2;
    public const byte PauseMusicEventCode = 3;
    public const byte RewindMusicEventCode = 4;
    public const byte FastFowardEventCode = 5;
    public const byte PauseMenuChangeEventCode = 6;
    public const byte SendClientsToJukeBoxEventCode = 7;
    public const byte SetWhiteEventCode = 8;
    public const byte SetOrangeEventCode = 9;
    public const byte SetGreenEventCode = 10;
    public const byte SetBlueEventCode = 11;
    public const byte SetPinkEventCode = 12;
    public const byte SetMultiEventCode = 13;

    public List<GameObject> avatarPrefabs = new List<GameObject>();
    public OVRInput.Controller controller;
    public GameObject canvas;

    GameObject dialogue;
    public Recorder recorder;
    public TextMeshProUGUI debugObject;

    //audio visualization
    int numCubes = 256;
    public float[] spectrum = new float[512];
    GameObject[] cubes = new GameObject[512];
    public Color[] colors = new Color[512];
    public List<GameObject> cubeList = new List<GameObject>();
    public List<GameObject> otherObjects = new List<GameObject>();
    public string currentColor;

    public float cubeScaler;
    public int cubeThreshold;

    //sparks
    public int particleCount;
    public float particleMinSize;
    public float particleMaxSize;
    private bool alreadyExploded;

    public GameObject cubePrefab, ground;
    public GameObject snow;

    //menu
    public GameObject particlePrefab, menuPanel, circleCenter, playButton, pauseButton, albumArt, songName;

    public GameObject asteroid;
    public GameObject pauseMenu, resumeButton, backJukeBoxButton;

    //lyrics
    List<LyricLine> songLyrics;
    public TextMeshProUGUI lyrics;
    Vector3 offset;
    public float lyricsDepth, menuDepth;
    public float lyricsHeight, menuHeight;
    private bool pauseMenuUp;

    AudioSource audioSource;

    public Material skyboxWhite;
    public Material skyboxOrange;
    public Material skyboxGreen;
    public Material skyboxBlue;
    public Material skyboxPink;
    public Material skyboxMulti;

    public GameObject tree;

    public GameObject multiObjects;
    public GameObject pinkObjects;
    public GameObject greenObjects;
    public GameObject blueObjects;
    public GameObject orangeObjects;
    public GameObject whiteObjects;
    int counter = 0;

    GameObject localAvatar;

    bool joinedRoom;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        Debug.Log("NetworkManager2 enabled.");
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        // Initially, pause menu is not displayed
        pauseMenuUp = false;

        if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions() { };
            PhotonNetwork.JoinOrCreateRoom(_room, roomOptions, TypedLobby.Default);
        }
        else
        {
            Debug.Log("Trying to connect using settings");
            if (PhotonNetwork.ConnectUsingSettings())
            {
                Debug.Log("Success connecting using Photon settings!");
            }
            else
            {
                Debug.LogError("Failed to connect using Photon settings");
            }
        }

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            dialogue = new GameObject();
        }
#endif

        // Add microphone input
        recorder.StartRecording();

        currentColor = "multi";
        offset = canvas.transform.position - Camera.main.transform.position;
        audioSource = GetComponent<AudioSource>();
        LoadSong(Properties.selectedSong);

        Vector3 center = transform.position;

        for (int i = 0; i < 128; i++)
        {
            float ang = 2.8125f * i;
            Vector3 pos = circleUp(center, 6.5f,ang);
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            GameObject cubeInstance = Instantiate(cubePrefab, pos, rot);

            var cubeRenderer = cubeInstance.GetComponent<Renderer>();

            colors[i] = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            cubeRenderer.material.SetColor("_Color", colors[i]);

            cubes[i] = cubeInstance;
            cubeList.Add(cubeInstance);
        }
    }

    public override void OnConnectedToMaster() //Callback function for when the first connection is established successfully.
    {
        Debug.Log("Connected to master");

        RoomOptions roomOptions = new RoomOptions() { };
        PhotonNetwork.JoinOrCreateRoom(_room, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        joinedRoom = true;

        // This is just a sphere that represents the player so we can easily see other players
        Debug.Log("Trying to instantiate player prefab");
        //PhotonNetwork.Instantiate("NetworkedPlayerLocal", Vector3.zero, Quaternion.identity, 0);
        GameObject localAvatar = Instantiate(Resources.Load("AJ Local")) as GameObject;


        PhotonView photonView = localAvatar.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(photonView))
        {
            Debug.Log("View ID allocated");
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            PhotonNetwork.RaiseEvent(InstantiateVrAvatarEventCode, photonView.ViewID, raiseEventOptions, SendOptions.SendReliable);
        }
        else
        {
            Debug.LogError("Failed to allocate a ViewId.");

            Destroy(localAvatar);
        }
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room");

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed.");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join room failed.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join random room failed.");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("SongSelector");
    }

    void OnGUI()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            dialogue.AddComponent<PermissionsRationaleDialog>();
            return;
        }
        else if (dialogue != null)
        {
            Destroy(dialogue);
        }
#endif
    }

    Vector3 circleUp(Vector3 center, float radius, float ang)
    {
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.z;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
    Vector3 velocity;
    // Update is called once per frame
    void Update()
    {
        if (!recorder.IsRecording)
        {
            recorder.RestartRecording();
            recorder.StartRecording();
        }

       /* string newText = "";
        newText += recorder.LevelMeter.CurrentAvgAmp + "|" + recorder.LevelMeter.CurrentPeakAmp + Environment.NewLine;
        newText += "Is Recording: " + recorder.IsRecording + Environment.NewLine;
        newText += "Is ActiveAndEnabled: " + recorder.isActiveAndEnabled + Environment.NewLine;
        newText += "Is CurrentlyTransmitting: " + recorder.IsCurrentlyTransmitting + Environment.NewLine;
        newText += "Is Initialized: " + recorder.IsInitialized + Environment.NewLine;
        newText += "PhotonMicrophoneDeviceId: " + recorder.PhotonMicrophoneDeviceId + System.Environment.NewLine;
        newText += "PhotonMicrophoneEnumerator Count: " + Recorder.PhotonMicrophoneEnumerator.Count + Environment.NewLine;
        var enumerator = Recorder.PhotonMicrophoneEnumerator;
        string trying = "";
        for (int i = 0; i < enumerator.Count; i++)
        {
            trying = enumerator.IDAtIndex(i) + "";
            recorder.PhotonMicrophoneDeviceId = enumerator.IDAtIndex(i);
            recorder.IsRecording = true;
            recorder.TransmitEnabled = true;
            break;
        }

        newText += "Trying to set mic: " + trying + Environment.NewLine;
        if (Microphone.devices == null || Microphone.devices.Length == 0)
        {
            newText += "No microphone device detected! " + Environment.NewLine;
        }
        else if (Microphone.devices.Length == 1)
        {
            newText += string.Format("Mic.: {0}", Microphone.devices[0]) + Environment.NewLine;
        }
        else
        {
            newText += string.Format("Multi.Mic.Devices:\n0. {0} (active)\n", Microphone.devices[0]) + Environment.NewLine;
            for (int i = 1; i < Microphone.devices.Length; i++)
            {
                newText += string.Format("{0}. {1}\n", i, Microphone.devices[i]) + Environment.NewLine;
            }
        }
        debugObject.text = newText;*/

        // If B button pressed, change whether pause menu is up or down
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            PauseMenuChange();
        }

        circleCenter.transform.rotation = Camera.main.transform.rotation;

        //sets song lyrics
        if (songLyrics != null)
        {
            lyrics.text = songLyrics.First().text;
            List<LyricLine> lyr = songLyrics.Where(x => x.time < Convert.ToDouble(audioSource.time) - 0.5).ToList();
            if (lyr.Count > 0)
            {
                lyrics.text = lyr.Last().text;
            }
        }

        //properly roates the canvas and menu to face towards the user at all times
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
        Vector3 newV = new Vector3(circleCenter.transform.position.x + lyricsDepth * circleCenter.transform.forward.x, lyricsHeight, circleCenter.transform.position.z + lyricsDepth * circleCenter.transform.forward.z);
        canvas.transform.position = Vector3.SmoothDamp(canvas.transform.position, newV, ref velocity, 0.5f * Time.deltaTime);

        Vector3 newV2 = new Vector3(Camera.main.transform.position.x + menuDepth * Camera.main.transform.forward.x, ground.transform.position.y + menuHeight, Camera.main.transform.position.z + menuDepth * Camera.main.transform.forward.z);
        menuPanel.transform.position = Vector3.SmoothDamp(menuPanel.transform.position, newV2, ref velocity, 1f * Time.deltaTime);

        Vector3 newV3 = new Vector3(Camera.main.transform.position.x + menuDepth * Camera.main.transform.forward.x, ground.transform.position.y + menuHeight + 0.5f, Camera.main.transform.position.z + menuDepth * Camera.main.transform.forward.z);
        pauseMenu.transform.position = Vector3.SmoothDamp(pauseMenu.transform.position, newV3, ref velocity, 1f * Time.deltaTime);

        if (counter % 5 == 0)
        {
            //updates audio visualization (specifically height of cubes) based on the current spectrum
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            float average = getAvg(spectrum);
            for (int j = 0; j < numCubes; j++)
            {
                //changes size of cubes in spectrum
                cubes[j].transform.localScale = new Vector3(cubes[j].transform.localScale.x, (spectrum[j] * 1000 * cubeScaler), cubes[j].transform.localScale.z);

                //updates the color of the audio visualization based on the color menu selection

                if (spectrum[j] > cubeThreshold * average)
                {
                    for (int i = 0; i < particleCount; i++)
                    {
                       float scale = UnityEngine.Random.Range(particleMinSize*100, particleMaxSize*100);
                       asteroid.transform.localScale = new Vector3(scale, scale, scale);
                       Instantiate(asteroid, cubes[j].transform.position + new Vector3(0,25,0), Quaternion.identity);
                    }
                }
            }
        }
        counter++; 
    }

    //gets the average height of the spectrum
    float getAvg(float[] spectrum)
    {
        float num = 0;
        for (int j = 0; j < numCubes; j++)
        {
            num += spectrum[j];
        }
        float avg = num / numCubes;
        return (avg);
    }

    //roates the color array (turns out this can be timely and kind of distracting)
    void rotateColors(Color[] colors)
    {
        Color temp = colors[0];
        for (int i = 0; i < numCubes - 1; i++)
        {
            colors[i] = colors[i + 1];
        }
        colors[numCubes - 1] = temp;
    }

    //gets the lyrics from the lyrics file loaded in and returns a list of lyrics
    List<LyricLine> getLyrics(string song)
    {
        TextAsset lyric = Resources.Load<TextAsset>("Lyrics/" + song);
        //StreamReader inp_stm = new StreamReader(Application.dataPath + "/Lyrics/" + Properties.lyricsFile[0] + ".lrc");
        StreamReader inp_stm = new StreamReader(new MemoryStream(lyric.bytes));
        List<LyricLine> inp_ln = new List<LyricLine>();
        while (!inp_stm.EndOfStream)
        {
            string line = inp_stm.ReadLine();
            if (isValidReg(line))
            {
                var culture = new CultureInfo("en-US");
                var formats = new string[] {
                        @"mm\:ss\.ff"
                    };
                string timeTxt = line.Substring(0, line.IndexOf("]") + 1).Replace("[", "").Replace("]", "");

                inp_ln.Add(new LyricLine(TimeSpan.ParseExact(timeTxt, formats, culture.NumberFormat).TotalSeconds, line.Substring(line.IndexOf("]") + 1)));
            }
        }

        inp_stm.Close(); inp_stm.Close();
        return inp_ln;
    }

    bool isValidReg(string str)
    {
        Regex rgx = new Regex(@"^\[[0-9]{2}:[0-9]{2}\.[0-9]{2}\].*$");
        return rgx.IsMatch(str);
    }

    //the following are methods called by the controller menu
    public void playMusic()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(PlayMusicEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void playMusicProcess()
    {
        if (audioSource.time > 0)
        {
            audioSource.UnPause();

        }
        else
        {
            audioSource.Play();
        }
        playButton.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void pauseMusic()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(PauseMusicEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void pauseMusicProcess()
    {
        audioSource.Pause();
       // preFabScale = 100;
        pauseButton.SetActive(false);
        playButton.SetActive(true);
    }

    public void rewindMusic()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(RewindMusicEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void rewindMusicProcess()
    {
        audioSource.time -= 10;
    }

    public void fastForwardMusic()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(FastFowardEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void fastForwardMusicProcess()
    {
        audioSource.time += 10;
    }

    void LoadSong(string song)
    {
        AudioClip lyric = Resources.Load<AudioClip>("Music/" + song);
        audioSource.clip = lyric;
        songLyrics = getLyrics(song);
        albumArt.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("AlbumArt/" + song);
        string[] songArr = song.Split('-');
        if (songArr.Length > 1)
            songName.GetComponent<TextMeshProUGUI>().text = songArr[0].Trim() + Environment.NewLine + songArr[1].Trim();
        else
            songName.GetComponent<TextMeshProUGUI>().text = songArr[0];

    }

    public void PauseMenuChange()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(PauseMenuChangeEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    public void PauseMenuChangeProcess()
    {
        if (pauseMenuUp)
        {
            pauseMenu.SetActive(false);
            playMusic();
        }
        else if (!pauseMenuUp)
        {
            pauseMenu.SetActive(true);
            pauseMusic();
        }

        pauseMenuUp = !pauseMenuUp;
    }

    public void backToJukeBox()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(SendClientsToJukeBoxEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void backToJukeBoxProcess()
    {
        PhotonNetwork.LeaveRoom();
    }

    //sets all cubes in the spectrum white

    public void setWhite()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(SetWhiteEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }
    void setWhiteProcess()
    {
        currentColor = "white";
        whiteObjects.active = true;
        blueObjects.active = false;
        greenObjects.active = false;
        orangeObjects.active = false;
        multiObjects.active = false;
        pinkObjects.active = false;


        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.02f, 0f, 0.1f, 0.5f, 1f));
            Renderer rend = ground.GetComponent<Renderer>();          
        }

        cubeThreshold = 1;
        particleMinSize = 0.02f;
        particleMaxSize = 0.05f;

        RenderSettings.skybox = skyboxWhite;
    }

    public void setOrange()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(SetOrangeEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void setOrangeProcess()
    {
        currentColor = "orange";

        whiteObjects.active = false;
        blueObjects.active = false;
        greenObjects.active = false;
        orangeObjects.active = true;
        multiObjects.active = false;
        pinkObjects.active = false;

        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.1f, 1f, 1f, 0.5f, 1f));
        }
        cubeThreshold = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;

        RenderSettings.skybox = skyboxOrange;
    }

    public void setGreen()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(SetGreenEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void setGreenProcess()
    {
        currentColor = "green";

        whiteObjects.active = false;
        blueObjects.active = false;
        greenObjects.active = true;
        orangeObjects.active = false;
        multiObjects.active = false;
        pinkObjects.active = false;

        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.2f, 0.4f, 1f, 1f, 0.5f, 1f));
        }

        cubeThreshold = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;

        RenderSettings.skybox = skyboxGreen;
    }

    public void setBlue()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(SetBlueEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void setBlueProcess()
    {
        currentColor = "blue";

        whiteObjects.active = false;
        blueObjects.active = true;
        greenObjects.active = false;
        orangeObjects.active = false;
        multiObjects.active = false;
        pinkObjects.active = false;

        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.5f, 0.7f, 1f, 1f, 0.5f, 1f));
        }

        cubeThreshold = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;

        RenderSettings.skybox = skyboxBlue;
    }

    public void setPink()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(SetPinkEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }
    void setPinkProcess()
    {
        currentColor = "pink";

        whiteObjects.active = false;
        blueObjects.active = false;
        greenObjects.active = false;
        orangeObjects.active = false;
        multiObjects.active = false;

        pinkObjects.active = true;

        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.8f, 0.9f, 1f, 1f, 0.5f, 1f));
        }

        cubeThreshold = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;

        RenderSettings.skybox = skyboxPink;
    }

    public void setMulti()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(SetMultiEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    public void setMultiProcess()
    {
        currentColor = "multi";

        pinkObjects.active = false;
        whiteObjects.active = false;
        blueObjects.active = false;
        greenObjects.active = false;
        orangeObjects.active = false;

        multiObjects.active = true;

        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }

        cubeThreshold = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;

        RenderSettings.skybox = skyboxMulti;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == InstantiateVrAvatarEventCode)
        {
            Debug.Log("Entered instantiate vr avatar event code");

            GameObject remoteAvatar = Instantiate(Resources.Load(avatarPrefabs[UnityEngine.Random.Range(0, avatarPrefabs.Count)].name)) as GameObject;
            PhotonView photonView = remoteAvatar.GetComponent<PhotonView>();
            photonView.ViewID = (int)photonEvent.CustomData;
            Debug.Log("Instantiated Remote Avatar with View ID:");
            Debug.Log(photonView.ViewID);
        }
        else if (photonEvent.Code == PlayMusicEventCode) 
        {
            playMusicProcess();
        }
        else if (photonEvent.Code == PauseMusicEventCode)
        {
            pauseMusicProcess();
        }
        else if (photonEvent.Code == RewindMusicEventCode)
        {
            rewindMusicProcess();
        }
        else if (photonEvent.Code == FastFowardEventCode)
        {
            fastForwardMusicProcess();
        }
        else if (photonEvent.Code == PauseMenuChangeEventCode)
        {
            PauseMenuChangeProcess();
        }
        else if (photonEvent.Code == SendClientsToJukeBoxEventCode)
        {
            backToJukeBoxProcess();
        }
        else if (photonEvent.Code == SetWhiteEventCode)
        {
            setWhiteProcess();
        }
        else if (photonEvent.Code == SetOrangeEventCode)
        {
            setOrangeProcess();
        }
        else if (photonEvent.Code == SetGreenEventCode)
        {
            setGreenProcess();
        }
        else if (photonEvent.Code == SetBlueEventCode)
        {
            setBlueProcess();
        }
        else if (photonEvent.Code == SetPinkEventCode)
        {
            setPinkProcess();
        }
        else if (photonEvent.Code == SetMultiEventCode)
        {
            setMultiProcess();
        }

    }

}