using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraControllerISOFollow : MonoBehaviour {

    // Use this for initialization
    public GameObject FollowObject;
    public Vector3 OffSet;

    bool IsFollowing = true;
	void Start () {

        FollowObject = GameObject.Find("Player");
    }
	
    public void Setup()
    {

        FollowObject = GameObject.Find("Player");
        if (SceneManager.GetActiveScene().name == "Town")
        {
            OffSet = new Vector3(-14.0f, 20.0f, -13.0f);
            transform.position = FollowObject.transform.position + OffSet;
        }

        else if (SceneManager.GetActiveScene().name == "DungeonOne")
        {
            OffSet = new Vector3(-5.0f, 10, 5.0f);
            transform.position = FollowObject.transform.position + OffSet;
        }

        Debug.Log("Offset - X:" + OffSet.x + " Y:" + OffSet.y + " Z:" + OffSet.z);
    }

    public void StopFollowing()
    {
        IsFollowing = false;
    }
    public void StartFollowing()
    {
        IsFollowing = true;
    }
	// Update is called once per frame
	void Update ()
    {
        if(FollowObject == null)
        {
            Setup();
        }
        if (FollowObject != null && IsFollowing)
        {
            transform.position = FollowObject.transform.position + OffSet;
        }
	}

}
        //transform.position = FollowObject.transform.position + OffSet;
        //transform.position = FollowObject.transform.position;        
        
    
    //FollowObject = GameObject.Find("Player");
        //if (SceneManager.GetActiveScene().name == "Town")
        //{
        //    OffSet = transform.position - FollowObject.transform.position;
        //}
        ////transform.position = FollowObject.transform.position + OffSet;
        ////transform.position = FollowObject.transform.position;
        //else if (SceneManager.GetActiveScene().name == "DungeonOne") {
        //    OffSet = new Vector3(-4.5f, 7, 10) - FollowObject.transform.position;
        //}

        //Debug.Log("Offset - X:" + OffSet.x + " Y:" + OffSet.y + " Z:" + OffSet.z);