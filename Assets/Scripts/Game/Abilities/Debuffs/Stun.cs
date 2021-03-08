using System.Collections;
using UnityEngine;


public class Stun : Debuff
{

    public Stun() : base ("Stun", "Stun an opponent of your choosing to change the tides", 1, AbilityUseTypes.RECHARGE, 30.0f, 20.0f)
    {
        
    }

    public override void Use()
    {
        BoltLog.Info("Inside stun use");
        if(target == null)
        {
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.SetActive(true);
            PlayerController closestPlayer = FindClosestPlayer();
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().targetObject = closestPlayer.gameObject;
            target = closestPlayer;
        }
        else
        {
            inUse = true;
            var request = StunEnemyPlayer.Create();
            request.Target = target.entity;
            request.End = false;
            request.Send();
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().targetObject = null;
        }
    }

    public override void EndEffect()
    {
        inUse = false;
        var request = StunEnemyPlayer.Create();
        request.Target = target.entity;
        request.End = true;
        request.Send();
    }

    public override Ability Clone()
    {
        return new Stun();
    }
}
