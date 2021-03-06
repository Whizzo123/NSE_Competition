using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class AbilityRegister : MonoBehaviour
{
    private Dictionary<string, Powerup> powerups;
    private Dictionary<string, Debuff> debuffs;
    private Dictionary<string, Trap> traps;

    public void Initialize()
    {
        RegisterPowerups();
        RegisterDebuffs();
        RegisterTraps();
    }

    public List<Ability> GetLoadoutList()
    {
        List<Ability> loadoutList = new List<Ability>();

        foreach (Powerup powerup in powerups.Values)
        {
            loadoutList.Add(powerup);
        }
        foreach (Debuff debuff in debuffs.Values)
        {
            loadoutList.Add(debuff);
        }
        foreach (Trap trap in traps.Values)
        {
            loadoutList.Add(trap);
        }

        return loadoutList;
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
        Camouflage camouflage = new Camouflage();
        //Powerup clueInterpretation = new Powerup("Interpretation", "Get occasional access to clues of where the closest rarest artefact is", 5, Effects.CreateClueForPlayer, AbilityUseTypes.RECHARGE, 40.0f);
        //Powerup largerTools = new Powerup("Larger Tools", "Magically makes your tools bigger allowing you to clear areas quicker", 5, Effects.EnlargeTools, AbilityUseTypes.RECHARGE, 50.0f);*/
        powerups.Add(speedBoost.GetAbilityName(), speedBoost);
        powerups.Add(playerTracker.GetAbilityName(), playerTracker);
        powerups.Add(camouflage.GetAbilityName(), camouflage);
    }

    private void RegisterDebuffs()
    {
        debuffs = new Dictionary<string, Debuff>();
        //Debuff slowdown = new Debuff("Slow Down", "Slow down enemy player of your choice", 2, Effects.SlowDown, AbilityUseTypes.RECHARGE, 5);
        Stun stunDebuff = new Stun();
        // debuffs.Add(slowdown.GetAbilityName(), slowdown);
        debuffs.Add(stunDebuff.GetAbilityName(), stunDebuff);
    }

    private void RegisterTraps()
    {
        traps = new Dictionary<string, Trap>();
        BearTrap bearTrap = new BearTrap();
        //Trap obstacleTrap = new Trap("Obstacle Surprise", "Spawns obstacles to delay enemy players", 3, Effects.ObstacleTrap, AbilityUseTypes.ONE_TIME);
        //Trap voodooPoisonTrap = new Trap("Voodoo Poision Trap", "Hits enemy with voodoo poison effect hindering their movement", 3, Effects.VoodooPoision, AbilityUseTypes.ONE_TIME);
        //Trap visionCloudTrap = new Trap("Vision Cloud", "Hinders your opponents by shrinking their visual field", 3, Effects.VisionClouding, AbilityUseTypes.ONE_TIME);
        traps.Add(bearTrap.GetAbilityName(), bearTrap);
    }



}
