using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject PlayerSpawn;
	// Use this for initialization
	void Start () {
		
	}
    private void Awake()
    {
        if (GameManager.instance.initalLoad == false)
        {
            Debug.Log("Setting PlayerSpawn Position");
            GameObject.Find("Player").transform.position = new Vector3(PlayerSpawn.transform.position.x, PlayerSpawn.transform.position.y, PlayerSpawn.transform.position.z);
            GameObject.Find("CameraController").gameObject.GetComponent<CameraControllerISOFollow>().StopFollowing();
            GameObject.Find("CameraController").gameObject.GetComponent<CameraControllerISOFollow>().Setup();
            GameObject.Find("CameraController").gameObject.GetComponent<CameraControllerISOFollow>().StartFollowing();
            GameObject.Find("Player").GetComponent<PlayerController>().TransitionSceneIn();

        }
        else
        {
            GameManager.instance.initalLoad = false;
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
