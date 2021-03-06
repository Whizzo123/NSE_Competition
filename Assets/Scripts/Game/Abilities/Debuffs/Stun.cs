using System.Collections;
using UnityEngine;


public class Stun : Debuff
{

    public Stun() : base ("Stun", "Stun an opponent of your choosing to change the tides", 1, AbilityUseTypes.RECHARGE, 30.0f, 20.0f)
    {
        
    }

}
