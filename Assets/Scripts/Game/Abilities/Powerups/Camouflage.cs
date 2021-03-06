using System.Collections;
using UnityEngine;

public class Camouflage : Powerup
{

    public Camouflage() : base("Camouflage", "Allows you to blend in with your surroundings for limited amount of time", 4, AbilityUseTypes.RECHARGE, 50.0f, 30.0f)
    {
      
    }

    public override Ability Clone()
    {
        return new Camouflage();
    }

}
