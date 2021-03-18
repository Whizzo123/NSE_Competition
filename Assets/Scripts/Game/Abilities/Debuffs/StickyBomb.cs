using System.Collections;
using UnityEngine;


public class StickyBomb : Debuff
{

    public StickyBomb() : base ("StickyBomb", "Stun an opponent of your choosing to change the tides", 1, AbilityUseTypes.ONE_TIME, 30.0f, 20.0f)
    {
        
    }

    public override void Use()
    {
        BoltLog.Info("Inside stun use");
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
            var request = StunEnemyPlayer.Create();
            request.Target = target.entity;
            request.End = false;
            request.Send();
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
            GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, true);
            base.Use();
        }
    }

    public override void EndEffect()
    {
        Debug.Log("Ending sticky effect");
        inUse = false;
        var request = StunEnemyPlayer.Create();
        request.Target = target.entity;
        request.End = true;
        request.Send();
        target = null;
        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, false);
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new StickyBomb();
    }
}
