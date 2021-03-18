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
        return (Trap)traps[name].Clone();
    }
    private void RegisterPowerups()
    {
        powerups = new Dictionary<string, Powerup>();
        SpeedBoost speedBoost = new SpeedBoost();
        PlayerTracker playerTracker = new PlayerTracker();
        Camouflage camouflage = new Camouflage();
        ClueInterpretator clueInterpretator = new ClueInterpretator();
        powerups.Add(speedBoost.GetAbilityName(), speedBoost);
        powerups.Add(playerTracker.GetAbilityName(), playerTracker);
        powerups.Add(camouflage.GetAbilityName(), camouflage);
        powerups.Add(clueInterpretator.GetAbilityName(), clueInterpretator);
    }

    private void RegisterDebuffs()
    {
        debuffs = new Dictionary<string, Debuff>();
        StickyBomb stunDebuff = new StickyBomb();
        ParalysisDart paralysisDart = new ParalysisDart();
        MortalSpell mortalSpell = new MortalSpell();
        debuffs.Add(stunDebuff.GetAbilityName(), stunDebuff);
        debuffs.Add(paralysisDart.GetAbilityName(), paralysisDart);
        debuffs.Add(mortalSpell.GetAbilityName(), mortalSpell);
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
