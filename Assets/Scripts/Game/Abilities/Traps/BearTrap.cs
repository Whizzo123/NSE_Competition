using System.Collections;
using UnityEngine;


public class BearTrap : Trap
{

    public BearTrap() : base ("Bear Trap", "Ensnare your opponents in a bear trap to immobilize them", 3, AbilityUseTypes.ONE_TIME, 7.0f)
    {

    }


    public override void Use()
    {
        //Trap Position Calculation
        Vector3 spawnPos = placingPlayer.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, 10))
        {
            spawnPos = hit.point;
        }

        //Spawn and sound
        BoltLog.Info("Spawn pos: " + spawnPos);
        //BoltNetwork.Instantiate(BoltPrefabs.BearTrap, spawnPos, Quaternion.identity).GetState<IBearTrap>().PlacingPlayer = placingPlayer.entity;
        base.Use();
        GameObject.FindObjectOfType<AudioManager>().PlaySound("BearTrapOpening");
    }

    public override Ability Clone()
    {
        return new BearTrap();
    }

}
