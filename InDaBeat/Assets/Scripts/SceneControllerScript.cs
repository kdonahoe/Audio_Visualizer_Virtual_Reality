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

[RequireComponent(typeof(AudioSource))]
public class SceneControllerScript : MonoBehaviour
{
    public GameObject canvas;
    AudioSource audioSource;
    int numCubes = 256;
    public float[] spectrum = new float[512];

    public GameObject cubePrefab, ground;
    GameObject[] cubes = new GameObject[512];
    public float scaler;

    public Color[] colors = new Color[512];

    public float circleSize;
    public int preFabScale;
    int counter = 0;

    public GameObject particlePrefab, menuPanel, circleCenter;
    public int particleCount;
    public float particleMinSize;
    public float particleMaxSize;
    private bool alreadyExploded;
    List<LyricLine> songLyrics;
    public TextMeshProUGUI lyrics;
    Vector3 offset;
    public float lyricsDepth, menuDepth;
    public float lyricsHeight, menuHeight;

    void Start()
    {
        offset = canvas.transform.position - Camera.main.transform.position;
        audioSource = GetComponent<AudioSource>();
		AudioClip lyric = Resources.Load<AudioClip>("Music/" + Properties.selectedSong);
		audioSource.clip = lyric;
        songLyrics = convertToText();
        audioSource.Play();

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
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
        //canvas.transform.LookAt(Camera.main.transform, Vector3.up);
        //canvas.transform.rotation.eulerAngles = new Vector3(0, 0, 0);
        Vector3 newV = new Vector3(circleCenter.transform.position.x + lyricsDepth * circleCenter.transform.forward.x, lyricsHeight, circleCenter.transform.position.z + lyricsDepth * circleCenter.transform.forward.z);
        canvas.transform.position = Vector3.SmoothDamp(canvas.transform.position, newV, ref velocity, 0.5f * Time.deltaTime);

        Vector3 newV2 = new Vector3(Camera.main.transform.position.x + menuDepth * Camera.main.transform.forward.x, ground.transform.position.y + menuHeight, Camera.main.transform.position.z + menuDepth * Camera.main.transform.forward.z);
        menuPanel.transform.position = Vector3.SmoothDamp(menuPanel.transform.position, newV2, ref velocity, 1f * Time.deltaTime);
        //canvas.transform.position = Vector3.SmoothDamp(canvas.transform.position, Camera.main.transform.position + 20f * Camera.main.transform.forward + new Vector3(-Camera.main.transform.position.x, 2f,0), ref velocity, 1f * Time.deltaTime);
        if (counter % 5 == 0)
        {
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            float average = getAvg(spectrum);
            print(average);
            for (int j = 0; j < numCubes; j++)
            {
                cubes[j].transform.localScale = new Vector3(cubes[j].transform.localScale.x, (spectrum[j] * 1000 * scaler), cubes[j].transform.localScale.z);

                if (j > 0)
                {
                    var cubeRenderer = cubes[j].GetComponent<Renderer>();
                    cubeRenderer.material.SetColor("_Color", colors[j]);
                }

                if (spectrum[j] > preFabScale * average)
                {
                    for (int i = 0; i < particleCount; i++)
                    {
                        float scale = UnityEngine.Random.Range(particleMinSize, particleMaxSize);
                        particlePrefab.transform.localScale = new Vector3(scale, scale, scale);
                        Instantiate(particlePrefab, cubes[j].transform.position, Quaternion.identity);
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
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                Interact(hit);
        }
#else
        if (Input.touchCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                Interact(hit);
        }
#endif
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

    void Interact(RaycastHit hit)
    {
        if (hit.collider.CompareTag("User"))
        {
            //songLyrics = webUtils.getTopLyrics(Properties.songsList[0]);
            songLyrics = convertToText();
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.Play();
            }
        }
    }

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

    List<LyricLine> convertToText()
    {
        TextAsset lyric = Resources.Load<TextAsset>("Lyrics/" + Properties.lyricsFile[0]);
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
}
