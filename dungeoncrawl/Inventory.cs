using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

   
   public Item Weapon = null;
   public Item Armor = null;
   public Item Gold = null;
   public Item Potion = null;

	// Use this for initialization
	void Start () {
       //Weapon = new Item();
       // Armor = new Item();
       // Gold = new Item();
       // Potion = new Item();
	}   
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateLoot(ITEM_TYPE type)
    {
        if(type == ITEM_TYPE.ARMOR)
        {
            Armor.Armor += 10;
        }
        else if(type == ITEM_TYPE.WEAPON)
        {
            Weapon.Damage += 10;
        }
        else if(type == ITEM_TYPE.POTION)
        {
            Potion.count += 1;
        }
        else if(type == ITEM_TYPE.GOLD)
        {
            Gold.count += 50;
        }
        else if (type == ITEM_TYPE.NONE)
        {
            Debug.Log("No loot given");
        }
    }
}
