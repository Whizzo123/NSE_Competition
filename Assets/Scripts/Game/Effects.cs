using System.Collections;
using UnityEngine;


public class Effects
{


    public static void SpeedBoost(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
        powerup.GetPlayerToEmpower().speed = 7f;
    }

    public static void SlowDown(Ability ability)
    {
        Debuff debuff = (Debuff)ability;
        debuff.GetTarget();
    }

    public static void BearTrap(Ability ability)
    {
        Trap trap = (Trap)ability;
        trap.GetProximityTriggerDistance();
    }
    
}
