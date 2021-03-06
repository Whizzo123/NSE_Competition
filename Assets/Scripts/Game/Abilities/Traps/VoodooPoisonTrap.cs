using System.Collections;
using UnityEngine;


public class VoodooPoisonTrap : Trap
{

    public VoodooPoisonTrap() :base ("Voodoo Poison Trap", "Hits enemy with voodoo poison effect hindering their movement", 3, AbilityUseTypes.ONE_TIME)
    {

    }


    public override Ability Clone()
    {
        return new VoodooPoisonTrap();
    }
}
