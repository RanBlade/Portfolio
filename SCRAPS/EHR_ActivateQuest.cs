using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_ActivateQuest : MonoBehaviour
{
    public SCRAPS_Objective objectiveRef;

    public bool activate = false;
    public bool doOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activate && !doOnce)
        {
            objectiveRef.gameObject.SetActive(true);
            doOnce = true;
        }
    }
}
