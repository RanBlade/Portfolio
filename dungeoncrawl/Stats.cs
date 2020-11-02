using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour  {

    //Basic stats
    [Header("Vitals")]
    public int currentHitPoints;
    public int MaxHitPoints = 100;
    public int currentStamina;
    public int MaxStamina = 100;
    public int currentMana;
    public int MaxMana = 100;

    //attributes
    [Header("Attributes")]
    public int Strength;
    public int Dexterity;
    public int Intellegence;

    //Attack/Defend Stats
    [Header("AttackValues")]
    public int ArmorValue;
    public int WeaponDamage;
    public int SpeedMultiplier;

    public void LoadStats()
    {
        Debug.Log("Loading Stats");
        ApplyAttributeMultipliers();
        currentHitPoints = MaxHitPoints;
        currentStamina = MaxStamina;
        currentMana = MaxMana;
    }
    public void UpdatAlleStats()
    {
        ApplyAttributeMultipliers();
    }

    void ApplyAttributeMultipliers()
    {
        MaxHitPoints = MaxHitPoints + ((Strength * Strength) / MaxHitPoints);
        MaxStamina = MaxStamina + ((Dexterity * Dexterity)/MaxStamina);
        MaxMana = MaxMana + ((Intellegence * Intellegence) / MaxMana);
        CalculateWeaponDamage();
        
    }

    public void ApplyDamage(int Damage)
    {
        if(ArmorValue > 0)
        {
            Damage = Damage - (ArmorValue / 2);
            if(Damage <= 0)
            {
                currentHitPoints -= 1;
            }
            else
            {
                currentHitPoints -= Damage;
            }
        }
        else
        {
            currentHitPoints -= Damage;
        }
    }

    public void CalculateWeaponDamage()
    {
        WeaponDamage = WeaponDamage + ((Strength / 2) + (Dexterity / 3));
    }
}
