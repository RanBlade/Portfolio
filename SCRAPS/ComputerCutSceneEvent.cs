using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerCutSceneEvent : EHR_BaseEvent
{
    public GameObject CutScene;
    public AudioClip soundClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    override public void RunEvent()
    {
        CutScene.GetComponent<StartCutSceneScript>().StartCutSceneInteraction();
        VoiceOverSystem.instance.VoiceOverEvent("Scrapper", "It looks like both systems that need" +
            "to be fixed are in the basement. Once I fix those, I can press the big red" +
            "button", soundClip);

        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<SCRAPS_Interactive>().enabled = false;
    }
}
