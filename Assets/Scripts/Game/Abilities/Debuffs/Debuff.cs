﻿using System;
using UnityEngine;

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

    public override void UpdateAbility()
    {
        if(useType == AbilityUseTypes.RECHARGE)
        if (inUse)
        {
            if (currentDuration < effectDuration)
            {
                Debug.Log("Debuff in use increasing duration now at: " + currentDuration);
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
    }

    public virtual void EndEffect()
    {
        currentCharge = 0;
        currentDuration = 0;
        inUse = false;
        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotChargingState(name, true);
    }

    protected PlayerController FindClosestPlayer()
    {
        float shortestDistance = float.MaxValue;
        PlayerController closestPlayer = null;

        foreach(BoltEntity entity in BoltNetwork.Entities)
        {
            if (entity.StateIs<IGamePlayerState>() == false || entity == castingPlayer.entity) continue;
            float newDistance = GetDistance(entity.GetComponent<PlayerController>().gameObject.transform.position,
                castingPlayer.gameObject.transform.position);
            if (newDistance < shortestDistance)
            {
                shortestDistance = newDistance;
                closestPlayer = entity.GetComponent<PlayerController>();
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
