using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCutSceneScript : MonoBehaviour
{

    public GameObject playerCam;
    public GameObject cutCam;
    private Animator cutAnim;
    private bool lockPlayer = false;
    public Transform thePlayer;
    public Transform playerEndPos;
    public Vector3 lastPos = Vector3.zero;

    public bool bCutSceneMovesPlayer = false;
    public bool doOnce = false;

    public bool canRun = true;

    // Start is called before the first frame update
    void Start()
    {
        cutAnim = GetComponent<Animator>();
        cutCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(lockPlayer)
        {
            if(thePlayer != null && bCutSceneMovesPlayer)
            {
                thePlayer.position = playerEndPos.position;
            }
            else if(thePlayer != null && !bCutSceneMovesPlayer)
            {
                thePlayer.position = lastPos;
            }
        }
    }

    public void StartCutScene()
    {
        if (canRun)
        {
            playerCam.SetActive(false);
            lockPlayer = true;

            cutAnim.SetTrigger("startCutScene");
        }
    }

    public void StopCutScene()
    {
        playerCam.SetActive(true);
        cutCam.SetActive(false);
        lockPlayer = false;
        Debug.Log("Stopping cut scene");
        //thePlayer.position = playerEndPos.position;
        //if(doOnce)
       // {
       //     gameObject.SetActive(false);
       // }
    }

    public void CanRun()
    {
        canRun = true;
    }

    public void StartCutSceneInteraction()
    {
        thePlayer = GameObject.FindWithTag("Player").transform;
        playerCam = GameObject.FindWithTag("MainCamera");
        lastPos = thePlayer.transform.position;
        cutCam.SetActive(true);
        StartCutScene();

        //Destroy(gameObject);
    }
}
