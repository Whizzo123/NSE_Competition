using System;
using System.Collections;
using UnityEngine;

public class Powerup : Ability
{

    private PlayerController playerToEmpower;
    private float effectDuration;
    private float currentDuration;

    public Powerup(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityType, float amountToCharge = 0, float durationOfEffect = 0) 
        : base(abilityName, abilityDescription, abilityCost, abilityType, amountToCharge)
    {
        effectDuration = durationOfEffect;
    }

    public override void UpdateAbility()
    {
        if (useType == AbilityUseTypes.RECHARGE)
        {
            if (inUse)
            {
                //Do stuff to do with duration of effect
                if (currentDuration < effectDuration)
                {
                    currentDuration += Time.deltaTime;
                    if (currentDuration > effectDuration)
                        currentDuration = effectDuration;
                }
                else
                {
                    EndEffect();
                }
            }
            else
            {
                if(currentCharge < fullCharge)
                {
                    currentCharge += Time.deltaTime;
                    if (currentCharge > fullCharge)
                        currentCharge = fullCharge;
                }
            }
        }
        else if (useType == AbilityUseTypes.ONE_TIME && used)
        {
            //Destroy

        }
        else if (useType == AbilityUseTypes.PASSIVE && !used)
        {
            used = true;
            Use();
        }
    }

    protected virtual void EndEffect()
    {
        currentCharge = 0;
        currentDuration = 0;
        inUse = false;
    }

    public PlayerController GetPlayerToEmpower()
    {
        return playerToEmpower;
    }

    public void SetPlayerToEmpower(PlayerController player)
    {
        playerToEmpower = player;
    }
}
