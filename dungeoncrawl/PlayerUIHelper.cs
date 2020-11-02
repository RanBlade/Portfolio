using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHelper : MonoBehaviour {

    public UIManager UIManagerRef;
    Stats PlayerStats;
    Inventory PI;
    QuestLog PLog;

    string InventoryStiring;
    string QuestLog = "No Current Quest";
	// Use this for initialization
	void Start () {
        UIManagerRef = GameObject.Find("GameManager").GetComponent<UIManager>();
        PlayerStats = gameObject.GetComponent<Stats>();
        PI = gameObject.GetComponent<Inventory>();
        PLog = gameObject.GetComponent<QuestLog>();
	}
	
	// Update is called once per frame
	void Update () {
        UIManagerRef.UpdateHealthText(PlayerStats.currentHitPoints.ToString(), PlayerStats.MaxHitPoints.ToString());
        UIManagerRef.UpdateStaminaText(PlayerStats.currentStamina.ToString(), PlayerStats.MaxStamina.ToString());
        UIManagerRef.UpdateManaText(PlayerStats.currentMana.ToString(), PlayerStats.MaxMana.ToString());
        UIManagerRef.UpdateInventoryText(FormatInventoryString());
        UIManagerRef.UpdateQuestText(FormatQuestLogString());

    }

    string FormatInventoryString()
    {
        InventoryStiring = PI.Gold.Name + ": " + PI.Gold.count.ToString() + "\n\n" +
            PI.Weapon.Name + "\n" + "Damage: " + PI.Weapon.Damage + "\n\n" +
            PI.Armor.Name + "\n" + "ArmorVal: " + PI.Armor.Armor + "\n\n" +
            PI.Potion.Name + " x " + PI.Potion.count;

        return InventoryStiring;
    }
    string FormatQuestLogString()
    {
        if (PLog.CurrentQuest != null)
        {
            string ObjectiveComplete;
            if (PLog.CurrentQuest.ObjectiveCollected)
                ObjectiveComplete = "Complete";
            else
                ObjectiveComplete = "Incomplete";

            QuestLog = PLog.CurrentQuest.Name + "\n\n" + PLog.CurrentQuest.Description + "\n " + ObjectiveComplete;
            return QuestLog;
        }
        else
        {
            return "No active quests";
        }
    }
}
