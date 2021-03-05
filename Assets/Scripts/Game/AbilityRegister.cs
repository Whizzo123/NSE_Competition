using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class AbilityRegister
{
    private Dictionary<string, Powerup> powerups;
    private Dictionary<string, Debuff> debuffs;
    private Dictionary<string, Trap> traps;

    public void Init()
    {
        RegisterPowerups();
        RegisterDebuffs();
        RegisterTraps();
    }

    public Powerup ClonePowerup(string name)
    {
        Powerup powerup = powerups[name];
        return new Powerup(powerup.GetAbilityName(), powerup.GetAbilityDescription(), powerup.GetAbilityCost(), powerup.GetEffect(), powerup.GetUseType(), powerup.GetChargeAmount());
    }

    public Debuff CloneDebuff(string name)
    {
        Debuff debuff = debuffs[name];
        return new Debuff(debuff.GetAbilityName(), debuff.GetAbilityDescription(), debuff.GetAbilityCost(), debuff.GetEffect(), debuff.GetUseType(), debuff.GetChargeAmount());
    }

    public Trap CloneTrap(string name)
    {
        Trap trap = traps[name];
        return new Trap(trap.GetAbilityName(), trap.GetAbilityDescription(), trap.GetAbilityCost(), trap.GetEffect(), trap.GetUseType(), trap.GetChargeAmount());
    }

    private void RegisterPowerups()
    {
        powerups = new Dictionary<string, Powerup>();
        Powerup speedBoost = new Powerup("Speed", "Run really fast", 3, Effects.SpeedBoost, AbilityUseTypes.PASSIVE);
        powerups.Add(speedBoost.GetAbilityName(), speedBoost);
    }

    private void RegisterDebuffs()
    {
        debuffs = new Dictionary<string, Debuff>();
        Debuff slowdown = new Debuff("Slow Down", "Slow down enemy player of your choice", 2, Effects.SlowDown, AbilityUseTypes.RECHARGE, 5);
        debuffs.Add(slowdown.GetAbilityName(), slowdown);
    }

    private void RegisterTraps()
    {
        traps = new Dictionary<string, Trap>();
        Trap bearTrap = new Trap("BearTrap", "Ensnare your opponents in a bear trap to immobilize them", 3, Effects.BearTrap, AbilityUseTypes.ONE_TIME);
        traps.Add(bearTrap.GetAbilityName(), bearTrap);
    }



}
