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
    public GameObject blueSpark, pinkSpark, orangeSpark, greenSpark;
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

    void Start()
    {
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

        /*
        //Doesn't quite work yet
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
        */
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
                      //  float scale = UnityEngine.Random.Range(particleMinSize*800, particleMaxSize*800);
                      //  asteroid.transform.localScale = new Vector3(scale, scale, scale);
                      //  Instantiate(asteroid, cubes[j].transform.position + new Vector3(0,5,0), Quaternion.identity);
                    }
                }
                        /*
                        if (spectrum[j] > cubeThreshold * average)
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
                        */
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

    //sets all cubes in the spectrum white
    public void setWhite()
    {
        currentColor = "white";

        blueObjects.active = false;
        greenObjects.active = true;
        orangeObjects.active = false;
        multiObjects.active = false;
        pinkObjects.active = false;


        foreach (GameObject cube in cubeList)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.02f, 0f, 0.1f, 0.5f, 1f));
            Renderer rend = ground.GetComponent<Renderer>();          
        }

        /*
        for (int i = 0; i < 128; i++)
        {
            Destroy(cubeList[i]);
            Vector3 center = transform.position;
            float ang = 2.8125f * i;
            Vector3 pos = RandomCircle(center, 6.5f, ang);
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            GameObject cubeInstance = Instantiate(cubePrefab, pos, rot);

            var cubeRenderer = cubeInstance.GetComponent<Renderer>();

            colors[i] = UnityEngine.Random.ColorHSV(0f, 0.02f, 0f, 0.1f, 0.5f, 1f);
            cubeRenderer.material.SetColor("_Color", colors[i]);

            cubes[i] = cubeInstance;
            cubeList.Add(cubeInstance);
        }
       

        Vector3 center = circleCenter.transform.position;

        if(otherObjects.Count > 0)
        {
            for (int i = 0; i < otherObjects.Count; i++)
            {
                otherObjects.Remove(otherObjects[i]);
            }
            otherObjects.Clear();
        }
        
     
        for (int i = 0; i < 128; i++)
        {
            float ang = (2.8125f) * i;
            float dist = UnityEngine.Random.value * 10 + 30.0f;
            Vector3 pos = RandomCircle(center, dist, ang);
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            otherObjects.Add(Instantiate(tree, pos, rot));
        }
         */
        cubeThreshold = 1;
        particleMinSize = 0.02f;
        particleMaxSize = 0.05f;

        RenderSettings.skybox = skyboxWhite;
    }

    public void setOrange()
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

    
}
