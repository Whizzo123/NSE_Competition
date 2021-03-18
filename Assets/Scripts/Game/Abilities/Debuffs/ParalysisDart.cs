using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParalysisDart : Debuff
{

    public ParalysisDart() : base("Paralysis Dart", "Shoot a poison dart capable of turning an enemies hands to lead so they cannot use tools", 3, AbilityUseTypes.ONE_TIME, 20.0f, 10.0f)
    {

    }

    public override void Use()
    {
        if(target == null)
        {
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.SetActive(true);
            PlayerController closestPlayer = FindClosestPlayer();
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(closestPlayer.gameObject);
            target = closestPlayer;
        }
        else
        {
            inUse = true;
            var request = ParalyzePlayerEvent.Create();
            request.Target = castingPlayer.entity;
            request.End = false;
            request.Send();
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
            GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, true);
            base.Use();
        }
    }

    public override void EndEffect()
    {
        inUse = false;
        var request = ParalyzePlayerEvent.Create();
        request.Target = castingPlayer.entity;
        request.End = true;
        request.Send();
        target = null;
        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, false);
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new ParalysisDart();
    }

}

