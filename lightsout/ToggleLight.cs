using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLight : MonoBehaviour
{
    [SerializeField]
    private Material lightOnMat;
    [SerializeField]
    private Material lightOffMat;
    [SerializeField]
    private int[] adjacentLightsID;

    private bool lightStatus = false;
    public int gridID = 0;

    [SerializeField]
    private LightsOut_GameManager currentGameMannager;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    //This function is to be able to toggle lights on and off after the game has started
    public void Toggle()
    {
        if(lightStatus)
        {
            lightStatus = false;
            gameObject.GetComponent<Renderer>().material = lightOffMat;
        }
        else if(!lightStatus)
        {
            lightStatus = true;
            gameObject.GetComponent<Renderer>().material = lightOnMat;
        }
    }

    //This function is so the game manager can randomly set lights on and off
    public void SetLightStatus(bool newStatus)
    {
        lightStatus = newStatus;

        if (lightStatus == false)
        {
            gameObject.GetComponent<Renderer>().material = lightOffMat;
        }
        else if (lightStatus == true)
        {
            gameObject.GetComponent<Renderer>().material = lightOnMat;
        }
    }

    public bool GetLightStatus()
    {
        return lightStatus;
    }
    public void OnMouseUp()
    {
        if (currentGameMannager.CheckIfGameIsActive())
        {
            currentGameMannager.PlayGridLoc(gridID);
        }
    }

    public int[] GetAdjacentLights()
    {
        return adjacentLightsID;
    }
}
