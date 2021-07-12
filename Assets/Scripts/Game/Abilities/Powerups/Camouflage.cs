using System.Collections;
using UnityEngine;
using Mirror;

public class Camouflage : Powerup
{

    public Camouflage() : base("Camouflage", "Allows you to blend in with your surroundings for limited amount of time", 4, AbilityUseTypes.RECHARGE, 50.0f, 15.0f)
    {
      
    }


    public override void Use()
    {
        Vector3 spawnPos = GetPlayerToEmpower().gameObject.transform.position;
        GetPlayerToEmpower().CmdSpawnCamouflageParticles(spawnPos);
        GetPlayerToEmpower().CmdToggleCamouflage(false, GetPlayerToEmpower());
        inUse = true;
    }

    protected override void EndEffect()
    {
        GetPlayerToEmpower().CmdToggleCamouflage(true, GetPlayerToEmpower());
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new Camouflage();
    }

}
