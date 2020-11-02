using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest {

    public string Name;
    public string Description;
    public int ID;
    public bool ObjectiveCollected;
    public bool QuestCompleted;
    // Use this for initialization

    static int IDCounter;
	public Quest()
    {
        Name = "none";
        ID = IDCounter++;
        Description = "none";
        ObjectiveCollected = false;
        QuestCompleted = false;
    }
}
