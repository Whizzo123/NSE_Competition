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
        BoltNetwork.Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "Invisibility_PA"),
            GetPlayerToEmpower().gameObject.transform.position, Quaternion.identity);
        CmdToggleCamouflage(false);
        inUse = true;
    }

    [Command]
    private void CmdToggleCamouflage(bool toggle)
    {
        RpcToggleCamouflage(toggle);
    }

    [ClientRpc]
    private void RpcToggleCamouflage(bool toggle)
    {
        GetPlayerToEmpower().ToggleMesh(toggle);
    }

    protected override void EndEffect()
    {
        CmdToggleCamouflage(true);
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new Camouflage();
    }

}
