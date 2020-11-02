using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ITEM_TYPE
{
    GOLD = 0,
    WEAPON,
    ARMOR,
    POTION,
    NONE
}

/*
 *This is a very simple overlapping item system
 * By declaring a type we can sort items
 * If a field is not used by an item set to -1. 
 * This way we have items without using Inheritence.
 * If this was a bigger game Items would be handled compelelty differenlty
 */
public class Item  {

    public string Name;
    public ITEM_TYPE Type;
    public int count;
    public int Damage;
    public int Armor;
    

    public Item()
    {
        Name = "NA";
        Type = ITEM_TYPE.NONE;

        count = -1;
        Damage = -1;
        Armor = -1;
    }

}
