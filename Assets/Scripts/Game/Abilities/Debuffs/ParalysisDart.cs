using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParalysisDart : Debuff
{

    public ParalysisDart() : base("Paralysis Dart", "Shoot a poison dart capable of turning an enemies hands to lead so they cannot use tools", 3, AbilityUseTypes.RECHARGE, 20.0f, 10.0f)
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
            Cast(true);
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
            GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, true);
            base.Use();
        }
    }

    private void Cast(bool toggle)
    {
        target.CmdSetImmobilized(true);
    }

    public override void EndEffect()
    {
        inUse = false;
        Cast(false);
        target = null;
        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, false);
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new ParalysisDart();
    }

}

