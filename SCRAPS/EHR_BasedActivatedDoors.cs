using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_BasedActivatedDoors : MonoBehaviour
{
    public EHR_BaseActivatedEvent baseOn;

    public Animator doorAnim;
    public AudioSource doorAudio;
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
        if(baseOn.checkOnceAll)
        {
            doorAnim.SetBool("doorOpen", true);
            doorAudio.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(baseOn.checkOnceAll)
        {
            doorAnim.SetBool("doorOpen", false);
            doorAudio.Play();
        }
    }
}
