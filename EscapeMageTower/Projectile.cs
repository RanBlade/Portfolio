using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //private fields for bullet ballistic data
    [Header("Projectile Ballistic Data")]
    [SerializeField]
    private float fProjectileSpeed = 20.0f;
    [SerializeField]
    private float fProjectileTravelTime = 2.0f;
    [SerializeField]
    private float fProjectileDamage = 10.0f;
    [SerializeField]
    private bool bSplashDamage;
    [SerializeField]
    private float fSplashRadius;
    public int tempScore;

    //Properties for projectile ballisitc data
    public float ProjectileSpeed
    {
        get
        {
            return fProjectileSpeed;
        }
        private set { }
    }
    public float ProjectileDamage
    {
        get
        {
            return fProjectileDamage;
        }
        private set { }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fProjectileTravelTime -= Time.deltaTime;
        if(fProjectileTravelTime <= 0.0f)
        {
            Debug.Log("Fireball reached EOL Destroying now");
            DestroyProjectile();
        }
    }

    public void Fire(Vector3 vDirectonVec)
    {
        vDirectonVec.Normalize();
        gameObject.GetComponent<Rigidbody>().velocity = (vDirectonVec * fProjectileSpeed);
    }

    private void OnDestroy()
    {
        
    }
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Enemy" && transform.tag != "Enemy")
        {
            collision.gameObject.GetComponent<EnemyStats>().ApplyDmg((int)fProjectileDamage);
            tempScore = collision.gameObject.GetComponent<EnemyStats>().PointsForKill;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddToScore(tempScore);
        }
        if(collision.transform.tag == "Player" && transform.tag == "Enemy")
        {
            collision.gameObject.GetComponent<PlayerController>().ApplyDmg((int)fProjectileDamage);
        }

        DestroyProjectile();
    }
}
