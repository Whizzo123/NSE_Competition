﻿using System;

public class Trap : Ability
{
    protected PlayerController placingPlayer;

    public Trap(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityType, float amountToCharge = 0) 
        : base(abilityName, abilityDescription, abilityCost, abilityType, amountToCharge)
    {

    }

    public override void UpdateAbility()
    {
        if(used)
        {
            //Destroy from inventory
            placingPlayer.abilityInventory.RemoveAbilityFromInventory(this);
        }
    }

    public void SetPlacingPlayer(PlayerController player)
    {
        placingPlayer = player;
    }

}
