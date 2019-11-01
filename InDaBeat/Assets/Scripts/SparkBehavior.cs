using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkBehavior : MonoBehaviour
{
    public Vector3 startVelocity;
    public float delay;
    public float shrinkSmooth;
    private Rigidbody rb;

    string sceneString;
    // Start is called before the first frame update
    void Start()
    {
        //sceneString = "snow";

        rb = GetComponent<Rigidbody>();
        rb.velocity = Random.rotation * startVelocity;
        Invoke("Die", delay);

        setScene("autumn");
    }

    private void Update()
    {
        //scale localtozero
        Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSmooth);

        if(transform.position.y <= 0)
        {
        //    Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }

    void setScene(string sceneString)
    {
        var particleRenderer = transform.GetComponent<Renderer>();
        if (sceneString == "snow")
        {
            particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.1f, 0f, 0.1f, 0.5f, 1f));
        }
        else if (sceneString == "autumn")
        {
            particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 0.1f, 1f, 1f, 0.5f, 1f));
        }
        else if (sceneString == "green")
        {
            particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.2f, 0.4f, 1f, 1f, 0.5f, 1f));
        }
        else if (sceneString == "blue")
        {
            particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.5f, 0.7f, 1f, 1f, 0.5f, 1f));
        }

        else if (sceneString == "pink")
        {
            particleRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV(0.8f, 0.9f, 1f, 1f, 0.5f, 1f));
        }
        else
        {
            particleRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }
    }
}
