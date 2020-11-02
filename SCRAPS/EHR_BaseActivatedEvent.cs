using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_BaseActivatedEvent : MonoBehaviour
{
    public bool checkOnceCR = false;
    public bool checkOncePP = false;
    public bool checkOnceTunnel = false;
    public bool checkOnceAll = false;
    public bool baseActivated = false;
    public GameObject lightGreen;
    public GameObject lightRed;

    public SCRAPS_Objective ControlRoomRef;
    public SCRAPS_Objective PumpsRef;
    public SCRAPS_Objective BreakerRef;

    public SCRAPS_Objective ActivatePowerPlantRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (controlRoomOn.completed && !checkOnceCR)
        {
            lightRed.SetActive(true);
            checkOnceCR = true;
            //mainQuest.UpdateObjective(1);
        }
        if(PowerPlantOn.completed && !checkOncePP)
        {
            //mainQuest.UpdateObjective(1);
            checkOncePP = true;
        }
        //if(TunnelSealed.completed && !checkOnceTunnel)
       //{
           //mainQuest.UpdateObjective(1);
            //checkOnceTunnel = true;
       //}

        if (controlRoomOn.completed && PowerPlantOn.completed && !checkOnceAll)
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
            gameObject.GetComponent<SCRAPS_Interactive>().enabled = true;
            checkOnceAll = true;
        }*/

        if(ControlRoomRef.isComplete && PumpsRef.isComplete && BreakerRef.isComplete && !checkOnceAll)
        {
            ActivatePowerPlantRef.gameObject.SetActive(true);

            gameObject.GetComponent<BoxCollider>().enabled = true;
            gameObject.GetComponent<SCRAPS_Interactive>().enabled = true;

            //ControlRoomRef.gameObject.SetActive(false);
            //PumpsRef.gameObject.SetActive(false);
            //BreakerRef.gameObject.SetActive(false);

            checkOnceAll = true;
        }
    }

    public void EnableButton()
    {
        lightRed.SetActive(false);
        lightGreen.SetActive(true);
        ActivatePowerPlantRef.UpdateObjective(1);
        baseActivated = true;
        gameObject.GetComponent<AudioSource>().Play();
        SCRAPS_MessageSystem.instance.NewMessage("Scrapper", "I have completed the mission, I should find the exit", SCRAPS_MessageSystem.msgType.standard);
    }
}
