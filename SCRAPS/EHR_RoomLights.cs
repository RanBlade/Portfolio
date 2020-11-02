using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_RoomLights : EHR_BaseEvent
{
    public GameObject[] roomLights;
    public int lightCount;

    public AudioClip soundClip;
    public GameObject SystemsCutScene;

    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        roomLights = new GameObject[lightCount];
        
        foreach(Transform child in transform)
        {
            if(child.tag == "LightSource")
            {
                Debug.Log("Found a light: " + child.name);
                foreach (Transform lightchild in child)
                {
                    if (lightchild.tag == "Light")
                    {
                        roomLights[count] = lightchild.gameObject;
                        count++;
                    }
                }
            }
        }

        for(int i = 0; i < count; i++)
        {
            roomLights[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override public void RunEvent()
    {
        for(int i = 0; i < count; i++)
        {
            roomLights[i].SetActive(true);
            
        }
        
        completed = true;
    }
}
