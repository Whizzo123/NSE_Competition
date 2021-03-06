using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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

    public static void ObstacleTrap(Ability ability)
    {
        Trap trap = (Trap)ability;
    }

    public static void VoodooPoision(Ability ability)
    {
        Trap trap = (Trap)ability;
    }

    public static void VisionClouding(Ability ability)
    {
        Trap trap = (Trap)ability;
    }

    public static void ActivatePlayerTracker(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
        //Sort out player tracker
        //powerup.UpdatePowerup();
    }

    public static void OnActivatePlayerTrackerEnd(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
        //Close player tracker so we can being to recharge it
    }

    public static void CreateClueForPlayer(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
    }

    public static void OnCreateClueForPlayerEnd(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
        //Stop particle effect or whatever that shows clue
    }

    public static void EnableCamouflage(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
    }

    public static void OnEnableCamouflageEnd(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
        //Stop making player invisible
    }

    public static void EnlargeTools(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
    }

    public static void OnEnlargeToolsEnd(Ability ability)
    {
        Powerup powerup = (Powerup)ability;
        //Shrink tools back down again
    }
    
}
