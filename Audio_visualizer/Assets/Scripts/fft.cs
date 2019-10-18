using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class fft : MonoBehaviour
{
    AudioSource audioSource;
    int numCubes = 256;
    public float[] spectrum = new float[512];

    public GameObject cubePrefab;
    GameObject[] cubes = new GameObject[512];
    public float scaler;

    public Color[] colors = new Color[512];

    public int preFabScale;
    int numRotation = 0;

    public GameObject particlePrefab;
    public int particleCount;
    public float particleMinSize;
    public float particleMaxSize;
    private bool alreadyExploded;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < numCubes; i++)
        {
            GameObject cubeInstance = (GameObject)Instantiate(cubePrefab);
            cubeInstance.transform.parent = this.transform;

            var cubeRenderer = cubeInstance.GetComponent<Renderer>();

            colors[i] = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            cubeRenderer.material.SetColor("_Color", colors[i]);

            if (numCubes == 512)
            {
                this.transform.eulerAngles = new Vector3(0, -0.703f * i, 0);
                cubeInstance.transform.position = Vector3.forward * 100;
            }
            if (numCubes == 256)
            {
                this.transform.eulerAngles = new Vector3(0, -3.2f * i, 0);
                cubeInstance.transform.position = Vector3.forward * 30;
            }
            if(numCubes == 128)
            {
                this.transform.eulerAngles = new Vector3(0, -4.5f * i, 0);
                cubeInstance.transform.position = Vector3.forward * 12;
            }
            
            cubes[i] = cubeInstance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        float average = getAvg(spectrum);
        print(average);
        for (int j = 0; j < numCubes; j++)
        {  
               cubes[j].transform.localScale = new Vector3(cubes[j].transform.localScale.x, (spectrum[j] * 1000* scaler), cubes[j].transform.localScale.z);
 
               if(j>0)
                {
                    var cubeRenderer = cubes[j].GetComponent<Renderer>();
                    cubeRenderer.material.SetColor("_Color", colors[j]);
                }   

               if(spectrum[j] >preFabScale*average)
                {
                for (int i = 0; i < particleCount; i++)
                {
                    float scale = Random.Range(particleMinSize, particleMaxSize);
                    particlePrefab.transform.localScale = new Vector3(scale, scale, scale);
                    Instantiate(particlePrefab, cubes[j].transform.position, Quaternion.identity);
                }
            }


        }
        numRotation++;

        if(numRotation % 60 == 0) //every 20 times
        {
           // rotateColors(colors);
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
}
