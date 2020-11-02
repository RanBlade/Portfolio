using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_BreakerRoom : EHR_BaseEvent
{
    int wireCount = 3;
    int wiresConnected = 0;

    public Material monitorFixed;
    public GameObject monitorRef;
    public SCRAPS_Objective objectiveRef;

    public EHR_BaseEvent LightsOnEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void RunEvent()
    {
        wiresConnected++;
        Debug.Log("Wires connected: " + wiresConnected);
        //objectiveRef.UpdateObjective(1);
        if(wiresConnected == wireCount)
        {
            LightsOnEvent.RunEvent();
            monitorRef.GetComponent<MeshRenderer>().material = monitorFixed;
        }
        completed = true;
    }
}
