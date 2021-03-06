using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class AbilityRegister : MonoBehaviour
{
    private Dictionary<string, Powerup> powerups;
    private Dictionary<string, Debuff> debuffs;
    private Dictionary<string, Trap> traps;

    public void Start()
    {
        RegisterPowerups();
        RegisterDebuffs();
        RegisterTraps();
    }

    public Ability Clone(string name)
    {
        if(powerups.ContainsKey(name))
        {
            return ClonePowerup(name);
        }
        else if(debuffs.ContainsKey(name))
        {
            return CloneDebuff(name);
        }
        else if(traps.ContainsKey(name))
        {
            return CloneTrap(name);
        }
        Debug.LogError("Couldn't find ability type to clone");
        return null;
    }

    public Powerup ClonePowerup(string name)
    {
        Powerup powerup = powerups[name];
        return new Powerup(powerup.GetAbilityName(), powerup.GetAbilityDescription(), powerup.GetAbilityCost(), powerup.GetUseType(), powerup.GetChargeAmount());
    }

    public Debuff CloneDebuff(string name)
    {
        Debuff debuff = debuffs[name];
        return new Debuff(debuff.GetAbilityName(), debuff.GetAbilityDescription(), debuff.GetAbilityCost(), debuff.GetUseType(), debuff.GetChargeAmount());
    }

    public Trap CloneTrap(string name)
    {
        Trap trap = traps[name];
        return new Trap(trap.GetAbilityName(), trap.GetAbilityDescription(), trap.GetAbilityCost(), trap.GetUseType(), trap.GetChargeAmount());
    }

    private void RegisterPowerups()
    {
        powerups = new Dictionary<string, Powerup>();
        SpeedBoost speedBoost = new SpeedBoost();
        PlayerTracker playerTracker = new PlayerTracker();
        /*Powerup playerTracker = new Powerup("Player Tracker", "Track player whereabouts on map", 5, Effects.ActivatePlayerTracker, AbilityUseTypes.RECHARGE, 30.0f, Effects.OnActivatePlayerTrackerEnd 50.0f);
        Powerup clueInterpretation = new Powerup("Interpretation", "Get occasional access to clues of where the closest rarest artefact is", 5, Effects.CreateClueForPlayer, AbilityUseTypes.RECHARGE, 40.0f);
        Powerup camouflage = new Powerup("Camouflage", "Allows you to blend in with your surroundings for limited amount of time", 4, Effects.EnableCamouflage, AbilityUseTypes.RECHARGE, 50.0f);
        Powerup largerTools = new Powerup("Larger Tools", "Magically makes your tools bigger allowing you to clear areas quicker", 5, Effects.EnlargeTools, AbilityUseTypes.RECHARGE, 50.0f);*/
        powerups.Add(speedBoost.GetAbilityName(), speedBoost);
    }

    private void RegisterDebuffs()
    {
        debuffs = new Dictionary<string, Debuff>();
        //Debuff slowdown = new Debuff("Slow Down", "Slow down enemy player of your choice", 2, Effects.SlowDown, AbilityUseTypes.RECHARGE, 5);
       // debuffs.Add(slowdown.GetAbilityName(), slowdown);
    }

    private void RegisterTraps()
    {
        traps = new Dictionary<string, Trap>();
        //Trap bearTrap = new Trap("Bear Trap", "Ensnare your opponents in a bear trap to immobilize them", 3, Effects.BearTrap, AbilityUseTypes.ONE_TIME);
        //Trap obstacleTrap = new Trap("Obstacle Surprise", "Spawns obstacles to delay enemy players", 3, Effects.ObstacleTrap, AbilityUseTypes.ONE_TIME);
        //Trap voodooPoisonTrap = new Trap("Voodoo Poision Trap", "Hits enemy with voodoo poison effect hindering their movement", 3, Effects.VoodooPoision, AbilityUseTypes.ONE_TIME);
        //Trap visionCloudTrap = new Trap("Vision Cloud", "Hinders your opponents by shrinking their visual field", 3, Effects.VisionClouding, AbilityUseTypes.ONE_TIME);
        //traps.Add(bearTrap.GetAbilityName(), bearTrap);
    }



}
