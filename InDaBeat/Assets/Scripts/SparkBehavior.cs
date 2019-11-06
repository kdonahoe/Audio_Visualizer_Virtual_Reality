using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkBehavior : MonoBehaviour
{
    public Vector3 startVelocity;
    public float delay;
    public float shrinkSmooth;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Random.rotation * startVelocity;
        Invoke("Die", delay);

        var particleRenderer = transform.GetComponent<Renderer>();
        particleRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
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
}
