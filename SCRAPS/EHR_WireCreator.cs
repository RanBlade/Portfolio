using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -------------------------------------------------------------------------------
 * This script is designed to be able to make modular wires in the editor. I have come to
 * the values used through simple testing. I might add more modification to the editble variables
 * through the inspector at a later time. 
 * -------------------------------------------------------------------------------
 */
[ExecuteInEditMode]
public class EHR_WireCreator : MonoBehaviour
{
    [Header("Regenerate Wire Options")]
    public bool reGenWires = false;
    public int wireCount = 5;
    public GameObject baseObject;
    public float spacing;
    [Header("Spring Values")]
    public bool useSpringInHingeJoint = true;
    public float springTension = 0.0f;
    public float springDampening = 0.0f;
    [Header("SCRAPS OPTIONS")]
    public bool isGravnull = false;
    public bool isSnappable = false;
    [Header("The Wire Array - DO NOT EDIT - Visible for testing!")]
    public GameObject[] wires;
    
    
    // Start is called before the first frame update
  
    // Update is called once per frame
    void Update()
    {
        //check the bool for regeneration and if this is in Editor mode. 
        //if I am correct this block cannot be called when the game is running.
        if (reGenWires && Application.isEditor)
        {
            //if our array is not empty then clear the array and delete the objects.
            foreach (GameObject go in wires)
            {
                if (go != null)
                {
                    Debug.Log("Destroying: " + go.name);
                    DestroyImmediate(go);
                }
            }

            Debug.Log("Regening wires");
            wires = new GameObject[wireCount];
            float objectOffset = 0.0f;
            for (int i = 0; i < wireCount; i++)
            {
                GameObject temp = Instantiate(baseObject, transform);
                temp.SetActive(true);
                temp.transform.position = transform.forward + transform.position + new Vector3(0.0f, 0.0f, objectOffset);
                objectOffset += spacing;
                //temp.transform.parent = transform;
                temp.AddComponent<HingeJoint>();
                temp.GetComponent<HingeJoint>().useSpring = useSpringInHingeJoint;
                JointSpring springlvl = new JointSpring();
                springlvl.spring = springTension;
                springlvl.damper = springDampening;
                temp.GetComponent<HingeJoint>().spring = springlvl;                              
                temp.GetComponent<Rigidbody>().useGravity = true;
                temp.GetComponent<Rigidbody>().mass = 1500.0f;

                //Lets connect the hinge of this object to the previous segment.
                if (i > 0)
                {
                    temp.GetComponent<HingeJoint>().connectedBody = wires[i - 1].GetComponent<Rigidbody>();
                }
                //if this is the first object in the array then we want to freeze
                //rotation and position
                if(i == 0)
                {
                    temp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                }
                //The last object on the wire needs to have some constraints applied
                //we also want to add a gravnull object and a snappable script.
                if(i == wireCount - 1)
                {
                    if (isGravnull)
                    {
                        temp.AddComponent<SCRAPS_GravnullObject>();
                    }
                    if(isSnappable)
                    {
                        temp.AddComponent<EHR_ObjectSnap>();
                    }
                    temp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
                }
                wires[i] = temp;//add the segment to the array.
            }

            reGenWires = false;

        }
    }
}
