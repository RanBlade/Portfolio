/*
 * Author: Eric Ranaldi
 * Date: 2/20/2020
 * 
 * Purpose: This was setup to enable the player to be in the state system. This state is mixed with 
 * what would normally be in a game manager. 
 * 
 * Credits: The work is solely mine I am only adding this to ensure I do not get docked points for not having a credits section
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//Player State namespaces
using PlayerStates;
using OMILGT.Utilities;
using OMILGT.Interfaces;

public enum GameOverVal
{
    PLAYING,
    YOUWIN,
    YOULOSE
}

public enum PlayerControls
{
    MOUSEMOVE,
    WASDMOVE
}

public class PlayerAgent : FSGDN.StateMachine.MachineBehaviour, IWatcher 
{
    //GameOver state variables
    private GameOverVal gameOver = GameOverVal.PLAYING;
    [Header("UI Refrences")]
    [SerializeField]
    Text gameOverText = null;
    [SerializeField]
    Text gameOverRestText = null;

    //movement state variables

    [Header("Player Control System")]
    [SerializeField]
    private PlayerControls currentControls = PlayerControls.WASDMOVE;
    [SerializeField]
    private float playerMoveSpeed = 10.0f;
    private bool mouseMoveAgain = false;
    private Vector3 MovementInputVec = Vector3.zero;

    [Header("Debug player visual refrences")]
    [SerializeField]
    private GameObject playerBodyRef = null;
    [SerializeField]
    private Material mainBodyMat = null;
    [SerializeField]
    private Material invisBodyMat = null;

    //Player power variables    
    private Vector3 initalPos = Vector3.zero;
    private bool isPlayerInvisible = false;
    private string invisTimerName = "InvisRunTimer";
    private string invisCoolDownTimer = "InvisCoolDownTimer";
    private string dashCooldDownTimer = "DashCoolDownTimer";
    private bool isInviOnCoolDown = false;
    private bool isDashOnCoolDown = false;
    [Header("Player Power settings")]
    [SerializeField]
    float dashSpeed = 20.0f;
    [SerializeField]
    float dashDistance = 10.0f;
    [SerializeField]
    private float dashCoolDownTimerLength = 12.0f;
    [SerializeField]
    private float timeInvisible = 5.0f;
    [SerializeField]
    private float invisCoolDownTimerLength = 10.0f;
  
    [SerializeField]
    Text dashCDText = null;
    [SerializeField]
    Text invisCDText = null;

    //Input command reference
    private Command currentCommand = null;

    public override void Start()
    {
        base.Start();

        dashCDText.text = "Available";
        invisCDText.text = "Available";
    }

    //Monobehaviour functions that have been overriden by MachineBehaviour
    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        InputHandler();

    }

    //MachineBehaviour functions
    public override void AddStates()
    {       
        AddState<IdleState>();
        AddState<GameOver>();
        AddState<ActivateDashState>();
        AddState<ActivateInvisState>();
        AddState<MouseMoveState>();
        AddState<MoveState>();
    
        SetInitialState<IdleState>();
        
    }


    //Functions to be used by the States within the PlayerAgent

    //Functions used by the MoveState Agent State
    public void MovePlayer(Vector3 dir)
    {
        if (currentControls == PlayerControls.MOUSEMOVE)
        {
            RaycastHit mouseHit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mouseHit, 100))
            {
                gameObject.GetComponent<NavMeshAgent>().SetDestination(mouseHit.point);
            }
        }
        else if(currentControls == PlayerControls.WASDMOVE)
        {
            gameObject.GetComponent<Rigidbody>().velocity = (MovementInputVec * playerMoveSpeed);

            if (MovementInputVec != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(((transform.position + MovementInputVec) - transform.position), Vector3.up);
            }
        }
    }

    public bool CheckIfAtDestination()
    {
        float distanceToTarget = Vector3.SqrMagnitude((gameObject.GetComponent<NavMeshAgent>().destination - transform.position));
        if(gameObject.GetComponent<NavMeshAgent>().pathStatus == NavMeshPathStatus.PathComplete && distanceToTarget <= (1.0f * 1.0f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "ExitDoor")
        {
            gameOver = GameOverVal.YOUWIN;
            ChangeState<GameOver>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<GuardAgent>() && !IsCurrentState<GameOver>())
        {
            gameOver = GameOverVal.YOULOSE;
            ChangeState<GameOver>();
        }

        if(IsCurrentState<ActivateDashState>())
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ChangeState<IdleState>();
        }
    }

    //Powers
    public void ActivateDash()
    {                     
        
        initalPos = transform.position;       

        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * dashSpeed;

        isDashOnCoolDown = true;
        TimerManager.instance.AddTimer(dashCooldDownTimer, dashCoolDownTimerLength, this);

        dashCDText.text = "On Cooldown";
        dashCDText.color = Color.red;

    }

    public bool DashPointReached()
    {
        float distancetoDashPoint = Vector3.SqrMagnitude(initalPos - transform.position);

        if (distancetoDashPoint >= (dashDistance * dashDistance))
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;           
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ActivateInvisiblePower()
    {
        isPlayerInvisible = true;
        isInviOnCoolDown = true;
        TimerManager.instance.AddTimer(invisTimerName, timeInvisible, this);
        TimerManager.instance.AddTimer(invisCoolDownTimer, invisCoolDownTimerLength, this);

        invisCDText.text = "On Cooldown";
        invisCDText.color = Color.red;
    }

    public void SetPlayerBodyInvis()
    {
        if (!isPlayerInvisible)
        {
            playerBodyRef.GetComponent<Renderer>().material = mainBodyMat;
        }
        else
        {
            playerBodyRef.GetComponent<Renderer>().material = invisBodyMat;
        }
    }

    public bool CheckIfPlayerInvisible()
    {
        return isPlayerInvisible;
    }

    public GameOverVal GetGameOverFlag()
    {
        return gameOver;
    }

    public void EnableGameOverUI(string message, Color color)
    {
        gameOverText.text = message;
        gameOverText.color = color;
        gameOverText.gameObject.SetActive(true);
        gameOverRestText.gameObject.SetActive(true);
    }

    public void ClearNavPath()
    {
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        gameObject.GetComponent<NavMeshAgent>().ResetPath();
    }

    public void EnableNavAgent()
    {
        gameObject.GetComponent<NavMeshAgent>().isStopped = false;
    }    

    public void InputHandler()
    {
        currentCommand = null;
        if (gameOver == GameOverVal.PLAYING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsCurrentState<IdleState>())
                {
                    if (currentControls == PlayerControls.MOUSEMOVE)
                    {

                        ChangeState<MouseMoveState>();
                    }
                }
                else if (IsCurrentState<MouseMoveState>())
                {
                    mouseMoveAgain = true;
                }
            }
            if (Input.GetKey(KeyCode.W))
            {
                if(!IsCurrentState<MoveState>() && !IsCurrentState<ActivateDashState>())
                {
                    ChangeState<MoveState>();
                }
                currentCommand = new MoveForwardCommandP();
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (!IsCurrentState<MoveState>() && !IsCurrentState<ActivateDashState>())
                {
                    ChangeState<MoveState>();
                }

                currentCommand = new MoveBackwardCommandP();
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (!IsCurrentState<MoveState>() && !IsCurrentState<ActivateDashState>())
                {
                    ChangeState<MoveState>();
                }

                currentCommand = new MoveLeftCommandP();
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!IsCurrentState<MoveState>() && !IsCurrentState<ActivateDashState>())
                {
                    ChangeState<MoveState>();
                }

                currentCommand = new MoveRightCommandP();
            }

            //Powers
            if (Input.GetKeyDown(KeyCode.Q) && !isDashOnCoolDown)
            {
                if(!IsCurrentState<ActivateDashState>())
                {
                    ChangeState<ActivateDashState>();
                }
                ChangeState<ActivateDashState>();
            }
            if(Input.GetKeyDown(KeyCode.E) && !isInviOnCoolDown)
            {
                if(!IsCurrentState<ActivateInvisState>() && !isPlayerInvisible)
                {
                    ChangeState<ActivateInvisState>();
                }

            }
            
            //keyUp commands
            if (Input.GetKeyUp(KeyCode.W))
            {
                if (!IsCurrentState<MoveState>() && !IsCurrentState<ActivateDashState>())
                {
                    ChangeState<MoveState>();
                }
                currentCommand = new StopMoveCommandP();

            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                if (!IsCurrentState<MoveState>() && !IsCurrentState<ActivateDashState>())
                {
                    ChangeState<MoveState>();
                }
                currentCommand = new StopMoveCommandP();

            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                if (!IsCurrentState<MoveState>() && !IsCurrentState<ActivateDashState>())
                {
                    ChangeState<MoveState>();
                }
                currentCommand = new StopMoveCommandP();

            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                if (!IsCurrentState<MoveState>() && !IsCurrentState<ActivateDashState>())
                {
                    ChangeState<MoveState>();
                }
                currentCommand = new StopMoveCommandP();

            }
        }
    }

    public void ResetMouseMoveFlag()
    {
        mouseMoveAgain = false;
    }

    public bool GetMouseMoveAgainFlag()
    {
        return mouseMoveAgain;
    }
    

    public void AddMovementInput(Vector3 moveInput)
    {
        MovementInputVec = moveInput;
    }

    public Command GetCurrentCommand()
    {
        return currentCommand;
    }

    public void OnNotify(string subjectName, WatcherMessages message)
    {
        if(subjectName == invisTimerName)
        {
            if(message == WatcherMessages.TIMERCOMPLETE)
            {
                Debug.LogWarning("Setting Player to visible");
                isPlayerInvisible = false;

                SetPlayerBodyInvis();

            }
        }
        else if(subjectName == invisCoolDownTimer)
        {
            if(message == WatcherMessages.TIMERCOMPLETE)
            {
                isInviOnCoolDown = false;

                invisCDText.text = "Available";
                invisCDText.color = Color.white;
            }
        }
        else if(subjectName == dashCooldDownTimer)
        {
            if(message == WatcherMessages.TIMERCOMPLETE)
            {
                isDashOnCoolDown = false;

                dashCDText.text = "Available";
                dashCDText.color = Color.white;
            }
        }
    }
}
