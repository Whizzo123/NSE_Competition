using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class AbilityRegister : MonoBehaviour
{

    [Tooltip("Abilities available")]private Dictionary<string, Ability> abilities;

    /// <summary>
    /// Call: Registers all abilites
    /// </summary>
    public void Initialize()
    {
        Register();
    }


    /// <summary>
    /// Returns all abilities
    /// </summary>
    public List<Ability> GetLoadoutList()
    {
        return abilities.Values.ToList<Ability>();
    }

    /// <summary>
    /// Returns name of all traps
    /// </summary>
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

    public Ability Clone(string name)
    {
        return abilities[name].Clone();
    }

    /// <summary>
    /// Sets up all abilities
    /// </summary>
    private void Register()
    {
        //Initialize abilities dictionary
        abilities = new Dictionary<string, Ability>();
        //Powerups
        Ability speedBoost = new Ability("Speed", "Run really fast", 3, AbilityUseTypes.RECHARGE, AbilityType.POWERUP, 10.0f, Effects.SpeedBoost,
            Effects.EndSpeedBoost, 20.0f);
        Ability camouflage = new Ability("Camouflage", "Allows you to blend in with your surroundings for limited amount of time", 4, AbilityUseTypes.RECHARGE,
            AbilityType.POWERUP, 15.0f, Effects.ActivateCamouflage, Effects.DeactivateCamouflage, 50.0f);
        Ability clueInterpretator = new Ability("Clue Interpretator", "Gain insight to where your nearby artefacts are", 5, AbilityUseTypes.RECHARGE,
            AbilityType.POWERUP, 10.0f, Effects.ActivateClueInterpretator, Effects.DeactivateClueInterpretator, 30.0f);
        Ability playerTracker = new Ability("PlayerTracker", "Locate the hunter with the most artefacts in their possession", 5, AbilityUseTypes.RECHARGE, AbilityType.POWERUP, 25.0f,
            Effects.ActivatePlayerTracker, Effects.DeactivatePlayerTracker, 30.0f);
        //Traps
        Ability bearTrap = new Ability("Bear Trap", "Ensare other hunters for a short time", 3, AbilityUseTypes.ONE_TIME, AbilityType.TRAP,
            0.0f, Effects.SpringBearTrap, null, 7.0f);
        Ability voodooTrap = new Ability("Voodoo Poison Trap", "Leave a noxious gas for other hunters and watch them fumble", 3, AbilityUseTypes.ONE_TIME,
            AbilityType.TRAP, 0.0f, Effects.SpringVoodooTrap, null, 7.0f);
        //Debuffs
        Ability stickyBomb = new Ability("StickyBomb", "Slow a nearby hunter with your nasty concoction", 1, AbilityUseTypes.RECHARGE, AbilityType.DEBUFF,
            20.0f, Effects.ThrowStickyBomb, Effects.EndStickyBombEffect, 30.0f);
        Ability mortalSpell = new Ability("Mortal Spell", "Strip a hunter of their god given powers for a short time", 3, AbilityUseTypes.RECHARGE,
            AbilityType.DEBUFF, 20.0f, Effects.CastMortalSpell, Effects.EndMortalSpell, 10.0f);
        Ability paralysisDart = new Ability("Paralysis Dart", "Paralyse a hunters hand, stop them in their tracks", 3, AbilityUseTypes.RECHARGE,
            AbilityType.DEBUFF, 10.0f, Effects.ThrowParalysisDart, Effects.EndParalysisDartEffect, 20.0f);
        //Add these abilities into the dictionary
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
}
