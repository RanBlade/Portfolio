using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_ExplosObjectiveThree : MonoBehaviour
{

    public SCRAPS_Objective explosionObjective;

    private bool doOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!doOnce)
            {
                explosionObjective.UpdateObjective(1);
                doOnce = true;

                SCRAPS_MessageSystem.instance.NewMessage("Scrapper",
                        "Found the cash!", SCRAPS_MessageSystem.msgType.emote);
            }
        }

    }
}
