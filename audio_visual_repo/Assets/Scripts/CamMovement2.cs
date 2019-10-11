using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement2 : MonoBehaviour
{
    //The target object that this camera is focosing on
    public GameObject targetObject;

    //A float used to tweak the camera movement
    [SerializeField]
    float cameraMovementSmooth = 1.0f;

    //An offset between camera and the target obejct
    Vector3 offset;

    void Start()
    {
        //Calculate the offset between the camera and the target game object
        //subtracting player position from camera position
        offset = transform.position - targetObject.transform.position;
    }

    void Update()
    {
        // offset to the camera
        transform.position = targetObject.transform.position + offset;
    }
}
