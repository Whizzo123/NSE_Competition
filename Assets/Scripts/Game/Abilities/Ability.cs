using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AbilityUseTypes { PASSIVE, RECHARGE, ONE_TIME};
public enum AbilityType { POWERUP, DEBUFF, TRAP};

public class Ability
{
    //Player reference
    protected string name;
    protected string description;
    protected int pointsCost;
    protected AbilityInventory inventory;
    protected AbilityUseTypes useType;
    private AbilityType abilityType;
    protected bool used;
    protected bool inUse;
    protected float fullCharge;
    protected float currentCharge;
    private float duration;
    private float currentDuration;
    private PlayerController castingPlayer;
    private PlayerController targetedPlayer;
    private Action<Ability> effectInvokedOnUse;
    private Action<Ability> effectInvokedOnEnd;
    private bool oppositeDebuffActivated;
    private int useCount;

    public Ability()
    {
        name = "default";
        description = "This is an ability";
        pointsCost = 1;
        useType = AbilityUseTypes.ONE_TIME;
        used = false;
        fullCharge = 0;
        oppositeDebuffActivated = false;
    }
    //Extra duration parameter, extra abilitytype parameter
    public Ability(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityUseType, AbilityType abilityType, float abilityDuration, Action<Ability> onUseAction, Action<Ability> onEndAction, float amountToCharge = 0)
    {
        name = abilityName;
        description = abilityDescription;
        pointsCost = abilityCost;
        useType = abilityUseType;
        used = false;
        fullCharge = amountToCharge;
        currentCharge = fullCharge;
        duration = abilityDuration;
        this.abilityType = abilityType;
        oppositeDebuffActivated = false;
        effectInvokedOnUse += onUseAction;
        effectInvokedOnEnd += onEndAction;
    }
    //would still need a clone method
    public Ability Clone()
    {
        return new Ability(name, description, pointsCost, useType, abilityType, duration, effectInvokedOnUse, effectInvokedOnEnd, fullCharge);
    }

    //keep a written version of this
    //virtual public void UpdateAbility()
    //{

    //}

    public void UpdateAbility()
    {
        if (abilityType == AbilityType.POWERUP || abilityType == AbilityType.DEBUFF)
        {
            if (useType == AbilityUseTypes.RECHARGE)
            {
                if (inUse)
                {
                    if (currentDuration < duration)
                    {
                        currentDuration += Time.deltaTime;
                        if (currentDuration > duration)
                            currentDuration = duration;
                    }
                    else
                    {
                        currentDuration = 0;
                        EndEffect();
                    }
                }
                else
                {
                    if (currentCharge < fullCharge)
                    {
                        currentCharge += Time.deltaTime;
                        if (currentCharge > fullCharge)
                        {
                            currentCharge = fullCharge;
                            GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotChargingState(name, false);
                        }
                    }
                }
            }
            else if (useType == AbilityUseTypes.ONE_TIME)
            {
                if (inUse)
                {
                    if (currentDuration < duration)
                    {
                        currentDuration += Time.deltaTime;
                        if (currentDuration > duration)
                            currentDuration = duration;
                    }
                    else
                    {
                        currentDuration = 0;
                        EndEffect();
                    }
                }
            }
            else if (useType == AbilityUseTypes.PASSIVE)
            {
                Use();
            }
        }
        else
        {
            if (used && useCount >= 3)
            {
                castingPlayer.abilityInventory.RemoveAbilityFromInventory(this);
            }
        }
    }

    public void Use()
    {
        AnimatorStateInfo state = castingPlayer.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        Animator animator = castingPlayer.transform.GetChild(0).GetComponent<Animator>();
        switch (abilityType)
        {
            case (AbilityType.POWERUP):
                if (useType == AbilityUseTypes.ONE_TIME)
                {
                    used = true;
                }
                break;
            case (AbilityType.DEBUFF):
                if (!state.IsName("Throw"))
                {
                    animator.SetTrigger("Throw");
                    used = true;
                }
                break;
            case (AbilityType.TRAP):
                if (!state.IsName("PutDown"))
                {
                    animator.SetTrigger("PutDown");
                }
                useCount++;
                break;
        }
        effectInvokedOnUse.Invoke(this);
    }

    private void EndEffect()
    {
        if(effectInvokedOnEnd != null)
            effectInvokedOnEnd.Invoke(this);
        if (abilityType == AbilityType.POWERUP || abilityType == AbilityType.DEBUFF)
        {
            currentCharge = 0;
            currentDuration = 0;
            inUse = false;
            GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotChargingState(name, true);
        }
        if (useType == AbilityUseTypes.ONE_TIME)
            castingPlayer.abilityInventory.RemoveAbilityFromInventory(this);
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

    public AbilityUseTypes GetUseType()
    {
        return useType;
    }

    public float GetChargeAmount()
    {
        return fullCharge;
    }

    public float GetCurrentCharge()
    {
        return currentCharge;
    }

    public void SetInventory(AbilityInventory _inventory)
    {
        inventory = _inventory;
    }

    public PlayerController GetCastingPlayer()
    {
        return castingPlayer;
    }

    public void SetCastingPlayer(PlayerController player)
    {
        castingPlayer = player;
    }

    public bool IsOppositeDebuffActivated()
    {
        return oppositeDebuffActivated;
    }

    public void SetOppositeDebuffActivated(bool activated)
    {
        oppositeDebuffActivated = activated;
    }

    public bool IsInUse()
    {
        return inUse;
    }

    public void SetInUse(bool use)
    {
        inUse = use;
    }

    public PlayerController GetTargetedPlayer()
    {
        return targetedPlayer;
    }

    public void SetTargetedPlayer(PlayerController target)
    {
        targetedPlayer = target;
    }

    public float GetDuration()
    {
        return duration;
    }

    public AbilityType GetType()
    {
        return abilityType;
    }
}
