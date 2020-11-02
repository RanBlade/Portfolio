using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("UI Text Objects")]
    [SerializeField]
    Text HealthText;
    [SerializeField]
    Text StaminaText;
    [SerializeField]
    Text ManaText;
    [SerializeField]
    Text InventoryText;
    [SerializeField]
    Text QuestText;

    //private strings for UI text
    string HealthTextString = "ERROR: NOT SET";
    string StaminaTextString = "ERROR: NOT SET";
    string ManaTextString = "ERROR: NOT SET";

    GameObject UICanvas;

    
    // Use this for initialization
    void Start () {

        HealthText = GameObject.Find("HealthText").GetComponent<Text>();
        StaminaText = GameObject.Find("StamText").GetComponent<Text>();
        ManaText = GameObject.Find("ManaText").GetComponent<Text>();
        InventoryText = GameObject.Find("InventoryText").GetComponent<Text>();
        QuestText = GameObject.Find("QuestText").GetComponent<Text>();

        HealthText.color = Color.red;
        StaminaText.color = Color.green;
        ManaText.color = Color.blue;
    }
	
	// Update is called once per frame
	void Update () {
        HealthText.text = HealthTextString;
        StaminaText.text = StaminaTextString;
        ManaText.text = ManaTextString;
	}

    public void UpdateHealthText(string CurrentString, string MaxString)
    {
        HealthTextString = "Health: " + CurrentString + "/" + MaxString;

    }
    public void UpdateStaminaText(string CurrentString, string MaxString)
    {
        StaminaTextString = "Stamina: " + CurrentString + "/" + MaxString;
    }
    public void UpdateManaText(string CurrentString, string MaxString)
    {
        ManaTextString = "Mana: " + CurrentString + "/" + MaxString;
    }

    public void UpdateInventoryText(string Invtext)
    {
        InventoryText.text = Invtext;
    }
    public void UpdateQuestText(string questtext)
    {
        QuestText.text = questtext;
    }
}
