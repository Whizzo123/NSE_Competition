using System.Collections;
using UnityEngine;


public class SpeedBoost : Powerup
{

    private float boostToSpeed;

    public SpeedBoost() : base("Speed", "Run really fast", 3, AbilityUseTypes.RECHARGE, 20.0f, 10.0f)
    {
        boostToSpeed = 7.5f;
    }

    public override void Use()
    {
        //All logic here
        if (GetPlayerToEmpower().speed != boostToSpeed && oppositeDebuffActivated == false)
        {
            GetPlayerToEmpower().speed = GetPlayerToEmpower().normalSpeed + boostToSpeed;
            inUse = true;
        }
        base.Use();
    }

    protected override void EndEffect()
    {
        GetPlayerToEmpower().speed = GetPlayerToEmpower().normalSpeed;
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new SpeedBoost();
    }

}
