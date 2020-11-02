using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_SnappingManager : MonoBehaviour
{
    [Header("Mechanic Objects")]
    [SerializeField]
    private GameObject mechanicDamageVol;
    [SerializeField]
    private GameObject mechanicParticleSys;
    [SerializeField]
    private GameObject mechanicSFX;
    [SerializeField]
    private GameObject mechanicPipeSnap;

    [Header("Objective Objects")]
    [SerializeField]
    private SCRAPS_Objective snappingObjective;
    [SerializeField]
    private EHR_ObjectiveTracker objectiveTwo;

    //helper variables
    private bool doOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mechanicPipeSnap.GetComponent<EHR_ObjectSnap>().pipeSnapped && !doOnce)
        {
            mechanicDamageVol.SetActive(false);
            mechanicParticleSys.SetActive(false);
            snappingObjective.UpdateObjective(1);
            mechanicSFX.SetActive(false);

            SCRAPS_MessageSystem.instance.NewMessage("Scrapper",
                "Looks like it is safe to go in the area now!",
                SCRAPS_MessageSystem.msgType.standard);

            doOnce = true;
        }
        if(objectiveTwo.objectiveMet && !snappingObjective.isComplete)
        {
            snappingObjective.UpdateObjective(1);
        }
    }
}
