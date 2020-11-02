using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_ObjectiveTracker : MonoBehaviour
{
    public bool objectiveMet = false;
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
        objectiveMet = true;
    }
}
