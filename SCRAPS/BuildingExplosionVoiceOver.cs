using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingExplosionVoiceOver : MonoBehaviour
{
    public SCRAPS_Objective objectiveRef;
    public AudioClip soundClip;
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
            VoiceOverSystem.instance.VoiceOverEvent("Scrapper", "That house looks like a great place to get" +
                "a wall section. But I have to place the explosives just right!", soundClip);
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
