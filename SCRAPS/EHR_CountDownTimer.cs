using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_CountDownTimer : MonoBehaviour
{
    private float timerStart;
    private float currentTime;
    private float timerLength;

    private bool timerStarted = false;
    private bool timerReached = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timerStarted)
        {
            timerLength -= 1 * Time.deltaTime;
            if(timerLength <= 0.0f)
            {
                timerReached = true;
                //timerStarted = false;
            }

        }
    }

    public void StartTimer(float length)
    {
        timerLength = length;
        timerStarted = true;

    }

    public bool TimerReached()
    {
        if(timerStarted && timerReached)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
