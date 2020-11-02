using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_ExplosObjectiveOne : MonoBehaviour
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



    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!doOnce)
            {
                if (SCRAPS_Inventory.instance.GetKeyItemAmount("Fuse") > 0 &&
                    SCRAPS_Inventory.instance.GetKeyItemAmount("TNT") > 0 &&
                    SCRAPS_Inventory.instance.GetKeyItemAmount("Timer") > 0)
                {
                    explosionObjective.UpdateObjective(1);
                    doOnce = true;
                    SCRAPS_MessageSystem.instance.NewMessage("Scrapper",
                        "Got the bomb parts!", SCRAPS_MessageSystem.msgType.emote);
                }
            }
        }
    }
}
