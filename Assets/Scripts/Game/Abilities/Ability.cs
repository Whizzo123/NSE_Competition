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
    protected bool used;
    protected bool inUse;
    protected float fullCharge;
    protected float currentCharge;

    public Ability()
    {
        name = "default";
        description = "This is an ability";
        pointsCost = 1;
        useType = AbilityUseTypes.ONE_TIME;
        used = false;
        fullCharge = 0;
    }

    public Ability(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityType, float amountToCharge = 0)
    {
        name = abilityName;
        description = abilityDescription;
        pointsCost = abilityCost;
        useType = abilityType;
        used = false;
        fullCharge = amountToCharge;
    }

    virtual public Ability Clone()
    {
        return new Ability(name, description, pointsCost, useType, fullCharge);
    }


    virtual public void UpdateAbility()
    {

    }

    virtual protected IEnumerator EffectDurationCountdown()
    {
        yield return null;
    }

    virtual public void Use()
    {
        BoltLog.Info("Just calling plain old use method");
        if (useType == AbilityUseTypes.ONE_TIME)
        {
            used = true;
        }
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
