using System.Collections;
using UnityEngine;


public class SpeedBoost : Powerup
{

    private float boostToSpeed;

    public SpeedBoost() : base("Speed", "Run really fast", 3, AbilityUseTypes.PASSIVE)
    {
        boostToSpeed = 7.5f;
    }

    public override void Use()
    {
        //All logic here
        GetPlayerToEmpower().speed = boostToSpeed;
        base.Use();
    }


    public override Ability Clone(Ability ability)
    {
        SpeedBoost speedBoost = (SpeedBoost)ability;
        return new SpeedBoost();
    }

}
