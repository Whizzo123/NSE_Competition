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
        /*if (GetPlayerToEmpower().state.Speed != boostToSpeed && oppositeDebuffActivated == false)
        {
            BoltLog.Info("Setting speed");
            GetPlayerToEmpower().state.Speed = GetPlayerToEmpower().speed + boostToSpeed;
            inUse = true;
        }*/
        base.Use();
    }

    protected override void EndEffect()
    {
        //GetPlayerToEmpower().state.Speed = GetPlayerToEmpower().speed;
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new SpeedBoost();
    }

}
