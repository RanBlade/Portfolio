using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void QuestRequest(GameObject QuestRequester)
    {
        Quest GivenQuest = new Quest();
       // Debug.Log("Quest Requested. Giving Quest: " + GivenQuest.Name);
        GivenQuest = GameManager.instance.GiveQuest();
        if (GivenQuest != null)
        {
            Debug.Log("Quest Requested. Giving Quest: " + GivenQuest.Name);
        }
        else
        {
            Debug.Log("Quest Requested. Not quest to give");
        }

        if (!QuestRequester.GetComponent<PlayerController>().OnQuest())
        {
            QuestRequester.GetComponent<PlayerController>().ReceiveQuest(GivenQuest);
        }
        else
        {
            Debug.Log("Player Already on quest");
        }
    }

    public void CompleteQuest(Quest completedQuest)
    {
        completedQuest.QuestCompleted = true;
    }

    void InputHandler()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {

        }
    }
}
