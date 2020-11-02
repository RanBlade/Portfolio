using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofManager : MonoBehaviour {


    float RoofFallDelay = 1.5f;
    float StartRoofTimer = 0.0f;

    bool RoofFall = false;
    public GameObject[] RoofList;
    int CurrentRoofIndex = 0;

	// Use this for initialization
	void Start () {
        RoofList = GameObject.FindGameObjectsWithTag("Falling Roof");
        CurrentRoofIndex = RoofList.Length -1;
        
	}
	
	// Update is called once per frame
	void Update () {
		if(!RoofFall)
        {
            //Debug.Log("Starting Fall Timer");
            StartRoofCountDown();
        }
        RoofTimer();
	}
    public void AddObj(GameObject obj)
    {
        Debug.Log("Adding Roof Segement to RoofManager");
        //RoofList.Add(obj);
    }

    private void StartRoofCountDown()
    {
        RoofFall = true;
        StartRoofTimer = Time.deltaTime + RoofFallDelay;

    }

    private void RoofTimer()
    {
        if (RoofFall)
        {
            StartRoofTimer -= Time.deltaTime;
            if (StartRoofTimer <= 0.0f)
            {
                RoofFall = false;
                Vector3 FallVec = Vector3.up * -10.0f;
                RoofList[CurrentRoofIndex].GetComponent<Rigidbody>().velocity = FallVec;
                RoofList[CurrentRoofIndex].GetComponent<Rigidbody>().useGravity = true;
                CurrentRoofIndex--;
            }

        }
    }


}
