using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBlocker : MonoBehaviour {

    public GameObject AltarGuard;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(AltarGuard.GetComponent<EnemyControllerBase>().IsDead())
        {
            gameObject.tag = "Quest";
        }
        else
        {
            gameObject.tag = "Blocked";
        }
	}
}
