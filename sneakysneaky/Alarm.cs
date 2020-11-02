/*
 * Author: Eric Ranaldi
 * Date: 2/20/2020
 * 
 * Purpose: This script had two uses. First it is used for a refrence object that the guard can navigate to in the Angry State
 * The second use is to run the alarm on a timer so all guards linked to this timer will be in the angry state while the alarm 
 * on.
 * 
 * Credits: The work is solely mine I am only adding this to ensure I do not get docked points for not having a credits section
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    [SerializeField]
    private GuardAgent[] alarmAgents;
    [SerializeField]
    private float alarmTimer = 0.0f;

    private bool alarmTriggered = false;
    private float currentTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(alarmTriggered)
        {
            currentTimer += Time.deltaTime;

            if(currentTimer >= alarmTimer)
            {
                alarmTriggered = false;
                currentTimer = 0;
            }
        }
    }

    public void TriggerAlarm()
    {
        if(!alarmTriggered)
        {
            alarmTriggered = true;
        }
    }

    public bool GetTriggerState()
    {
        return alarmTriggered;
    }

    public void ClearAlarmState()
    {
        alarmTriggered = false;
        currentTimer = 0;
    }
}
