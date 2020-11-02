/*
 * Author: Eric Ranaldi
 * Date: 2/20/2020
 * 
 * Purpose: This is the behaviour script for the Guard State machine system. This class defines all the behaviour and information
 * relevant to a guard in the game. The state classes and the state system use this class to implement game logic
 * 
 * Credits: Full Sail instructors for Game Development: Used some structures as a template for things I did and some of the simple classes and states were used from the NavAgentStateMachine_Best class (namely pause and idle)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Guard state namespace
using GuardStates;

public class GuardAgent : FSGDN.StateMachine.MachineBehaviour
{
    //Class vairables. Should all be private and set to default value
    //##################################################################
    //refrences to objects the guard needs to know about in the world in order to perform as the guard should
    [Header("World Refrence Objects")]
    [SerializeField]
    private NavPoint[] navPoints = null;
    [SerializeField]
    private Alarm myAlarm = null;

    //Refrences and world object/positon variables pertaining to the guard
    [Header("Guard Refrence Objects")]
    [SerializeField]
    private GameObject guardBodyRef = null;
    [SerializeField]
    private GameObject highLightObjectRef = null;
    private Transform guardTransform = null;


    //detection variables / guard sight
    private float sightDistance = 15.0f;
    private GameObject currentTarget = null;

    //Patrolling variables 
    private int navPointIndex = 0;
    private bool isReversed = false;

    //angry variables
    private bool travelingToAlarm = false;

    //pause state vairables
    private bool paused = false;
    private FSGDN.StateMachine.State lastState = null;
    int stateThatTriggeredIdle = 0;


    //DEBUG MANAGER VARIABLES - these should only be used(refrecned/changed) by the GuardManager
    private bool isMadeAngry = false;
    //##################################################################



    //================================================
    //Monobehaviour classes 
    //================================================
    public override void Start()
    {
        base.Start();

        guardTransform = gameObject.transform;


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NavPoint>() && !(IsCurrentState<PursueState>() || IsCurrentState<PauseState>()) && 
            !travelingToAlarm)
        {
            if (IsCurrentState<PatrolState>())
            {
                stateThatTriggeredIdle = 1;
                ChangeState<IdleState>();
            }
            else if (IsCurrentState<AngryState>())
            {
                stateThatTriggeredIdle = 2;
                ChangeState<IdleState>();
            }
            
        }
        else if (other.gameObject.GetComponent<Alarm>())
        {
            if (IsCurrentState<AngryState>())
            {
                myAlarm.TriggerAlarm();

                travelingToAlarm = false;

                bool haveCurrentTarget = DoesGuardHaveTarget();
                if (haveCurrentTarget && CheckDistanceToTarget())
                {
                    ChangeState<PursueState>();
                }
                else
                {
                    ChangeState<AngryState>();
                }
            }
        }


        if (other.gameObject.GetComponent<RoomGate>())
        {
            myAlarm.ClearAlarmState();
            ChangeState<PatrolState>();
        }
    }

    public override void Update()
    {
        base.Update();

    }
    //================================================

    //================================================
    //MachineBehaviour override functions
    //================================================
    public override void AddStates()
    {
        AddState<PatrolState>();
        AddState<IdleState>();
        AddState<PursueState>();
        AddState<AngryState>();
        AddState<PauseState>();

        SetInitialState<PatrolState>();
    }
    //================================================

    //================================================
    //Guard Agent functions and behaviours
    //================================================

    ///---------------------------------------------------
    /// Navigation functions for nav points
    ///---------------------------------------------------
    public void PickNextNavPoint()
    {
        if (isReversed)
        {

            navPointIndex--;
            if (navPointIndex < 0)
            {
                navPointIndex = navPoints.Length - 1;
            }

        }
        else
        {
            navPointIndex++;
            if (navPointIndex >= navPoints.Length)
            {
                navPointIndex = 0;
            }

        }

    }

    public void StartPatrolling()
    {
        bool isIndexValid = IsNavPointIndexValid();
        if (isIndexValid)
        {
            gameObject.GetComponent<NavMeshAgent>().SetDestination(navPoints[navPointIndex].transform.position);
        }
        else
        {
            Debug.LogError("Tried to access invalid index in NavPoints array : StartPatrolling()");
        }
    }

    public void GoToAlarm()
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(myAlarm.transform.position);
    }
    /// ---------------------------------------------------

    /// ---------------------------------------------------
    /// Detection functions 
    /// ---------------------------------------------------
    public bool CheckForPlayer()
    {
        //Can't import a laymask with a unity package so we are going to have to do it the slower way with all collsiions.. 
        Collider[] hitColliders = Physics.OverlapSphere(guardTransform.position, sightDistance);//, LayerMask.GetMask("PlayerMask"));

        //this should never be more then 1 element with the PlayerMask BUT just to be safe we will iterate the list.
        foreach (Collider c in hitColliders)
        {
            if (c.name == "Player")
            {
                //Debug.LogWarning(name + ": I see a player");
                bool invisblePlayerCheck = c.gameObject.GetComponent<PlayerAgent>().CheckIfPlayerInvisible();
                if(invisblePlayerCheck)
                {
                    //Debug.LogWarning("Nevermind Player is invisble");
                    return false;
                }
                //we want the normalized vector to use with the dot product to get our angle value.
                Vector3 toPlayer = (c.transform.position - guardTransform.position);

                float dot = Vector3.Dot(guardTransform.forward, toPlayer.normalized);

                // we are calling this a lot so we use the fast method. Have to remember to squard all the distances compared to this
                float distanceToPlayer = Vector3.SqrMagnitude(toPlayer);

                //the side detection should be 50% normal distance. periprial vision and all
                float sightDistanceSide = (sightDistance * 0.5f);

                //This is like hearing and should be 25% of the normal distance
                float sightDistanceBack = (sightDistance * 0.25f);

                RaycastHit playerHit;
                Vector3 origin = GetGuardTransformPos();

                //dot check if player is in front LOS between 45~ degrees
                //.707 should be about a 45 degree angle and 1 is directly in front
                if ((dot >= 0.707f && dot <= 1.0f) && (distanceToPlayer <= sightDistance * sightDistance))
                {
                    //Lets do a raycast to see if there is an object blocking the LOS
                    if (Physics.Raycast(origin, toPlayer, out playerHit, sightDistance))
                    {
                        if (playerHit.collider.name == "Player") //if its not the player we can't see the player
                        {
                            SetCurrentTarget(playerHit.collider.gameObject);
                            return true;
                        }
                    }
                }
                //check dot to see if the player is to the side of the guard... with a reduced distance
                //-.707 is about a 45 degree angle behind guard and .707 is about a 45 degree angle in front of guard
                //this checks the space between that span
                else if ((dot >= 0.0f && dot <= 0.707f) && (distanceToPlayer <= (sightDistanceSide * sightDistanceSide)))
                {
                    //Lets do a raycast to see if there is an object blocking the LOS
                    if (Physics.Raycast(origin, toPlayer, out playerHit, sightDistanceSide))
                    {
                        if (playerHit.collider.name == "Player") //if its not the player we can't see the player
                        {
                            SetCurrentTarget(playerHit.collider.gameObject);
                            return true;
                        }
                    }
                }
                //Check to see if the player is behind the guard... with an even greater reduced distance
                //-1 is directly behind the player and 0 is directly beside the player.
                //this checks all the space in that area
                else if ((dot >= -1.0f && dot <= 0.0f) && (distanceToPlayer <= (sightDistanceBack * sightDistanceBack)))
                {
                    //Lets do a raycast to see if there is an object blocking the LOS
                    if (Physics.Raycast(origin, toPlayer, out playerHit, sightDistanceBack))
                    {
                        if (playerHit.collider.name == "Player") //if its not the player we can't see the player
                        {
                            SetCurrentTarget(playerHit.collider.gameObject);
                            return true;
                        }
                    }
                }
            }           
        }

        return false; //if we got here we did not hit the player and can return a false
    }
    public bool CheckDistanceToTarget()
    {
        if (currentTarget == null) // no need to do the math if current target is null
        {
            return false;
        }

        float distanceToTarget = Vector3.SqrMagnitude(currentTarget.transform.position - transform.position);

        if (distanceToTarget <= (sightDistance * sightDistance))
        {
            return true;
        }

        return false; //if we have gotten here that means the player is not within range to keep following. 
    }

    public bool CheckIfPlayerInvisble()
    {
        if (currentTarget == null) // no need to do the math if current target is null
        {
            return false;
        }
        else
        {
            bool invisCheck = currentTarget.GetComponent<PlayerAgent>().CheckIfPlayerInvisible();
            if (invisCheck)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       
    }
    //-----------------------------------------------------------

    //-----------------------------------------------------------
    //Pursue functions
    //-----------------------------------------------------------
    public void FollowCurrentTarget()
    {
        if (currentTarget)
        {
            gameObject.GetComponent<NavMeshAgent>().SetDestination(currentTarget.transform.position);
        }

    }
    //-----------------------------------------------------------

    //-----------------------------------------------------------
    //Pause functions
    //-----------------------------------------------------------
    public void Pause()
    {
        // toggle paused value
        paused = !paused;

        if (paused)
        {
            // store current state for use when unpausing
            lastState = currentState;

            // change state to Pause
            ChangeState<PauseState>();
            GetComponent<NavMeshAgent>().isStopped = true;
        }
        else
        {
            // restore stored state when pausing earlier
            ChangeState(lastState.GetType());
            GetComponent<NavMeshAgent>().isStopped = false;
        }
    }
    //-----------------------------------------------------------


    //================================================
    //Helper/Getter/Setter functions
    //================================================
    public void SetMainColor(Color color)
    {
        guardBodyRef.GetComponent<Renderer>().material.color = color;
    }

    public void SetHighLightColor(Color color)
    {
        highLightObjectRef.GetComponent<Renderer>().material.color = color;
    }

    public void SetHighLightBool(bool highlight)
    {
        if (highlight)
        {
            highLightObjectRef.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            highLightObjectRef.GetComponent<Renderer>().material.color = Color.grey;
        }
    }
    public GameObject GetCurrentTarget()
    {
        if (currentTarget)
        {
            return currentTarget;
        }
        else
        {
            Debug.LogError("currentTarget is null : GetCurrentTarget(). Trying using DoesGuardHaveTarget before GetCurrentTarget");
            return null;
        }
    }

    public void SetCurrentTarget(GameObject go)
    {
        currentTarget = go;
    }

    public bool DoesGuardHaveTarget()
    {
        if (currentTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Transform GetGuardTransform()
    {
        if (guardTransform)
        {
            return guardTransform;
        }
        else
        {
            Debug.LogError("guardTansform not set : GetGuardTransform()");
            return null;
        }
    }

    public Vector3 GetRight45RayVec()
    {
        if (guardTransform == null)
        {
            Debug.LogError("guardTransform is not set : GetRight45RayVec()");
            return Vector3.zero;
        }
        else
        {
            return (guardTransform.forward + guardTransform.right).normalized; //just some simple vector math to get a 45 angle to check with the ray
        }
    }

    public Vector3 GetLeft45RayVec()
    {
        if (guardTransform == null)
        {
            Debug.LogError("guardTransform is not set : GetLeft45RayVec()");
            return Vector3.zero;
        }
        else
        {
            return (guardTransform.forward - guardTransform.right).normalized; //just some simple vector math to get a 45 angle to check with the ray
        }
    }

    public Vector3 GetForwardRayVec()
    {
        if (guardTransform == null)
        {
            Debug.LogError("guardTransform is not set : GetForwardRayVec()");
            return Vector3.zero;
        }
        else
        {
            return guardTransform.transform.forward;
        }
    }

    public Vector3 GetRightRayVec()
    {
        if (guardTransform == null)
        {
            Debug.LogError("guardTransform is not set : GetRightRayVec()");
            return Vector3.zero;
        }
        else
        {
            return guardTransform.transform.right;
        }
    }

    public Vector3 GetGuardTransformPos()
    {
        if (guardTransform == null)
        {
            Debug.LogError("guardTransform is not set : GetGuardTransformPos()");
            return Vector3.zero;
        }
        else
        {
            return guardTransform.position;
        }
    }

    public float GetSightDistance()
    {
        return sightDistance;
    }

    public bool CheckGuardAlarmStatus()
    {
        if (myAlarm == null)
        {
            Debug.LogError("MyAlarm is not set : CheckGuardAlarmStatus()");
            return false;
        }
        else
        {
            return myAlarm.GetTriggerState();
        }
    }

    public void SetTravelingToAlarm(bool status)
    {
        travelingToAlarm = status;
    }

    public bool GetTravelingToAlarmStatus()
    {
        return travelingToAlarm;
    }

    public bool CheckIfNavPointReached() //not used but will keep incase need arises in the future. probably a better way to implement this
    {
        bool isIndexValid = IsNavPointIndexValid();
        if (isIndexValid)
        {
            float distanceToNavPoint = Vector3.SqrMagnitude(navPoints[navPointIndex].gameObject.transform.position - guardTransform.position);

            if (distanceToNavPoint <= 1.0f * 1.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.LogError("Tried to access invalid index in NavPoints Array : CheckIfNavPointReached()");
            return false;
        }
    }

    public bool IsReversed()
    {
        return isReversed;
    }

    //debug and guard manager helper fucntions
    public void ToggleReversed()
    {
        if (IsCurrentState<PatrolState>())
        {
            isReversed = !isReversed;
            if (isReversed)
            {
                SetMainColor(Color.blue);
                PickNextNavPoint();
                StartPatrolling();
            }
            else if (!isReversed)
            {
                SetMainColor(Color.green);
                PickNextNavPoint();
                StartPatrolling();
            }
        }
        else if (IsCurrentState<AngryState>())
        {
            isReversed = !isReversed;
            if (isReversed)
            {
                SetMainColor(new Color(0.9f, 0.1f, 0.1f));
                PickNextNavPoint();
                StartPatrolling();
            }
            else if (!isReversed)
            {
                SetMainColor(Color.red);
                PickNextNavPoint();
                StartPatrolling();
            }
        }
    }

    public void MakeAngry()
    {
        isMadeAngry = !isMadeAngry;

        if (!IsCurrentState<AngryState>() && isMadeAngry)
        {
            ChangeState<AngryState>();
        }
        else
        {
            myAlarm.ClearAlarmState();
            ChangeState<PatrolState>();
        }
    }

    public int GetStateThatTriggeredIdle()
    {
        if (stateThatTriggeredIdle != 0)
        {
            return stateThatTriggeredIdle;
        }
        else
        {
            Debug.LogError("StateThatTriggeredIdle is null : GetStateThatTriggeredIdle()");
            return 0;
        }
    }

    public void ClearIdleTriggerState()
    {
        stateThatTriggeredIdle = 0;
    }

    private bool IsNavPointIndexValid()
    {
        if (navPointIndex >= 0 && navPointIndex < navPoints.Length)
        {
            return true;
        }
        else
        {
            Debug.LogError("NavPointIndex: " + navPointIndex + " is out of bounds of NavPoints array");
            return false;
        }
    }
    //================================================
}

//OLD CODE KEPT AROUND TO SEE IMPROVEMENTS AND TO BE ABLE TO GO BACK TO IN CASE SOMETHING ISN"T WORKING RIGHT 
/*
 * ----This was checkPlayer fucntion
        //The raycast hit info..Using one here might work but I don't wnat a wall collision to block the player from being detected
        RaycastHit playerHit;
        //RaycastHit playerHitLeft;
        //RaycastHit playerHitRight; //This caused a bug and I wnat to research why so not deleting this code right now

        Vector3 origin = GetGuardTransformPos();
        Vector3 direction = GetForwardRayVec();

        if (Physics.Raycast(origin, direction, out playerHit, sightDistance))
        {
            Vector3 debugDir = GetForwardRayVec() * playerHit.distance;
            Debug.DrawRay(origin, debugDir, Color.red);

            if (playerHit.collider.gameObject.name == "Player")
            {
                SetCurrentTarget(playerHit.collider.gameObject);
                return true;
            }
        }

        direction = GetRight45RayVec();

        if (Physics.Raycast(origin, direction, out playerHit, sightDistance))
        {
            Vector3 debugDir = GetRight45RayVec() * playerHit.distance;
            Debug.DrawRay(origin, debugDir, Color.yellow);

            if (playerHit.collider.gameObject.name == "Player")
            {
                SetCurrentTarget(playerHit.collider.gameObject);
                return true;
            }
        }

        direction = GetLeft45RayVec();

        if (Physics.Raycast(origin, direction, out playerHit, sightDistance))
        {
            Vector3 debugDir = GetLeft45RayVec() * playerHit.distance;
            Debug.DrawRay(origin, debugDir, Color.green);

            if (playerHit.collider.gameObject.name == "Player")
            {
                SetCurrentTarget(playerHit.collider.gameObject);
                return true;
            }
        }

        //side facing Rays with a shorter distance then front but longer distance then abck
        float sideSightDistance = sightDistance / 2;
        direction = -GetRightRayVec();

        if (Physics.Raycast(origin, direction, out playerHit, sideSightDistance))
        {
            Vector3 debugDir = -GetRightRayVec() * playerHit.distance;
            Debug.DrawRay(origin, debugDir, Color.yellow);

            if (playerHit.collider.gameObject.name == "Player")
            {
                SetCurrentTarget(playerHit.collider.gameObject);
                return true;
            }
        }

        direction = GetRightRayVec();

        if (Physics.Raycast(origin, direction, out playerHit, sideSightDistance))
        {
            Vector3 debugDir = GetRightRayVec() * playerHit.distance;
            Debug.DrawRay(origin, debugDir, Color.yellow);

            if (playerHit.collider.gameObject.name == "Player")
            {
                SetCurrentTarget(playerHit.collider.gameObject);
                return true;
            }
        }

        //Back facing detection rays
        //We want this to be like a "sound" detection and have a lot smaller range so we will divide sight distance by a constnat
        float backSightDistance = sightDistance / 5;
        direction = -GetForwardRayVec();

        if (Physics.Raycast(origin, direction, out playerHit, backSightDistance))
        {
            Vector3 debugDir = -GetForwardRayVec() * playerHit.distance;
            Debug.DrawRay(origin, debugDir, Color.red);

            if (playerHit.collider.gameObject.name == "Player")
            {
                SetCurrentTarget(playerHit.collider.gameObject);
                return true;
            }
        }

        direction = -GetRight45RayVec();

        if (Physics.Raycast(origin, direction, out playerHit, backSightDistance))
        {
            Vector3 debugDir = -GetRight45RayVec() * playerHit.distance;
            Debug.DrawRay(origin, debugDir, Color.magenta);

            if (playerHit.collider.gameObject.name == "Player")
            {
                SetCurrentTarget(playerHit.collider.gameObject);
                return true;
            }
        }

        direction = -GetLeft45RayVec();

        if (Physics.Raycast(origin, direction, out playerHit, backSightDistance))
        {
            Vector3 debugDir = -GetLeft45RayVec() * playerHit.distance;
            Debug.DrawRay(origin, debugDir, Color.magenta);

            if (playerHit.collider.gameObject.name == "Player")
            {
                SetCurrentTarget(playerHit.collider.gameObject);
                return true;
            }
        }*/
