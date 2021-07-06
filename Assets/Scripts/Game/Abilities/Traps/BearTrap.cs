using System.Collections;
using UnityEngine;
using Unity;
using Mirror;


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
        Debug.Log("Spawn pos: " + spawnPos);
        placingPlayer.CmdSpawnBearTrap(spawnPos, placingPlayer);
        base.Use();
        GameObject.FindObjectOfType<AudioManager>().PlaySound("BearTrapOpening");
    }

    public override Ability Clone()
    {
        return new BearTrap();
    }

}
