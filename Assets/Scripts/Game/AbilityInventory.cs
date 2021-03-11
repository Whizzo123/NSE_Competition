using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class AbilityInventory
{

    private PlayerController player;

    private List<Ability> abilities;

    private List<Ability> removeList;

    public AbilityInventory(PlayerController playerOwningInventory)
    {
        player = playerOwningInventory;
        abilities = new List<Ability>();
    }

    public void Update()
    {
        if (removeList != null)
        {
            foreach (Ability ability in removeList)
            {
                abilities.Remove(ability);
            }
            removeList = null;
        }
        foreach (Ability ability in abilities)
        {
            ability.UpdateAbility();
        }
    }

    public void ActivateAbility(string abilityName)
    {
        BoltLog.Info("Inside activate ability: " + abilityName);
        foreach (Ability ability in abilities)
        {
            BoltLog.Info("Enumeration: " + ability.GetAbilityName());
            if(ability.GetAbilityName() == abilityName && ability.GetCurrentCharge() == ability.GetChargeAmount())
            {
                BoltLog.Info("Found ability calling use function");
                ability.Use();
                break;
            }
        }
    }

    public void AddAbilityToInventory(Ability ability)
    {
        if(ability.GetType().IsSubclassOf(typeof(Powerup)))
        {
            Powerup powerup = (Powerup)ability;
            powerup.SetPlayerToEmpower(player);
            powerup.SetInventory(this);
            abilities.Add(powerup);
        }
        else if(ability.GetType().IsSubclassOf(typeof(Debuff)))
        {
            Debuff debuff = (Debuff)ability;
            debuff.SetCastingPlayer(player);
            debuff.SetInventory(this);
            abilities.Add(debuff);
        }
        else if(ability.GetType().IsSubclassOf(typeof(Trap)))
        {
            Debug.Log("Setting trap player");
            Trap trap = (Trap)ability;
            trap.SetInventory(this);
            trap.SetPlacingPlayer(player);
            abilities.Add(trap);
        }
        else
        {
            BoltLog.Error("ERROR: ABILITY ATTEMPTING TO BE ADDED TO INVENTORY THAT HAS NO SUBTYPE");
        }
        
    }

    public Ability FindAbility(string abilityName)
    {
        foreach (Ability ability in abilities)
        {
            if(ability.GetAbilityName() == abilityName)
            {
                return ability;
            }
        }
        return null;
    }

    public void RemoveAbilityFromInventory(Ability ability)
    {
        removeList = new List<Ability>();
        removeList.Add(ability);
        GameObject.FindObjectOfType<AbilitySlotBarUI>().RemoveItemFromBar(ability.GetAbilityName());
    }

}
