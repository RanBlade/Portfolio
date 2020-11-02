using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TransitionScript : MonoBehaviour {

    public string SceneToLoad;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TransitionScene()
    {
        Debug.Log("Changing Scene");
        GameManager.instance.SceneChange = true;
        SceneManager.LoadScene(SceneToLoad);
    }

}
