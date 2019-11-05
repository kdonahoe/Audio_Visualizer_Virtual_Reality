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

[RequireComponent(typeof(AudioSource))]
public class SceneControllerScript : MonoBehaviour
{
    public OVRInput.Controller controller;
    public GameObject canvas;
    AudioSource audioSource;
    int numCubes = 256;
    public float[] spectrum = new float[512];

    public GameObject cubePrefab, ground;
    public GameObject snow;
    GameObject[] cubes = new GameObject[512];
    public float cubeScaler;

    public Color[] colors = new Color[512];

    public float circleSize;
    public int preFabScale;
    int counter = 0;

    public GameObject particlePrefab, menuPanel, circleCenter, playButton, pauseButton, albumArt, songName;
    public GameObject blueSpark, pinkSpark, orangeSpark, greenSpark;
    public GameObject pauseMenu, resumeButton, backJukeBoxButton;
    public int particleCount;
    public float particleMinSize;
    public float particleMaxSize;
    private bool alreadyExploded;
    List<LyricLine> songLyrics;
    public TextMeshProUGUI lyrics;
    Vector3 offset;
    public float lyricsDepth, menuDepth;
    public float lyricsHeight, menuHeight;

    public List<GameObject> cubeList = new List<GameObject>();

    public string currentColor;
    void Start()
    {
        currentColor = "multi";
        offset = canvas.transform.position - Camera.main.transform.position;
        audioSource = GetComponent<AudioSource>();
        LoadSong(Properties.selectedSong);

        for (int i = 0; i < numCubes; i++)
        {
            GameObject cubeInstance = (GameObject)Instantiate(cubePrefab);
            cubeInstance.transform.parent = this.transform;

            var cubeRenderer = cubeInstance.GetComponent<Renderer>();

            colors[i] = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            cubeRenderer.material.SetColor("_Color", colors[i]);

            if (numCubes == 512)
            {
                this.transform.eulerAngles = new Vector3(0, -0.703f * i, 0);
                cubeInstance.transform.position = Vector3.forward * 100 * circleSize;
            }
            if (numCubes == 256)
            {
                this.transform.eulerAngles = new Vector3(0, -3.55f * i, 0);
                cubeInstance.transform.position = Vector3.forward * 30 * circleSize;
            }
            if (numCubes == 128)
            {
                this.transform.eulerAngles = new Vector3(0, -5.5f * i, 0);
                cubeInstance.transform.position = Vector3.forward * 12 * circleSize;
            }
            cubeInstance.transform.position = new Vector3(cubeInstance.transform.position.x, ground.transform.position.y, cubeInstance.transform.position.z);
            cubes[i] = cubeInstance;
            cubeList.Add(cubeInstance);


        }
    }
    Vector3 velocity;
    // Update is called once per frame
    void Update()
    {
        circleCenter.transform.rotation = Camera.main.transform.rotation;
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote) && OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
        {
            OVRInput.Update();
            OVRInput.SetControllerVibration(60, 60, OVRInput.Controller.RTouch);
        }
        if (OVRInput.Get(OVRInput.Button.One))
        {
            audioSource.Pause();
            pauseMenu.SetActive(true);
        }
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
        //canvas.transform.LookAt(Camera.main.transform, Vector3.up);
        //canvas.transform.rotation.eulerAngles = new Vector3(0, 0, 0);
        Vector3 newV = new Vector3(circleCenter.transform.position.x + lyricsDepth * circleCenter.transform.forward.x, lyricsHeight, circleCenter.transform.position.z + lyricsDepth * circleCenter.transform.forward.z);
        canvas.transform.position = Vector3.SmoothDamp(canvas.transform.position, newV, ref velocity, 0.5f * Time.deltaTime);

        Vector3 newV2 = new Vector3(Camera.main.transform.position.x + menuDepth * Camera.main.transform.forward.x, ground.transform.position.y + menuHeight, Camera.main.transform.position.z + menuDepth * Camera.main.transform.forward.z);
        menuPanel.transform.position = Vector3.SmoothDamp(menuPanel.transform.position, newV2, ref velocity, 1f * Time.deltaTime);

        Vector3 newV3 = new Vector3(Camera.main.transform.position.x + menuDepth * Camera.main.transform.forward.x, ground.transform.position.y + menuHeight + 0.5f, Camera.main.transform.position.z + menuDepth * Camera.main.transform.forward.z);
        pauseMenu.transform.position = Vector3.SmoothDamp(pauseMenu.transform.position, newV3, ref velocity, 1f * Time.deltaTime);
        //canvas.transform.position = Vector3.SmoothDamp(canvas.transform.position, Camera.main.transform.position + 20f * Camera.main.transform.forward + new Vector3(-Camera.main.transform.position.x, 2f,0), ref velocity, 1f * Time.deltaTime);
        if (counter % 5 == 0)
        {
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            float average = getAvg(spectrum);
            print(average);
            for (int j = 0; j < numCubes; j++)
            {
                cubes[j].transform.localScale = new Vector3(cubes[j].transform.localScale.x, (spectrum[j] * 1000 * cubeScaler), cubes[j].transform.localScale.z);


                if (spectrum[j] > preFabScale * average)
                {
                    for (int i = 0; i < particleCount; i++)
                    {
                        if(currentColor == "white")
                        {

                            float scale = UnityEngine.Random.Range(particleMinSize, particleMaxSize);
                            snow.transform.localScale = new Vector3(scale, scale, scale);
                            Instantiate(snow, cubes[j].transform.position, Quaternion.identity);
                        }
                        else if(currentColor == "orange")
                        {
                            float scale = UnityEngine.Random.Range(particleMinSize, particleMaxSize);
                            orangeSpark.transform.localScale = new Vector3(scale, scale, scale);
                            Instantiate(orangeSpark, cubes[j].transform.position, Quaternion.identity);
                        }
                        else if (currentColor == "green")
                        {
                            float scale = UnityEngine.Random.Range(particleMinSize, particleMaxSize);
                            greenSpark.transform.localScale = new Vector3(scale, scale, scale);
                            Instantiate(greenSpark, cubes[j].transform.position, Quaternion.identity);
                        }
                        else if (currentColor == "blue")
                        {
                            float scale = UnityEngine.Random.Range(particleMinSize, particleMaxSize);
                            blueSpark.transform.localScale = new Vector3(scale, scale, scale);
                            Instantiate(blueSpark, cubes[j].transform.position, Quaternion.identity);
                        }
                        else if (currentColor == "pink")
                        {
                            float scale = UnityEngine.Random.Range(particleMinSize, particleMaxSize);
                            pinkSpark.transform.localScale = new Vector3(scale, scale, scale);
                            Instantiate(pinkSpark, cubes[j].transform.position, Quaternion.identity);
                        }
                        else
                        {
                            float scale = UnityEngine.Random.Range(particleMinSize, particleMaxSize);
                            particlePrefab.transform.localScale = new Vector3(scale, scale, scale);
                            Instantiate(particlePrefab, cubes[j].transform.position, Quaternion.identity);
                        }
                        
                    }
                }
            }
        }
        counter++;
        //    numRotation++;

        //  if(numRotation % 60 == 0) //every 20 times
        //   {
        // rotateColors(colors);
        // }
//#if UNITY_EDITOR
//        if (Input.GetMouseButtonUp(0))
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit))
//                Interact(hit);
//        }
//#else
//        if (Input.touchCount > 0)
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit))
//                Interact(hit);
//        }
//#endif
        if (songLyrics != null)
        {
            lyrics.text = songLyrics.First().text;
            List<LyricLine> lyr = songLyrics.Where(x => x.time < Convert.ToDouble(audioSource.time) - 0.5).ToList();
            if (lyr.Count > 0)
            {
                lyrics.text = lyr.Last().text;
            }
        }
    }

    //void Interact(RaycastHit hit)
    //{
    //    if (hit.collider.CompareTag("User"))
    //    {
    //        //songLyrics = webUtils.getTopLyrics(Properties.songsList[0]);
    //        songLyrics = convertToText();
    //        if (audioSource.isPlaying)
    //        {
    //            audioSource.Pause();
    //        }
    //        else
    //        {
    //            audioSource.Play();
    //        }
    //    }
    //}

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
    void rotateColors(Color[] colors)
    {
        Color temp = colors[0];
        for (int i = 0; i < numCubes - 1; i++)
        {
            colors[i] = colors[i + 1];
        }
        colors[numCubes - 1] = temp;
    }

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
                // Do Something with the input. 
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

    public void playMusic()
    {
        audioSource.Play();
        playButton.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void pauseMusic()
    {
        audioSource.Pause();
       // preFabScale = 100;
        pauseButton.SetActive(false);
        playButton.SetActive(true);
    }

    public void rewindMusic()
    {
        audioSource.time -= 10;
    }

    public void fastForwardMusic()
    {
        audioSource.time += 10;
    }

    public void setWhite()
    {
        currentColor = "white";
        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.02f, 0f, 0.1f, 0.5f, 1f));
            Renderer rend = ground.GetComponent<Renderer>();          
        }
        preFabScale = 1;
        particleMinSize = 0.02f;
        particleMaxSize = 0.05f;
    }

    public void setOrange()
    {
        currentColor = "orange";
        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.1f, 1f, 1f, 0.5f, 1f));
        }
        preFabScale = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;
    }

    public void setGreen()
    {
        currentColor = "green";
        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.2f, 0.4f, 1f, 1f, 0.5f, 1f));
        }

        preFabScale = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;
    }

    public void setBlue()
    {
        currentColor = "blue";
        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.5f, 0.7f, 1f, 1f, 0.5f, 1f));
        }

        preFabScale = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;
    }


    public void setPink()
    {
        currentColor = "pink";
        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.8f, 0.9f, 1f, 1f, 0.5f, 1f));
        }

        preFabScale = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;
    }

    public void setMulti()
    {
        currentColor = "multi";
        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }

        preFabScale = 3;
        particleMinSize = 0.01f;
        particleMaxSize = 0.2f;
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

    public void ResumeSong()
    {
        audioSource.Play();
        pauseMenu.SetActive(false);
    }

    public void backToJukeBox()
    {
        SceneManager.LoadScene("SongSelector");
    }
}
