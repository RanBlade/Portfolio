using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_ObjectSnap : MonoBehaviour
{
    [Header("Snap Setup Objects")]
    [SerializeField]
    private GameObject staticObj;
    [SerializeField]
    private GameObject parentWireObj;
    [SerializeField]
    private bool isObjectWire = false;
    [SerializeField]
    private EHR_BaseEvent EventScriptRef;
    [SerializeField]
    private SCRAPS_Objective objectiveRef;
    [SerializeField]
    private string snapName;

    public bool pipeSnapped = false;
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
        if (!pipeSnapped)
        {
            //Debug.Log("Trigger for Object Snapping Entered: " + other.name);
            if (other.GetComponent<EHR_Snappable>() != null && !isObjectWire)
            {
                if (other.GetComponent<EHR_Snappable>().Snapname == snapName)
                {
                    Debug.Log("Testing if pipes are having nullref");
                    
                    //staticObj.SetActive(true);
                    if(objectiveRef != null)
                    {
                        objectiveRef.UpdateObjective(1);
                    }

                    pipeSnapped = true;
                    if (EventScriptRef != null)
                    {
                        EventScriptRef.RunEvent();
                    }
                     
                    other.GetComponent<EHR_Snappable>().SetObjActive();
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                }
            }
            else if (other.GetComponent<EHR_Snappable>() != null && isObjectWire)
            {
                Debug.Log("Wire Can Snap!");
                Debug.Log("Testing if wires are having nullref");
                transform.position = other.gameObject.transform.position;
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                gameObject.GetComponent<SCRAPS_GravnullObject>().enabled = false;
                if (EventScriptRef != null)
                {
                    EventScriptRef.RunEvent();
                }
                pipeSnapped = true;
                if (objectiveRef != null)
                {
                    objectiveRef.UpdateObjective(1);
                }
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
                if(staticObj != null && parentWireObj != null)
                {
                    staticObj.SetActive(true);
                    parentWireObj.SetActive(false);

                }

            }
        }
    }
}
