using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_WaterPumpsEvent : EHR_BaseEvent
{
    private int pumpsFixedCount = 0;

    public GameObject MonitorRef;
    public Material MonitorMaterial;
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
        pumpsFixedCount++;
        Debug.Log("pumps fixed count: " + pumpsFixedCount);

        if(pumpsFixedCount == 2)
        {
            MonitorRef.GetComponent<MeshRenderer>().material = MonitorMaterial;
        }
    }
}
