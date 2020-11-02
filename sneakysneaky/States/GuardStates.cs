/*
 * Author: Eric Ranaldi
 * Date: 2/20/2020
 * 
 * Purpose: To define all the states of the Guard and the game logic in each state
 * 
 * Credits: Full Sail instructors and staff who created NavAgentStateMachine_Best. I used that script to pull in the simple states such as idle and Pause. Also used the inspritation to create a GuardAgent getter function to use in the states.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GuardStates
{
    public class GuardState : FSGDN.StateMachine.State
    {
        protected GuardAgent GuardAgentStateMachine()
        {
            return ((GuardAgent)machine);
        }
    }


    public class IdleState : GuardState
    {
        float timer = 0;

        public override void Enter()
        {
            base.Enter();
            timer = 0;
            int previousState = GuardAgentStateMachine().GetStateThatTriggeredIdle();
            if(previousState != 0)
            {
                if(previousState == 1)
                {
                    GuardAgentStateMachine().SetMainColor(new Color(0.0f, 0.5f, 0.0f));
                }
                else if(previousState == 2)
                {
                    GuardAgentStateMachine().SetMainColor(new Color(0.5f, 0.0f, 0.0f));
                }
            }
            else
            {
                Debug.LogError("Idle  state should not be called without setting triggerState");
            }

            GuardAgentStateMachine().ClearIdleTriggerState();
           
        }

        public override void Execute()
        {
            timer += Time.deltaTime;
            if (timer >= 2.0f)
            {
                if (!GuardAgentStateMachine().CheckGuardAlarmStatus())
                {
                    machine.ChangeState<PatrolState>();
                    GuardAgentStateMachine().PickNextNavPoint();
                }
                else if(GuardAgentStateMachine().CheckGuardAlarmStatus())
                {
                    machine.ChangeState<AngryState>();
                    GuardAgentStateMachine().PickNextNavPoint();
                }
            }
        }
    }

    public class PatrolState : GuardState
    {
        public override void Enter()
        {
            base.Enter();

            if (GuardAgentStateMachine().IsReversed() == true)
            {               
                GuardAgentStateMachine().SetMainColor(Color.blue);
            }
            else
            { 
                GuardAgentStateMachine().SetMainColor(Color.green);
            }
            GuardAgentStateMachine().SetCurrentTarget(null);
            GuardAgentStateMachine().StartPatrolling();
        }

        public override void Execute()
        {
            base.Execute();

            //Have the agent check for player in line of sight
            bool foundPlayer = GuardAgentStateMachine().CheckForPlayer();
            bool alarmTriggered = GuardAgentStateMachine().CheckGuardAlarmStatus();
            if (foundPlayer || alarmTriggered)
            {             
                machine.ChangeState<AngryState>();
            }
        }       
    }

    public class PursueState : GuardState
    {
        public override void Enter()
        {
            base.Enter();
            GuardAgentStateMachine().SetMainColor(Color.magenta);
        }

        public override void Execute()
        {
            base.Execute();

            bool targetInRange = GuardAgentStateMachine().CheckDistanceToTarget();
            bool invisCheck = GuardAgentStateMachine().CheckIfPlayerInvisble();
            if (targetInRange && !invisCheck)
            {
                GuardAgentStateMachine().FollowCurrentTarget();
            }
            else
            {
                machine.ChangeState<PatrolState>();
            }
        }
    }

    public class AngryState : GuardState
    {
        private bool startPursueInEnter = false;
        private bool travelingToAlarm = false;
        public override void Enter()
        {
            base.Enter();
            GuardAgentStateMachine().SetMainColor(Color.red);

            bool isAlarmTriggeredAlready = GuardAgentStateMachine().CheckGuardAlarmStatus();
            bool doesGuardHaveTarget = GuardAgentStateMachine().DoesGuardHaveTarget();

            if (!isAlarmTriggeredAlready)
            {
                GuardAgentStateMachine().GoToAlarm();
                GuardAgentStateMachine().SetTravelingToAlarm(true); //this could probably be combined into the GoToAlarm function since well it would be true if your telling the agent to go to alarm.
            }
            else if(doesGuardHaveTarget && isAlarmTriggeredAlready)
            {
                bool targetInRange = GuardAgentStateMachine().CheckDistanceToTarget();
                if (targetInRange)
                {
                    machine.ChangeState<PursueState>();
                    startPursueInEnter = true;
                }
                else
                {
                    GuardAgentStateMachine().StartPatrolling();
                }
             
            }
            else if(isAlarmTriggeredAlready)
            {
                GuardAgentStateMachine().StartPatrolling();
            }                       
        }

        public override void Execute()
        {
            base.Execute();
            if (!startPursueInEnter)
            {
                bool foundPlayer = GuardAgentStateMachine().CheckForPlayer();
                bool checkAlarm = GuardAgentStateMachine().CheckGuardAlarmStatus();
                travelingToAlarm = GuardAgentStateMachine().GetTravelingToAlarmStatus();

                if (checkAlarm)
                {
                    if (foundPlayer)
                    {
                        if (!machine.IsCurrentState<PursueState>())
                        {
                            machine.ChangeState<PursueState>();
                        }
                    }
                   
                }
                else if(!checkAlarm && !travelingToAlarm)
                {
                    Debug.LogWarning("Timer has expired. " + GuardAgentStateMachine().name + " going back to patrol");
                    machine.ChangeState<PatrolState>();
                }
            }

        }

    }

    public class PauseState : GuardState
    {
        public override void Enter()
        {
            base.Enter();
            GuardAgentStateMachine().SetMainColor(Color.grey);
        }
    }
}
