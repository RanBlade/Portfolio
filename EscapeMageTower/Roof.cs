using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour {

   
    
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Roof Collided");
        if (collision.transform.tag == "Enemy")
        {
            Debug.Log("Crushed a Enemy");
            collision.gameObject.GetComponent<EnemyStats>().ApplyDmg(1000);
        }
        if (collision.transform.tag == "Player")
        {
            Debug.Log("Crushed the Player");
            collision.gameObject.GetComponent<PlayerController>().ApplyDmg(1000);
        }


    }
}
