/*
 * Author: Eric Ranaldi
 * Date: 2/20/2020
 * 
 * Purpose: A simple script that provides functionality that has the camera follow the player. Extremely simple not camera
 * lag or user defineable settings.
 * 
 * Credits: The work is solely mine I am only adding this to ensure I do not get docked points for not having a credits section
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject followObject;

    //How fare above the character do we want the camera?
    private Vector3 cameraOffset = new Vector3(0.0f, 19.0f, -10.0f);

    // Start is called before the first frame update
    void Start()
    {
        if (followObject)
        {
            gameObject.transform.position = followObject.transform.position + cameraOffset;
        }
        else
        {
            Debug.LogError("Follow Object not set in CameraFollow Script. Attempting to set with 'Player' :: " + gameObject.name);

            followObject = GameObject.Find("Player");

            if(!followObject)
            {
                Debug.LogError("Coult not find 'Player' object. Camera not attached and is not following");
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(followObject)
        {
            Vector3 newPostion = new Vector3(followObject.transform.position.x, 0.0f, followObject.transform.position.z);
            gameObject.transform.position = newPostion + cameraOffset;
        }

    }
}
