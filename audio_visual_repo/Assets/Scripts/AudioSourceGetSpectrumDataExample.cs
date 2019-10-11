using UnityEngine;



[RequireComponent(typeof(AudioSource))]

public class AudioSourceGetSpectrumDataExample : MonoBehaviour
{
    public GameObject cube;

    private void Start()
    {
        var cubeRenderer = cube.GetComponent<Renderer>();
        //Call SetColor using the shader property name "_Color" and setting the color to red
        cubeRenderer.material.SetColor("_Color", Color.red);
        if (cube.transform.gameObject.name.Contains("2") || cube.transform.gameObject.name.Contains("4"))
        {
            cubeRenderer.material.SetColor("_Color", Color.magenta);
        }
    }
    void Update()
    {
        float[] spectrum = new float[256];

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 1; i < spectrum.Length - 1; i++)
        {
              Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
              Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
              Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
              Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);

            if (i % 2 == 0)
            {
                if (cube.transform.gameObject.name.Contains("1"))
                {
                    cube.transform.position = new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3);
                }

                if (cube.transform.gameObject.name.Contains("3"))
                {
                    cube.transform.position = new Vector3(Mathf.Log(i) + 4, Mathf.Log(spectrum[i]), 3);
                }

                if (cube.transform.gameObject.name.Contains("5"))
                {
                    cube.transform.position = new Vector3(Mathf.Log(i) + 8, Mathf.Log(spectrum[i]), 3);
                }
            }
            else
            {
                if (cube.transform.gameObject.name.Contains("2"))
                {
                    cube.transform.position = new Vector3(Mathf.Log(i) + 2, Mathf.Log(spectrum[i]), 3);
                }
                if (cube.transform.gameObject.name.Contains("4"))
                {
                    cube.transform.position = new Vector3(Mathf.Log(i) + 6, Mathf.Log(spectrum[i]), 3);
                }
            }
            

        }
    }
}
