using System.Collections;
using UnityEngine;


public class BearTrap : Trap
{

    public BearTrap() : base ("Bear Trap", "Ensnare your opponents in a bear trap to immobilize them", 3, AbilityUseTypes.ONE_TIME, 10.0f)
    {

    }


    public override void Use()
    {
        //Place down trap

        Vector3 spawnPos = placingPlayer.transform.position;
        BoltLog.Info("Spawn pos: " + spawnPos);
        BoltNetwork.Instantiate(BoltPrefabs.BearTrap, spawnPos, Quaternion.identity).GetState<IBearTrap>().PlacingPlayer = placingPlayer.entity;
        base.Use();
    }

    public override Ability Clone()
    {
        return new BearTrap();
    }

}
