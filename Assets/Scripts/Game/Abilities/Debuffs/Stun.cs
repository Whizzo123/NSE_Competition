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
    }

    public override Ability Clone()
    {
        return new Stun();
    }
}
