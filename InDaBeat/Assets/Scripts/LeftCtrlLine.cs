using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftCtrlLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    public GameObject controller;
    private float sightLength = 2.0F;
    private Vector3[] points = new Vector3[2];
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.03F;
        lineRenderer.endWidth = 0.03F;
    }

    // Update is called once per frame
    void Update()
    {
        points[0] = controller.transform.position;
        points[1] = controller.transform.position + (sightLength * controller.transform.forward);
        lineRenderer.SetPositions(points);
    }
}
