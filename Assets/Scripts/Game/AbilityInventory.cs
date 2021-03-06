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

    public void AddAbilityToInventory(Ability ability)
    {
        if(ability.GetType().IsSubclassOf(typeof(Powerup)))
        {
            Powerup powerup = (Powerup)ability;
            powerup.SetPlayerToEmpower(player);
            abilities.Add(powerup);
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
