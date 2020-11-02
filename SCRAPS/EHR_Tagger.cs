using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_Tagger : MonoBehaviour
{
    [SerializeField]
    private string objTag = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetTag()
    {
        //We want to let the player know this obj is not tagged so any testers can fix this
        if(objTag == "")
        {
            SCRAPS_MessageSystem.instance.NewMessage("ERROR", 
                "THIS OBJECT NOT TAGGED within EHR_Tagger!", SCRAPS_MessageSystem.msgType.bad);
            return objTag;
        }
        else
        {
            return objTag;
        }
    }
}
