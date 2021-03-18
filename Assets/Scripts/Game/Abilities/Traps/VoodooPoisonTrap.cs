using System.Collections;
using UnityEngine;


public class VoodooPoisonTrap : Trap
{

    private LayerMask ground = LayerMask.GetMask("ground", "DesertGround", "GrassGround", "SnowGround", "SwampGround", "JungleGround", "SwampWater");

    public VoodooPoisonTrap() :base ("Voodoo Poison Trap", "Hits enemy with voodoo poison effect hindering their movement", 3, AbilityUseTypes.ONE_TIME)
    {

    }


    public override void Use()
    {
        RaycastHit hit;
        Vector3 spawnPos = Vector3.zero;
        if(Physics.Raycast(placingPlayer.transform.position, Vector3.down, out hit, float.MaxValue, ground))
        {
            spawnPos = placingPlayer.transform.position;
            BoltNetwork.Instantiate(BoltPrefabs.VoodooPoisonTrap, spawnPos, Quaternion.identity).GetState<IVoodooPoisonTrap>().PlacingPlayer = placingPlayer.entity;
            base.Use();
        }        
    }

    public override Ability Clone()
    {
        return new VoodooPoisonTrap();
    }
}
