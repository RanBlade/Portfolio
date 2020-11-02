using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum AI_STATE
{
    IDLE,
    START_FOLLOW,
    FOLLOW,
    ROAM_SEARCH,
    ATTACK,
    DIE
}
public class EnemyControllerBase : MonoBehaviour {

    //AI State fields
    [SerializeField]
    AI_STATE currentState;
    

    //Agent fields
    NavMeshAgent aAIAgent;
    Animator aAIAnimator;
    Stats AIStats;

    //Targetting and pathing fields
    [SerializeField]
    GameObject currentTarget = null;
    [SerializeField]
    bool followingCurrentTarget = false;
    [SerializeField]
    Vector3 vOrigin;
    [SerializeField]
    Vector3 currentWayPoint;
    [SerializeField]
    float fMaxPatrolDistance = 30.0f;
    [SerializeField]
    bool currentlyPatrolling = false;


    //Animation fields -- may be removed and intigrated into state.
    bool isWalking = false;
    bool isAttacking = false;
    bool animationChange = false;
    bool DeathCompleted = false;
    bool GivenLoot = false;

    //Attack fields
    SphereCollider WeaponCollider;
    float TimeOfLastAttack;
    float WeaponCooldown = 1.0f;
    bool attackOnCooldown;
    public GameObject TargetedBy;

    bool alive = true;
    
    // Use this for initialization    
	void Start () {
        aAIAgent = GetComponent<NavMeshAgent>();
        aAIAnimator = GetComponent<Animator>();
        AIStats = GetComponent<Stats>();
        AIStats.LoadStats();
        SwitchState(AI_STATE.ROAM_SEARCH);

        //Set Origin to inital position
        vOrigin = transform.position;

        WeaponCollider = GetComponentInChildren<SphereCollider>();
        if(WeaponCollider != null)
        {
            Debug.Log("Found Weapon Collider");
        }
	}

    // Update is called once per frame
    void Update() {

        switch (currentState)
        {
            case AI_STATE.IDLE:
                break;
            case AI_STATE.START_FOLLOW:
                StartFollowingTarget();
                break;
            case AI_STATE.FOLLOW:
                if (checkPathComplete())
                {
                    aAIAgent.isStopped = true;
                    isWalking = false;
                    animationChange = true;
                    followingCurrentTarget = false;
                    SwitchState(AI_STATE.ROAM_SEARCH);
                }
                else
                {
                    
                }
                break;
            case AI_STATE.ROAM_SEARCH:
                if (!SearchForTarget())
                {
                    if (!currentlyPatrolling)
                    {
                        RandomPatrol();
                    }
                    else
                    {
                        currentlyPatrolling = !checkPathComplete();
                    }
                }
                break;
            case AI_STATE.ATTACK:
                if (Vector3.Distance(transform.position, currentTarget.transform.position) <= 5.0f)
                {
                    transform.LookAt(currentTarget.transform.position);
                    aAIAnimator.SetBool("Attack", true);
                    Attack();
                }
                else
                {
                    TimeOfLastAttack = 0.0f;
                    attackOnCooldown = false;
                    aAIAnimator.SetBool("Attack", false);
                    SwitchState(AI_STATE.ROAM_SEARCH);
                }
                break;
            case AI_STATE.DIE:
                if (!DeathCompleted)
                {
                    GiveLoot();

                    gameObject.GetComponent<CapsuleCollider>().enabled = false;
                    gameObject.GetComponent<Collider>().enabled = false;
                    gameObject.GetComponent<Rigidbody>().useGravity = false;
                    gameObject.GetComponent<Rigidbody>().detectCollisions = false;
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;


                    isWalking = false;
                    aAIAgent.isStopped = true;
                    aAIAgent.enabled = false;
                    aAIAnimator.Play("Death1");

                    DeathCompleted = true;
                }
                break;
            default:
                Debug.Log("No valid state set for: " + transform.name);
                break;
        }
        CheckForDeath();
        AnimationHandler();
    }

    void RandomPatrol()
    {
        if (!currentlyPatrolling)
        {
            if (CheckIfAtOrigin())
            {
               // Debug.Log("At Orgin Starting Patrol");
                Vector3 RandomDirectionVector;
                RandomDirectionVector = transform.position + Random.insideUnitSphere.normalized * fMaxPatrolDistance;
                RandomDirectionVector.y = transform.position.y;
                
                aAIAgent.destination = RandomDirectionVector;
                currentWayPoint = RandomDirectionVector;
                aAIAgent.isStopped = false;
                isWalking = true;
                animationChange = true;
                currentlyPatrolling = true;
            }
            else //if(!currentlyPatrolling)
            {
              //  Debug.Log("Not at Origin returning home");
                aAIAgent.destination = vOrigin;
                currentWayPoint = vOrigin;
                aAIAgent.isStopped = false;
                isWalking = true;
                animationChange = true;
                currentlyPatrolling = true;

            }
        }
        
        
    }
    void StartFollowingTarget()
    {
        Debug.Log("Starting to follow target");  
        aAIAgent.destination = currentTarget.transform.position;
        currentWayPoint = currentTarget.transform.position;
        aAIAgent.isStopped = false;
        isWalking = true;
        animationChange = true;
        followingCurrentTarget = true;
        SwitchState(AI_STATE.FOLLOW);
    }

    bool SearchForTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20);
        
        foreach(Collider c in hitColliders)
        {
            if(c.transform.tag == "Player" && c.gameObject.GetComponent<PlayerController>().ReportDeath())
            {
                
                Debug.Log(transform.name + " see's the player and has targeted him");                  
                currentTarget = c.gameObject;
                currentlyPatrolling = false;
                SwitchState(AI_STATE.START_FOLLOW);
                return true;
            }
        }
        //Debug.Log("No Target Found");
        followingCurrentTarget = false;
        currentTarget = null;
        return false;
    }

    bool checkPathComplete()
    {
        float DistanceSQToTarget = Vector3.SqrMagnitude(transform.position - currentWayPoint);
        if (DistanceSQToTarget <= 1.0f)
        {
           // Debug.Log("Path Completed: " + DistanceSQToTarget);
            currentWayPoint = Vector3.zero;
            return true;
        }
        else
        {
            //Debug.Log("Path is not complete. Conintinuing");
            return false;
        }
    }
    bool CheckIfAtOrigin()
    {
        float DistanceSQToOrigin = Vector3.SqrMagnitude(transform.position - vOrigin);
        //Debug.Log("DistanceSQToOrigin: " + DistanceSQToOrigin);
        if (DistanceSQToOrigin <= 0.5f)
        {
           // Debug.Log("We are at orgigin");
            return true;
        }
        else
        {
            //Debug.Log("We are not at origin");
            return false;
        }
    }
    void UpdateTarget()
    {
        followingCurrentTarget = false;
        SearchForTarget();

    }
    void AnimationHandler()
    {
        if(animationChange)
        {
            if(isWalking)
            {
                aAIAnimator.SetBool("Walking", true);
                animationChange = false;
            }
            else if(!isWalking)
            {
                aAIAnimator.SetBool("Walking", false);
                animationChange = false;
            }
            
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("COLLISION WITH: " + collision.transform.name);
        if (currentState != AI_STATE.ATTACK)
        {
            if (!aAIAgent.isStopped && (collision.transform.tag == "Player"))
            {
                aAIAgent.isStopped = true;
                aAIAnimator.SetBool("Walking", false);
                Debug.Log("Ran into: " + collision.transform.name);
                if (currentTarget != null)
                {
                    if (currentTarget.gameObject.transform.name == collision.gameObject.transform.name)
                    {
                        SwitchState(AI_STATE.ATTACK);
                    }
                }

            }
            else if (collision.transform.tag != "floor" || collision.transform.tag == "Enemy")
            {
                //Debug.Log("Restarting path");
                currentlyPatrolling = false;
            }
        }
       
    }
    void SwitchState(AI_STATE newState)
    {
        Debug.Log("Switching to state: " + newState.ToString());
        currentState = newState;
    }

    void Attack()
    {
        //Debug.Log(currentTarget.GetComponent<PlayerController>().ReportDeath());
        if (currentTarget.GetComponent<PlayerController>().ReportDeath())
        {
            if (attackOnCooldown == false)
            {
                TimeOfLastAttack = Time.deltaTime + WeaponCooldown;
                currentTarget.GetComponent<PlayerController>().ApplyIncomingDamage(AIStats.WeaponDamage);
                attackOnCooldown = true;
            }
            else
            {
                TimeOfLastAttack -= Time.deltaTime;
                if (TimeOfLastAttack < 0)
                {
                    attackOnCooldown = false;
                }
            }
        }
        else
        {
            Debug.Log("Target is DEAD");
            aAIAnimator.SetBool("Attack", false);
            currentTarget = null;
            SwitchState(AI_STATE.ROAM_SEARCH);
        }
    }

    public void ApplyIncomingDamage(int Damage)
    {
        AIStats.ApplyDamage(Damage);
    }
    void CheckForDeath()
    {
        if(AIStats.currentHitPoints <= 0)
        {
            if (currentState != AI_STATE.DIE)
            {
                SwitchState(AI_STATE.DIE);
            }
        }
    }
    public bool IsDead()
    {
        if(currentState == AI_STATE.DIE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void GiveLoot()
    {
        if (GivenLoot == false)
        {
            TargetedBy.GetComponent<PlayerController>().TakeLoot((ITEM_TYPE)Random.Range(0, 4));
            GivenLoot = true;
        }
        
    }
}
