using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_MasterTwistEvent : EHR_BaseEvent
{
    public EHR_TriggerCheck enableEvent;

    public GameObject barrelOne;
    public GameObject barrelTwo;
    public GameObject barrelThree;
    public GameObject houseToDestroy;
    public AudioClip soundClip;

    public int barrelsExploded = 0;
    private bool enableBarrels = false;
    private bool eventRun = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if(enableEvent.areaTriggered && !enableBarrels)
        {
            enableBarrels = true;
            barrelOne.SetActive(true);
            barrelTwo.SetActive(true);
            barrelThree.SetActive(true);

        }*/

        if(!eventRun && barrelsExploded >= 3)
        {
            RunEvent();
        }
    }

    public override void RunEvent()
    {
        foreach(Transform child in transform)
        {
            if(!child.gameObject.activeInHierarchy)
            {
                child.gameObject.SetActive(true);
            }
        }
        houseToDestroy.SetActive(false);
        gameObject.GetComponent<AudioSource>().Play();
        VoiceOverSystem.instance.VoiceOverEvent("Raynick", "WHAT WAS THAT! DO NOT DAMAGE MY POWER " +
            "PLANT ANY FURTHUR!", soundClip);
        Debug.Log("Running MasterTwistEvent");
        completed = true;
        eventRun = true;
    }
}
