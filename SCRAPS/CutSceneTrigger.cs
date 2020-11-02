using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    public StartCutSceneScript cutScene;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            cutScene.thePlayer = other.transform;
            cutScene.playerCam = GameObject.FindWithTag("MainCamera");
            cutScene.lastPos = other.transform.position;
            cutScene.cutCam.SetActive(true);
            cutScene.StartCutScene();

            Destroy(gameObject);
        }
    }
}
