using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class AbilityRegister : MonoBehaviour
{
    //private Dictionary<string, Powerup> powerups;
    //private Dictionary<string, Debuff> debuffs;
    //private Dictionary<string, Trap> traps;

    private Dictionary<string, Ability> abilities;

    public void Initialize()
    {
        //RegisterPowerups();
        //RegisterDebuffs();
        //RegisterTraps();
        Register();
    }

    //public List<Ability> GetLoadoutList()
    //{
    //    List<Ability> loadoutList = new List<Ability>();

    //    foreach (Powerup powerup in powerups.Values)
    //    {
    //        loadoutList.Add(powerup);
    //    }
    //    foreach (Debuff debuff in debuffs.Values)
    //    {
    //        loadoutList.Add(debuff);
    //    }
    //    foreach (Trap trap in traps.Values)
    //    {
    //        loadoutList.Add(trap);
    //    }

    //    return loadoutList;
    //}

    public List<Ability> GetLoadoutList()
    {
        return abilities.Values.ToList<Ability>();
    }

    //public string[] GetTrapList()
    //{
    //    return traps.Keys.ToArray<string>();
    //}

    public string[] GetTrapList()
    {
        List<string> trapList = new List<string>();
        foreach (string key in abilities.Keys)
        {
            if (abilities[key].GetType() == AbilityType.TRAP)
                trapList.Add(key);
        }
        return trapList.ToArray<string>();
    }

    //public Ability Clone(string name)
    //{
    //    if(powerups.ContainsKey(name))
    //    {
    //        return ClonePowerup(name);
    //    }
    //    else if(debuffs.ContainsKey(name))
    //    {
    //        return CloneDebuff(name);
    //    }
    //    else if(traps.ContainsKey(name))
    //    {
    //        return CloneTrap(name);
    //    }
    //    Debug.LogError("Couldn't find ability type to clone");
    //    return null;
    //}

    //public Powerup ClonePowerup(string name)
    //{
    //    return (Powerup)powerups[name].Clone();
    //}

    //public Debuff CloneDebuff(string name)
    //{
    //    return (Debuff)debuffs[name].Clone();
    //}

    //public Trap CloneTrap(string name)
    //{
    //    Trap trap = traps[name];
    //    return (Trap)traps[name].Clone();
    //}

    public Ability Clone(string name)
    {
        return abilities[name].Clone();
    }

    private void Register()
    {
        //powerups = new Dictionary<string, Powerup>();
        /*SpeedBoost speedBoost = new SpeedBoost();
        PlayerTracker playerTracker = new PlayerTracker();
        Camouflage camouflage = new Camouflage();
        ClueInterpretator clueInterpretator = new ClueInterpretator();
        powerups.Add(speedBoost.GetAbilityName(), speedBoost);
        powerups.Add(playerTracker.GetAbilityName(), playerTracker);
        powerups.Add(camouflage.GetAbilityName(), camouflage);
        powerups.Add(clueInterpretator.GetAbilityName(), clueInterpretator);*/
        abilities = new Dictionary<string, Ability>();
        Ability speedBoost = new Ability("Speed", "Run really fast", 3, AbilityUseTypes.RECHARGE, AbilityType.POWERUP, 10.0f, Effects.SpeedBoost,
            Effects.EndSpeedBoost, 20.0f);
        Ability camouflage = new Ability("Camouflage", "Allows you to blend in with your surroundings for limited amount of time", 4, AbilityUseTypes.RECHARGE,
            AbilityType.POWERUP, 15.0f, Effects.ActivateCamouflage, Effects.DeactivateCamouflage, 50.0f);
        Ability clueInterpretator = new Ability("Clue Interpretator", "Gain insight to where your nearby artefacts works", 5, AbilityUseTypes.RECHARGE,
            AbilityType.POWERUP, 10.0f, Effects.ActivateClueInterpretator, Effects.DeactivateClueInterpretator, 20.0f);
        Ability playerTracker = new Ability("PlayerTracker", "Track other players on the map", 5, AbilityUseTypes.RECHARGE, AbilityType.POWERUP, 25.0f,
            Effects.ActivatePlayerTracker, Effects.DeactivatePlayerTracker, 30.0f);
        Ability bearTrap = new Ability("Bear Trap", "Ensnare your opponents in a bear trap to immobilize them", 3, AbilityUseTypes.ONE_TIME, AbilityType.TRAP,
            0.0f, Effects.SpringBearTrap, null, 7.0f);
        Ability voodooTrap = new Ability("Voodoo Poison Trap", "Hits enemy with voodoo poison effect hindering their movement", 3, AbilityUseTypes.ONE_TIME,
            AbilityType.TRAP, 0.0f, Effects.SpringVoodooTrap, null, 7.0f);
        Ability stickyBomb = new Ability("StickyBomb", "Stun an opponent of your choosing to change the tides", 1, AbilityUseTypes.RECHARGE, AbilityType.DEBUFF,
            20.0f, Effects.ThrowStickyBomb, Effects.EndStickyBombEffect, 30.0f);
        Ability mortalSpell = new Ability("Mortal Spell", "Brings an enemy back down to the mortal plane all abilities are stripped for 10 secs", 3, AbilityUseTypes.RECHARGE,
            AbilityType.DEBUFF, 20.0f, Effects.CastMortalSpell, Effects.EndMortalSpell, 10.0f);
        Ability paralysisDart = new Ability("Paralysis Dart", "Shoot a poison dart capable of turning an enemies hands to lead so they cannot use tools", 3, AbilityUseTypes.RECHARGE,
            AbilityType.DEBUFF, 10.0f, Effects.ThrowParalysisDart, Effects.EndParalysisDartEffect, 20.0f);
        abilities.Add(speedBoost.GetAbilityName(), speedBoost);
        abilities.Add(camouflage.GetAbilityName(), camouflage);
        abilities.Add(clueInterpretator.GetAbilityName(), clueInterpretator);
        abilities.Add(playerTracker.GetAbilityName(), playerTracker);
        abilities.Add(stickyBomb.GetAbilityName(), stickyBomb);
        abilities.Add(mortalSpell.GetAbilityName(), mortalSpell);
        abilities.Add(paralysisDart.GetAbilityName(), paralysisDart);
        abilities.Add(bearTrap.GetAbilityName(), bearTrap);
        abilities.Add(voodooTrap.GetAbilityName(), voodooTrap);
    }

    //private void RegisterDebuffs()
    //{
    //    debuffs = new Dictionary<string, Debuff>();
    //    StickyBomb stunDebuff = new StickyBomb();
    //    ParalysisDart paralysisDart = new ParalysisDart();
    //    MortalSpell mortalSpell = new MortalSpell();
    //    debuffs.Add(stunDebuff.GetAbilityName(), stunDebuff);
    //    debuffs.Add(paralysisDart.GetAbilityName(), paralysisDart);
    //    debuffs.Add(mortalSpell.GetAbilityName(), mortalSpell);
    //}

    //private void RegisterTraps()
    //{
    //    traps = new Dictionary<string, Trap>();
    //    BearTrap bearTrap = new BearTrap();
    //    VoodooPoisonTrap voodooTrap = new VoodooPoisonTrap();
    //    traps.Add(bearTrap.GetAbilityName(), bearTrap);
    //    traps.Add(voodooTrap.GetAbilityName(), voodooTrap); 
    //}



}
