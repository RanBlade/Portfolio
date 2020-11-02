using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlRoomVoiceOverScript : MonoBehaviour
{
    public EHR_RoomLights objectRef;

    public AudioClip soundClip;
    public GameObject SystemsCutScene;

    private bool doOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(objectRef.completed && !doOnce)
        {
            VoiceOverSystem.instance.VoiceOverEvent("Scrapper", "I wonder if these computers will" +
                " show me what systems need to be fixed.", soundClip);
            SystemsCutScene.GetComponent<StartCutSceneScript>().CanRun();
            doOnce = true;
        }
    }
}
