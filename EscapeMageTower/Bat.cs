using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour {


    public  GameObject LightingBoltPrefab;

    public GameObject BatHome;
    EnemyStats BatStats;
    Rigidbody rbBat;

    bool bMoveFromHomeNeg = true;
    bool bMoveFromHomePos;

    private float fBoltCoolDown = 2.5f;
    private float fTimeOfLastFire = 0.0f;
    private bool bBoltFired = false;

    // Use this for initialization
    void Start()
    {
        BatStats = gameObject.GetComponent<EnemyStats>();
        rbBat = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBat();
        FireBolt();
        BoltCoolDown();
    }

    void MoveBat()
    {
        Vector3 DistanceFromHome = BatHome.transform.position - gameObject.transform.position;
       

        if (DistanceFromHome.x > 10.0f)
        {
            bMoveFromHomePos = false;
            bMoveFromHomeNeg = true;
        }
        if (DistanceFromHome.x < -10.0f)
        {
            bMoveFromHomeNeg = false;
            bMoveFromHomePos = true;
        }


        if (bMoveFromHomePos)
        {
            rbBat.velocity = Vector3.right * -BatStats.Speed;
        }
        else if (bMoveFromHomeNeg)
        {
            rbBat.velocity = Vector3.right * BatStats.Speed;
        }
    }
    void FireBolt()
    {
        if (!bBoltFired)
        {
            float zRot = Random.Range(-230.0f, -340.0f);
            Vector3 offset = new Vector3(0.0f, 1.2f, 0.0f);


            GameObject lightiningBolt = Instantiate(LightingBoltPrefab, transform.position - offset, Quaternion.identity);
            lightiningBolt.transform.Rotate(0.0f, 0.0f, zRot);
            lightiningBolt.tag = "Enemy";
            lightiningBolt.GetComponent<Projectile>().tempScore = BatStats.PointsForKill;
            lightiningBolt.GetComponent<Projectile>().Fire(-lightiningBolt.transform.right);
            StartBoltCoolDown();
        }
    
    }

    private void StartBoltCoolDown()
    {
        bBoltFired = true;
        fTimeOfLastFire = Time.deltaTime + fBoltCoolDown;

    }

    private void BoltCoolDown()
    {
        if (bBoltFired)
        {
            fTimeOfLastFire -= Time.deltaTime;
            if(fTimeOfLastFire <= 0.0f)
            {
                bBoltFired = false;
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().ApplyDmg(1);
        }
    }
}
