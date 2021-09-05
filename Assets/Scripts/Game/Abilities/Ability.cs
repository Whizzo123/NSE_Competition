using UnityEngine;
using System;

public enum AbilityUseTypes { PASSIVE, RECHARGE, ONE_TIME};
public enum AbilityType { POWERUP, DEBUFF, TRAP};

/// <summary>
/// Base class for all abilities. Contains usage, functions and all information regarding the abililty besides <see cref="Effects"/>
/// </summary>
public class Ability
{

    #region VARIABLES
    [Header("Basic info")]
    [Tooltip("Name of ability")]protected string name;
    [Tooltip("Description for ability")]protected string description;
    [Tooltip("Cost of abilities")]protected int pointsCost;
    [Tooltip("How is it stored?")] protected AbilityUseTypes useType;
    [Tooltip("How does it affect gameplay?")] private AbilityType abilityType;
    [Space]

    [Header("Usage")]
    [Tooltip("JoeComment")] protected bool used;
    [Tooltip("JoeComment what is the need for used and inUse being here?")] protected bool inUse;
    [Tooltip("The cooldown time to reach to be able to use")] protected float fullCharge;
    [Tooltip("The current cooldown time")] protected float currentCharge;
    [Tooltip("The duration of effects to reach before ending")] private float duration;
    [Tooltip("The current duration of effects")] private float currentDuration;
    [Tooltip("JoeComment")] private bool oppositeDebuffActivated;
    [Tooltip("Amount of One_Time abilities")] private int useCount;

    /// <summary>
    /// Effect to use on use ability use see <see cref="Effects"/>
    /// </summary>
    [Tooltip("Effect to use on use ability use")] private Action<Ability> effectInvokedOnUse;
    /// <summary>
    /// Effect to use on ending of ability see <see cref="Effects"/>
    /// </summary>
    [Tooltip("Effect to use on ending of ability")] private Action<Ability> effectInvokedOnEnd;
    [Space]

    [Header("Component references")]
    [Tooltip("JoeComment")] protected AbilityInventory inventory;
    [Tooltip("Player to not target with traps and debuffs")] private PlayerController castingPlayer;
    [Tooltip("Player to target with debuffs. JoeComment does this also do trapped players?")] private PlayerController targetedPlayer;
    #endregion

    #region SETUP
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

    public Ability Clone()
    {
        return new Ability(name, description, pointsCost, useType, abilityType, duration, effectInvokedOnUse, effectInvokedOnEnd, fullCharge);
    }
    #endregion

    /// <summary>
    /// Manages duration, charge time, passive usage and one time usage
    /// </summary>
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
                        if (abilityType == AbilityType.POWERUP)
                            GameObject.FindObjectOfType<AbilityTimerContainer>().UpdateTimer(name, currentDuration);
                        else
                            UpdateTargetTimer();
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

    private void UpdateTargetTimer()
    {
        GameObject.FindObjectOfType<Effects>().CmdUpdateTargetTimer(targetedPlayer.playerName, name, currentDuration);
    }
    
    /// <summary>
    /// Uses ability, invoke effect and sets animator
    /// </summary>
    public void Use()
    {
        
        AnimatorStateInfo state = castingPlayer.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        Animator animator = castingPlayer.transform.GetChild(0).GetComponent<Animator>();
        switch (abilityType)
        {
            case (AbilityType.POWERUP):
                CreateLocalAbilityEffectTimer(name, duration, false);
                if (useType == AbilityUseTypes.ONE_TIME)
                {
                    //Not sure if this is gonna do anything if its the end of August and still isn't used delete me pls :)
                }
                effectInvokedOnUse.Invoke(this);
                //Reset charge value and set slot charging state
                if (useType == AbilityUseTypes.RECHARGE)
                {
                    currentCharge = 0;
                    GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotChargingState(name, true);
                }
                break;
            case (AbilityType.DEBUFF):
                
                effectInvokedOnUse.Invoke(this);
                if (!state.IsName("Throw") && inUse)
                {
                    GameObject.FindObjectOfType<Effects>().CmdCreateAbilityEffectTimer(name, targetedPlayer.playerName, duration);
                    animator.SetTrigger("Throw");
                    if (useType == AbilityUseTypes.RECHARGE)
                    {
                        currentCharge = 0;
                        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotChargingState(name, true);
                    }
                }
                break;
            case (AbilityType.TRAP):
                if (!state.IsName("PutDown"))
                {
                    animator.SetTrigger("PutDown");
                    effectInvokedOnUse.Invoke(this);
                    //Reset charge value and set slot charging state
                    if (useType == AbilityUseTypes.RECHARGE)
                    {
                        currentCharge = 0;
                        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotChargingState(name, true);
                    }
                }
                useCount++;
                break;
        }
        used = true;
        
    }
    
    private void EndEffect()
    {
        if(effectInvokedOnEnd != null)
            effectInvokedOnEnd.Invoke(this);
        if (abilityType == AbilityType.POWERUP || abilityType == AbilityType.DEBUFF)
        {
            currentDuration = 0;
            inUse = false;
        }
        if (useType == AbilityUseTypes.ONE_TIME)
            castingPlayer.abilityInventory.RemoveAbilityFromInventory(this);
    }

    public static void CreateLocalAbilityEffectTimer(string abilityName, float fullDuration, bool badEffect)
    {
        GameObject.FindObjectOfType<AbilityTimerContainer>().AddTimer(abilityName, fullDuration, badEffect);
    }

    public void ResetUseCount()
    {
        useCount = 0;
    }

    #region GETTERS_AND_SETTERS
    #region GET_BASIC_INFO
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
    public AbilityType GetType()
    {
        return abilityType;
    }
    #endregion

    #region GET_USAGE
    public float GetChargeAmount()
    {
        return fullCharge;
    }
    public float GetCurrentCharge()
    {
        return currentCharge;
    }
    public float GetDuration()
    {
        return duration;
    }

    public float GetCurrentDuration()
    {
        return currentDuration;
    }

    public bool IsInUse()
    {
        return inUse;
    }
    public void SetInUse(bool use)
    {
        inUse = use;
    }
    #endregion

    public bool IsOppositeDebuffActivated()
    {
        return oppositeDebuffActivated;
    }
    public void SetOppositeDebuffActivated(bool activated)
    {
        oppositeDebuffActivated = activated;
    }

    #region SET_GET_REFERENCES
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
    public PlayerController GetTargetedPlayer()
    {
        return targetedPlayer;
    }
    public void SetTargetedPlayer(PlayerController target)
    {
        targetedPlayer = target;
    }
    #endregion


    #endregion

}
