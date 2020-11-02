/*
 * Author: Eric Ranaldi
 * Date: 2/20/2020
 * 
 * Purpose: This script creates functionality for some debug tools to use during game testing. Debug functions are  bound to the 
 * Aplha number keys on the keyboard
 * 
 * Credits: Experiements would use for reference during a rapid prototype of this script. Otherwise the work is mine.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugTools
{

    public class GuardManager : MonoBehaviour
    {
        [SerializeField]
        private GuardAgent[] guardAgents = null;
        [SerializeField]
        private NavPoint[] navPoints = null;
        [SerializeField]
        GameObject GuardMangerDebugText;

        int selectedIndex = 0;

        bool showNavPoints = true;

        // Start is called before the first frame update
        void Start()
        {
            //guardAgents = FindObjectsOfType<GuardAgent>(); //disabled this becuase of the random results making it hard to see and 
            //grade
            if (guardAgents != null)
            {
                foreach (GuardAgent ga in guardAgents)
                {
                    ga.SetHighLightBool(false);
                }
                selectedIndex = 0;
                guardAgents[selectedIndex].SetHighLightBool(true);
            }

            //Find all the navpoints
            navPoints = FindObjectsOfType<NavPoint>();

            GuardMangerDebugText.SetActive(true);


        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                bool isIndexValid = IsSelectedIndexValid();
                if (isIndexValid)
                {
                    guardAgents[selectedIndex].SetHighLightBool(false);
                }

                selectedIndex++;
                if (selectedIndex >= guardAgents.Length)
                {
                    selectedIndex = 0;
                }

                isIndexValid = IsSelectedIndexValid();
                if (isIndexValid)
                {
                    guardAgents[selectedIndex].SetHighLightBool(true);
                }

                Debug.Log("AgentManager selected agent #" + selectedIndex + " " + guardAgents[selectedIndex]);

            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                bool isIndexValid = IsSelectedIndexValid();
                if (isIndexValid)
                {
                    guardAgents[selectedIndex].SetHighLightBool(false);
                }
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex = guardAgents.Length - 1;
                }

                isIndexValid = IsSelectedIndexValid();
                if (isIndexValid)
                {
                    guardAgents[selectedIndex].SetHighLightBool(true);
                }

                Debug.Log("AgentManager selected agent #" + selectedIndex + " " + guardAgents[selectedIndex]);

            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                bool isIndexValid = IsSelectedIndexValid();
                if (isIndexValid)
                {
                    guardAgents[selectedIndex].Pause();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                bool isIndexValid = IsSelectedIndexValid();
                if (isIndexValid)
                {
                    guardAgents[selectedIndex].ToggleReversed();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                bool isIndexValid = IsSelectedIndexValid();
                if (isIndexValid)
                {
                    guardAgents[selectedIndex].MakeAngry();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                showNavPoints = !showNavPoints;
                if (navPoints != null)
                {
                    if (!showNavPoints)
                    {
                        foreach (NavPoint np in navPoints)
                        {
                            np.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    else
                    {
                        foreach (NavPoint np in navPoints)
                        {
                            np.GetComponent<Renderer>().enabled = true;
                        }
                    }
                }
            }

        }

        private bool IsSelectedIndexValid()
        {
            if (selectedIndex >= 0 && selectedIndex < navPoints.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
