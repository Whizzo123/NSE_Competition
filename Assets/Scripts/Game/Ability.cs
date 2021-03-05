using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AbilityUseTypes { PASSIVE, RECHARGE, ONE_TIME};

public class Ability
{

    protected string name;
    protected string description;
    protected int pointsCost;
    protected Action<Ability> onAbilityUse;
    protected AbilityUseTypes useType;
    private bool used;
    protected float fullCharge;
    protected float currentCharge;

    public Ability()
    {
        name = "default";
        description = "This is an ability";
        pointsCost = 1;
        onAbilityUse += DefaultAbility;
        useType = AbilityUseTypes.ONE_TIME;
        used = false;
        fullCharge = 0;
    }

    public Ability(string abilityName, string abilityDescription, int abilityCost, Action<Ability> onUse, AbilityUseTypes abilityType, float amountToCharge = 0)
    {
        name = abilityName;
        description = abilityDescription;
        pointsCost = abilityCost;
        onAbilityUse += onUse;
        useType = abilityType;
        used = false;
        fullCharge = amountToCharge;
    }

    public void UpdateAbility()
    {
        if (useType == AbilityUseTypes.RECHARGE)
        {
            if (currentCharge < fullCharge)
            {
                currentCharge += Time.deltaTime;
                if (currentCharge > fullCharge)
                    currentCharge = fullCharge;
            }
        }
        else if (useType == AbilityUseTypes.PASSIVE)
        {
            Use(this);
        }
    }

    virtual public void Use(Ability ability)
    {
        if (used)
        {
            //Somehow get rid of ability from player use
        }
        else
        {
            onAbilityUse.Invoke(ability);
            if (useType == AbilityUseTypes.ONE_TIME)
            {
                used = true;
            }
        }
    }

    private void DefaultAbility(Ability ability)
    {
        Debug.LogWarning("void DefaultAbility(): You have activated my trap card!!!");
    }

    public string GetAbilityName()
    {
        return name;
    }

    public string GetAbilityDescription()
    {
        return description;
    }

    public int GetAbilityCost()
    {
        return pointsCost;
    }

    public Action<Ability> GetEffect()
    {
        return onAbilityUse;
    }

    public AbilityUseTypes GetUseType()
    {
        return useType;
    }

    public float GetChargeAmount()
    {
        return fullCharge;
    }
}
