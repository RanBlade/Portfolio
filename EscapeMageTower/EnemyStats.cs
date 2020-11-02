using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    public static int enenyCount;
    public int HitPoints;
    public float Speed;
    public int AttackDmg;
    public int PointsForKill;

	// Use this for initialization
	void Start () {
        enenyCount++;
	}
	
	// Update is called once per frame
	void Update () {
		if(HitPoints <= 0)
        {
            Destroy(gameObject);
            enenyCount--;
        }
	}

    public void ApplyDmg(int dmg)
    {
        HitPoints -= dmg;
    }
    public int GivePoints()
    {
        return PointsForKill;
    }
}
