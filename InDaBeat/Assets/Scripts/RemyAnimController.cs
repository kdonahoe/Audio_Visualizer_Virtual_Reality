using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemyAnimController : MonoBehaviour
{
    static Animator anim;
    public float speed = 2.0f;
    public float rotationSpeed = 75.0f;
    public GameObject camera;
    float cameraDistance;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        //cameraDistance = camera.transform.position.z - transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
        //camera.transform.Translate(0, 0, translation);
        //camera.transform.Rotate(0, rotation / cameraDistance, 0);

        if (translation != 0)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
}
