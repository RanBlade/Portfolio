using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_TriggerCheck : MonoBehaviour
{
    private bool doOnce = false;
    public bool areaTriggered = false;

    public GameObject objectRef;
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
        if(!doOnce && other.tag == "Player")
        {
            doOnce = true;
            areaTriggered = true;
            if(objectRef != null)
            {
                objectRef.SetActive(true);
            }
            
        }
    }
}
