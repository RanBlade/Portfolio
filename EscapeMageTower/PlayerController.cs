using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    //Character control variables
    EntityStats PlayerStats;
    Rigidbody rbCharacter;

    //UI ELEMENTS
    public Text CantTeleport;

    //private internal control variables
    [SerializeField]
    private bool bMove;
    private bool bJumped;
    private bool bLanded;
    [SerializeField]
    private bool bFacingForward = true;

    private Vector3 MoveVelocity;
    private Vector3 JumpVelocity;

    [SerializeField]
    private GameObject vFireballSpawn;
    [SerializeField]
    private GameObject FireballPrefab;
    [SerializeField]
    private GameObject PlayerWandObj;
    [SerializeField]
    private Camera MainCamera;

    private bool bTeleportFired = false;
    private bool bFireBallFired = false;

    private float fTimeOfLastTeleport = 0.0f;
    private float fTimeOfLastFireBall = 0.0f;

    private float fTeleportCoolDown = 2.0f;
    private float fFireBallCoolDown = 1.5f;
    // Use this for initialization
    void Start ()
    {
        rbCharacter = gameObject.GetComponent<Rigidbody>();
        PlayerStats = gameObject.GetComponent<EntityStats>();

        bLanded = true;
        bJumped = false;
    }
	
	// Update is called once per frame
	void Update () {

        HandleInput();


        CheckFacing();
        UpdateMoveVel();
        UpdateJumpVel();

        FireballCoolDown();
        TeleportCoolDown();
    }
    private void FixedUpdate()
    {
        rbCharacter.velocity = MoveVelocity + JumpVelocity;

    }
    void HandleInput()
    {
       
        if (Input.GetKey(KeyCode.S))
        {

            bMove = true;
            bFacingForward = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            bMove = true;
            bFacingForward = false;
        }
        if(Input.GetMouseButtonDown(1) && !bTeleportFired)
        {
            Teleport();
            
        }
        //Mouse controls
        if (Input.GetKeyDown(KeyCode.Space) && bLanded)
        {
            bJumped = true;
        }
        if (Input.GetMouseButtonDown(0) && !bFireBallFired)
        {
            CastFireball();
            
        }

        WandMovement();
        DistanceToGoal();
        
    }

    void WandMovement()
    {
        
    }
    //Character movement and controls
    void UpdateMoveVel()
    {
        if (bMove)
        {
            MoveVelocity = transform.right * PlayerStats.fSpeed;
            bMove = false;
        }
        else
        {
            MoveVelocity = transform.right * 0.0f;
        }
      
    }
    void CheckFacing()
    {
        
        if(!bFacingForward)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, -178f, transform.rotation.z), 0.5f);           
        }
        else if(bFacingForward)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, 0.0f, transform.rotation.z), 0.5f);           
        }
    }
    void UpdateJumpVel()
    {

        if (bJumped && transform.position.y < 4.0f)
        {
            JumpVelocity += transform.up * 1.2f;
            bLanded = false;

        }
        else
        {
            if (transform.position.y > 0.0f)
            {
                bJumped = false;
                JumpVelocity = transform.up * -16.0f;
            }
            else
            {
                JumpVelocity = transform.up * 0.0f;
                //bLanded = true;
            }

        }
        if(bLanded)
        {
            JumpVelocity = transform.up * 0.0f;
        }
    }
    void DistanceToGoal()
    {
       
        Vector3 goalVector = GameObject.FindGameObjectWithTag("Goal").transform.position;
        Vector3 Distance = transform.position - goalVector;
        gameObject.GetComponent<EntityStats>().fDistanceToGoal = Distance.magnitude;
    }
    //Spells and abilities
    void CastFireball()
    {
        Debug.Log("FireballSpawn transform" + vFireballSpawn.transform.position.ToString());
        GameObject Fireball = Instantiate(FireballPrefab, vFireballSpawn.transform.position, vFireballSpawn.transform.rotation);
        Fireball.GetComponent<Projectile>().Fire(Fireball.transform.right);
        StartFireballCoolDown();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            bLanded = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Goal")
        {
            Debug.Log("YOU WIN!");
            SceneManager.LoadScene(2);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            bLanded = false;
        }
    }

    public void AddToScore(int score)
    {
        gameObject.GetComponent<EntityStats>().iScore += score;
    }
    public void ApplyDmg(int dmg)
    {
        gameObject.GetComponent<EntityStats>().iHealth -= dmg;
    }

    void Teleport()
    {
        RaycastHit hit;
        Vector3 RayLoc = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        if (Physics.Raycast(RayLoc, Vector3.right, out hit, 10.0f))
        {
            Debug.Log("Canot Teleport");
            CantTeleport.text = "CANT TELEPORT";
        }
        else
        {
            transform.position = new Vector3(transform.position.x + 10.0f, transform.position.y, transform.position.z);
            CantTeleport.text = "";
            StartTeleportCoolDown();
        }
    }

    private void StartFireballCoolDown()
    {
        bFireBallFired = true;
        fTimeOfLastFireBall = Time.deltaTime + fFireBallCoolDown;

    }

    private void FireballCoolDown()
    {
        if (bFireBallFired)
        {
            fTimeOfLastFireBall -= Time.deltaTime;
            if (fTimeOfLastFireBall <= 0.0f)
            {
                bFireBallFired = false;
            }

        }
    }

    private void StartTeleportCoolDown()
    {
        bTeleportFired = true;
        fTimeOfLastTeleport = Time.deltaTime + fTeleportCoolDown;

    }

    private void TeleportCoolDown()
    {
        if (bTeleportFired)
        {
            fTimeOfLastTeleport -= Time.deltaTime;
            if (fTimeOfLastTeleport <= 0.0f)
            {
                bTeleportFired = false;
            }

        }
    }

}
