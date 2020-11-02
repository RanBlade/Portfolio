using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextItemController : MonoBehaviour {

    public string GameoverString;
    public bool winGame;
    public bool InfoSet = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (InfoSet == true)
        {
            if (SceneManager.GetActiveScene().name == "GameOver")
            {
                GameObject.Find("WinLoseText").gameObject.GetComponent<Text>().text = GameoverString;
                if (winGame)
                {
                    GameObject.Find("WinLoseText").gameObject.GetComponent<Text>().color = Color.green;
                }
                else
                {
                    GameObject.Find("WinLoseText").gameObject.GetComponent<Text>().color = Color.red;
                }
            }
        }
    }

    public void SetGameOverString(string temp, bool winLose)
    {
        GameoverString = temp;
        winGame = winLose;
        InfoSet = true;
    }
}
