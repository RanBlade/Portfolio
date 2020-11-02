using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text EnemyCounter;
    public Text PlayerScore;
    public Text DistanceToGoal;
    public GameObject Player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        EnemyCounter.text = "Enemies Left: " + EnemyStats.enenyCount;
        PlayerScore.text = "SCORE: " + Player.GetComponent<EntityStats>().iScore;
        DistanceToGoal.text = "Distance to goal: " + Player.GetComponent<EntityStats>().fDistanceToGoal.ToString("F2") + "m";
	}
}
