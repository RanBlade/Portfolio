using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OMILGT.Interfaces;

namespace OMILGT
{
    namespace Utilities
    {
        public class Timer : ISubject
        {
            private string myName = " ";
            private float timer = 0.0f;
            private float countDownTime = 0.0f;
            private bool persistentTimer = false;

            List<IWatcher> myWatchers = new List<IWatcher>();

            public Timer(string name, float newTimer, bool keepTimer = false)
            {
                myName = name;
                countDownTime = newTimer;
                timer = 0.0f;
                persistentTimer = keepTimer;
            }

            public Timer(string name, float newTimer, IWatcher newWatcher, bool keepTimer = false)
            {
                myName = name;
                countDownTime = newTimer;
                timer = 0.0f;
                persistentTimer = keepTimer;

                AddWatcher(newWatcher);
            }

            public bool IsTimerPersistant()
            {
                return persistentTimer;
            }

            public string GetName()
            {
                return myName;
            }
            public void RunTimer()
            {
                timer += Time.deltaTime;
            }

            public void ResetTimer()
            {
                timer = 0.0f;
            }

            public bool TimerCompleted()
            {
                if (timer >= countDownTime)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
            public List<IWatcher> GetWatcherList()
            {
                return myWatchers;
            }

            //ISubject methods
            public void AddWatcher(IWatcher watcher)
            {
                myWatchers.Add(watcher);
            }

            public void RemoveWatcher(IWatcher watcher)
            {
                myWatchers.Remove(watcher);
            }

            public void Notify()
            {
                foreach (IWatcher iw in myWatchers)
                {
                    iw.OnNotify(myName, WatcherMessages.TIMERCOMPLETE);
                }
            }



        }
    }
}
