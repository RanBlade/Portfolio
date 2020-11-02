using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityStats : MonoBehaviour {

    // Use this for initialization
    public int iHealth = 0;
    public float fSpeed = 0.0f;
    public int iScore = 0;
    public float fDistanceToGoal;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(iHealth <= 0)
        {
            SceneManager.LoadScene(3);
        }
	}
}
