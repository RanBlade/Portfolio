using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantLeaveEvent : MonoBehaviour
{
    public SCRAPS_Objective objectiveRef;
    public AudioClip soundClip;
    public GameObject CutSceneRef;
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
        if(objectiveRef.isComplete)
        {
            CutSceneRef.GetComponent<BoxCollider>().isTrigger = true;
        }
        else
        {
            VoiceOverSystem.instance.VoiceOverEvent("Raynick", "You have not activated the power" +
                " plant yet! The boat will not bring you to the Bastion until you do.", soundClip);
            CutSceneRef.GetComponent<BoxCollider>().isTrigger = false;
        }
    }
}
