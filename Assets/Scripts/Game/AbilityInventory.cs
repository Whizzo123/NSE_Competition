using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class AbilityInventory
{

    private PlayerController player;

    private List<Ability> abilities;

    public AbilityInventory(PlayerController playerOwningInventory)
    {
        player = playerOwningInventory;
        abilities = new List<Ability>();
    }

    public void Update()
    {
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
            if(ability.GetAbilityName() == abilityName)
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
            abilities.Add(powerup);
        }
        else if(ability.GetType().IsSubclassOf(typeof(Debuff)))
        {
            Debuff debuff = (Debuff)ability;
            debuff.SetCastingPlayer(player);
            abilities.Add(debuff);
        }
        else
        {
            abilities.Add(ability);
        }
        
    }

    public void RemoveAbilityFromInventory(Ability ability)
    {
        abilities.Remove(ability);
    }

}
