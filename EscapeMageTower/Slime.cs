using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour {

    public GameObject SlimeHome;
    EnemyStats SlimeStats;
    Rigidbody rbSlime;

    bool bMoveFromHomeNeg = true;
    bool bMoveFromHomePos;
	// Use this for initialization
	void Start () {
        SlimeStats = gameObject.GetComponent<EnemyStats>();
        rbSlime = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        MoveSlime();
	}

    void MoveSlime()
    {
        Vector3 DistanceFromHome = SlimeHome.transform.position - gameObject.transform.position;
        //Debug.Log("Slime Move Info: " + DistanceFromHome.x + " MoveFromHomePos: " + bMoveFromHomePos + " bMoveFromHomeNeg: " + bMoveFromHomeNeg);

        if(DistanceFromHome.x > 5.0f)
        {
            bMoveFromHomePos = false;
            bMoveFromHomeNeg = true;
        }
        if (DistanceFromHome.x < -5.0f)
        {
            bMoveFromHomeNeg = false;
            bMoveFromHomePos = true;
        }
       
        
        if(bMoveFromHomePos)
        {
            rbSlime.velocity = Vector3.right * -SlimeStats.Speed;
        }
        else if(bMoveFromHomeNeg)
        {
            rbSlime.velocity = Vector3.right * SlimeStats.Speed;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().ApplyDmg(1);
        }
    }

}
