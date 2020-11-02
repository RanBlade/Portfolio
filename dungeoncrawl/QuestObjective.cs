using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjective : MonoBehaviour {

    public int QuestID = -1;

    public GameObject QuestObject;
	// Use this for initialization
	void Start () {
        QuestObject = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool GiveQuestObjective(int tempID)
    {
        if (QuestID == tempID)
        {
            Debug.Log("QuestID match Player collecting item");
            QuestObject.GetComponent<QuestObjectController>().OnCompletedQuest();
            return true;
        }
        else
        {
            Debug.Log("QuestID does not match. Keep looking");
            return false;
        }
    }
}
