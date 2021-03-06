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
        return (Powerup)powerups[name].Clone();
    }

    public Debuff CloneDebuff(string name)
    {
        return (Debuff)debuffs[name].Clone();
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
        ObstacleTrap obstacleTrap = new ObstacleTrap();
        VoodooPoisonTrap voodooTrap = new VoodooPoisonTrap();
        VisionCloudTrap visionTrap = new VisionCloudTrap();
        traps.Add(bearTrap.GetAbilityName(), bearTrap);
        traps.Add(obstacleTrap.GetAbilityName(), obstacleTrap);
        traps.Add(voodooTrap.GetAbilityName(), voodooTrap);
        traps.Add(visionTrap.GetAbilityName(), visionTrap);
    }



}
