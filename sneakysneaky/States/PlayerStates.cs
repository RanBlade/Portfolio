/*
 * Author: Eric Ranaldi
 * Date: 2/20/2020
 * 
 *Purpose: To define all the states of the Player and the game logic in each state
 * 
 * Credits: Full Sail instructors and staff who created NavAgentStateMachine_Best. I used that script to pull in the simple states such as
 * idle state. Also used the inspritation to create a PlayerAgent getter function to use in the states.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using OMILGT.Utilities;

namespace PlayerStates
{
    public class PlayerState : FSGDN.StateMachine.State
    {
        //Refrence to player Agent script
        protected PlayerAgent GetPlayerAgentStateMachine()
        {
            return ((PlayerAgent)machine);
        }
    }

    public class IdleState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();
        }

        public override void Execute()
        {
            base.Execute();

        }
    }

    public class GameOver : PlayerState
    {
        private float resetTimerLen = 5.0f;
        private float timer = 0.0f;

        public override void Enter()
        {
            base.Enter();

            GameOverVal currentGameOverState = GetPlayerAgentStateMachine().GetGameOverFlag();

            if (currentGameOverState == GameOverVal.YOUWIN)
            {
                GetPlayerAgentStateMachine().EnableGameOverUI("YOU ESCAPED!", Color.green);
            }
            else if (currentGameOverState == GameOverVal.YOULOSE)
            {
                GetPlayerAgentStateMachine().EnableGameOverUI("YOU HAVE BEEN CAUGHT!", Color.red);
            }
        }

        public override void Execute()
        {
            base.Execute();
            
                timer += Time.deltaTime;
                if (timer >= resetTimerLen)
                {
                    SceneManager.LoadScene(0);
                }

            
        }
    }


    public class MoveState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();
        }

        public override void Execute()
        {
            base.Execute();

            Command currentCommand = GetPlayerAgentStateMachine().GetCurrentCommand();

            if (currentCommand != null)
            {
                if (currentCommand.commandName == "MoveForward")
                {
                    currentCommand.Execute(GetPlayerAgentStateMachine().gameObject);
                }
                if (currentCommand.commandName == "MoveBack")
                {
                    currentCommand.Execute(GetPlayerAgentStateMachine().gameObject);
                }
                if (currentCommand.commandName == "MoveRight")
                {
                    currentCommand.Execute(GetPlayerAgentStateMachine().gameObject);
                }
                if (currentCommand.commandName == "MoveLeft")
                {
                    currentCommand.Execute(GetPlayerAgentStateMachine().gameObject);
                }

                if(currentCommand.commandName == "Stop")
                {
                    currentCommand.Execute(GetPlayerAgentStateMachine().gameObject);
                }
            }

            GetPlayerAgentStateMachine().MovePlayer(Vector3.zero);
            //machine.ChangeState<IdleState>();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    public class MouseMoveState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();

            GetPlayerAgentStateMachine().MovePlayer(Vector3.zero);
        }

        public override void Execute()
        {
            base.Execute();

            //GetPlayerAgentStateMachine().InputHandler();

            bool reachedDestination = GetPlayerAgentStateMachine().CheckIfAtDestination();
            bool canPlayerMoveAgain = GetPlayerAgentStateMachine().GetMouseMoveAgainFlag();

            if(canPlayerMoveAgain)
            {
                GetPlayerAgentStateMachine().MovePlayer(Vector3.zero);
                GetPlayerAgentStateMachine().ResetMouseMoveFlag();
            }
            else if(reachedDestination)
            {
                machine.ChangeState<IdleState>();
                Debug.LogWarning("Player moving to idle state after mouse move");
            }

        }
    }


    public class ActivateDashState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();

            GetPlayerAgentStateMachine().ClearNavPath();
            GetPlayerAgentStateMachine().ActivateDash();
        }

        public override void Execute()
        {
            base.Execute();

            if(GetPlayerAgentStateMachine().DashPointReached())
            {
                Debug.LogWarning("Dash Completed");
                machine.ChangeState<MoveState>();
            }
        }
    }

    public class ActivateInvisState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();
            GetPlayerAgentStateMachine().ActivateInvisiblePower();
            GetPlayerAgentStateMachine().SetPlayerBodyInvis();

            //machine.ChangeState<IdleState>();
        }

        public override void Execute()
        {
            base.Execute();
        }
    }
}