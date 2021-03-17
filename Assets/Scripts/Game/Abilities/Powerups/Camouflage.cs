using System.Collections;
using UnityEngine;

public class Camouflage : Powerup
{

    public Camouflage() : base("Camouflage", "Allows you to blend in with your surroundings for limited amount of time", 4, AbilityUseTypes.RECHARGE, 50.0f, 15.0f)
    {
      
    }


    public override void Use()
    {
        var request = ToggleCamouflage.Create();
        request.Target = GetPlayerToEmpower().entity;
        request.Toggle = false;
        request.Send();
        inUse = true;
    }

    protected override void EndEffect()
    {
        var request = ToggleCamouflage.Create();
        request.Target = GetPlayerToEmpower().entity;
        request.Toggle = true;
        request.Send();
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new Camouflage();
    }

}
