using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OMILGT.Interfaces;

namespace OMILGT
{
    namespace Utilities
    {
        public class TimerManager : MonoBehaviour
        {
            List<Timer> timers = new List<Timer>();
            List<Timer> removeTimers = new List<Timer>();

            static public TimerManager instance;
            // Start is called before the first frame update
            void Start()
            {
                if(instance == null)
                {
                    instance = this;
                }
                else if(instance != null)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            // Update is called once per frame
            void Update()
            {
                //Run the timers and check if they are completed
                foreach(Timer t in timers)
                {
                    if (!t.TimerCompleted())
                    {
                        //Debug.LogWarning("Timer " + t.GetName() + " is running");
                        t.RunTimer();
                    }

                    if(t.TimerCompleted())
                    {
                        t.Notify();

                        if (!t.IsTimerPersistant())
                        {
                            removeTimers.Add(t);
                        }
                    }
                }

                if (removeTimers.Count > 0.0f)
                {
                    foreach (Timer t in removeTimers)
                    {
                        timers.Remove(t);
                    }

                    removeTimers.Clear();
                }

                
            }

            public void AddTimer(string timerName, float timeToCount, IWatcher watcherObj)
            {
                Debug.LogWarning("Adding Timer");
                Timer newTimer = new Timer(timerName, timeToCount, watcherObj);
                timers.Add(newTimer);
            }

            public void RegisterToWatchTimer(string timerName, IWatcher newWatcher)
            {
                foreach(Timer t in timers)
                {
                    if(t.GetName() == timerName)
                    {
                        t.AddWatcher(newWatcher);
                    }
                }
            }
            
            

        }
    }
}
