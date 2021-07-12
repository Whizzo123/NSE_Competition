using System;
using UnityEngine;
using Mirror;

public class Debuff : Ability
{
    protected PlayerController target;
    protected float effectDuration;
    protected float currentDuration;
    protected PlayerController castingPlayer;

    public Debuff(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityType, 
        float amountToCharge = 0, float amountToLast = 0, PlayerController castingPlayer = null) 
        : base(abilityName, abilityDescription, abilityCost, abilityType, amountToCharge)
    {
        effectDuration = amountToLast;
        currentDuration = 0;
        this.castingPlayer = castingPlayer;
    }

    public void SetTarget(PlayerController player)
    {
        target = player;
    }

    public PlayerController GetTarget()
    {
        return target;
    }

    public override void Use()
    {
        //Do animation
        AnimatorStateInfo state = castingPlayer.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (!state.IsName("Throw"))
        {
            castingPlayer.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Throw");
        }
        base.Use();
    }
    
    

    public override void UpdateAbility()
    {
        if(useType == AbilityUseTypes.RECHARGE)
        if (inUse)
        {
            if (currentDuration < effectDuration)
            {
                currentDuration += Time.deltaTime;
                if (currentDuration > effectDuration)
                    currentDuration = effectDuration;
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
        else if (useType == AbilityUseTypes.PASSIVE)
        {
            Use();
        }
        else if(useType == AbilityUseTypes.ONE_TIME)
        {
            if(inUse)
            {
                if (currentDuration < effectDuration)
                {
                    currentDuration += Time.deltaTime;
                    if (currentDuration > effectDuration)
                        currentDuration = effectDuration;
                }
                else
                {
                    currentDuration = 0;
                    EndEffect();
                }
            }
        }
    }

    public virtual void EndEffect()
    {
        currentCharge = 0;
        currentDuration = 0;
        inUse = false;
        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotChargingState(name, true);
        castingPlayer.abilityInventory.RemoveAbilityFromInventory(this);
    }

    protected PlayerController FindClosestPlayer()
    {
        //Ask Joe to explain this bit of code
        float shortestDistance = float.MaxValue;
        PlayerController closestPlayer = null;
        foreach(PlayerController player in GameObject.FindObjectsOfType<PlayerController>())
        {
            if (player == castingPlayer) continue;
            float newDistance = GetDistance(player.transform.position,
                castingPlayer.transform.position);
            if (newDistance < shortestDistance)
            {
                shortestDistance = newDistance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }

    private float GetDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    public void SetCastingPlayer(PlayerController player)
    {
        castingPlayer = player;
    }

    public PlayerController GetCastingPlayer()
    {
        return castingPlayer;
    }
}
