using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkBehavior : MonoBehaviour
{
    public Vector3 startVelocity;
    public float delay;
    public float shrinkSmooth;
    private Rigidbody rb;
    GameObject sceneControl;
    string color;
    // Start is called before the first frame update
    void Start()
    {
        sceneControl = GameObject.Find("Scene Controller");
        color = sceneControl.GetComponent<SceneControllerScript>().currentColor;
        rb = GetComponent<Rigidbody>();
        rb.velocity = Random.rotation * startVelocity;
        Invoke("Die", delay);

        var particleRenderer = transform.GetComponent<Renderer>();
        particleRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
    }

    private void Update()
    {
        //scale localtozero
        Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSmooth); color = sceneControl.GetComponent<SceneControllerScript>().currentColor;

        /*
        if (color != "default")
        {
            if (color == "white")
            {
                var particleRenderer = transform.GetComponent<Renderer>();
                particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.1f, 0f, 0.1f, 0.5f, 1f));
            }
            else if (color == "orange")
            {
                var particleRenderer = transform.GetComponent<Renderer>();
                particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.1f, 1f, 1f, 0.5f, 1f));
            }
            else if (color == "green")
            {
                var particleRenderer = transform.GetComponent<Renderer>();
                particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.2f, 0.4f, 1f, 1f, 0.5f, 1f));
            }
            else if (color == "blue")
            {
                var particleRenderer = transform.GetComponent<Renderer>();
                particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.5f, 0.7f, 1f, 1f, 0.5f, 1f));
            }

            else if (color == "pink")
            {
                var particleRenderer = transform.GetComponent<Renderer>();
                particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.8f, 0.9f, 1f, 1f, 0.5f, 1f));
            }
            else if (color == "multi")
            {
                var particleRenderer = transform.GetComponent<Renderer>();
                particleRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            }
        }
        */


    }
    void Die()
    {
        Destroy(gameObject);
    }

}
