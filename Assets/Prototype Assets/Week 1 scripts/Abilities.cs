using UnityEngine;

public class Abilities
{
    ///<summary>Name of the ability, must be unique</summary>
    protected string name;
    /// <summary> Description for player on what ability does</summary>
    protected string description;
    ///<summary>The amount of points this ability consume in the loadouts screen</summary>
    protected int loadoutPoints;

    //Is there a better way to choose this. Problem: I want the inherited classes to be able to choose whether they are a passive, recharge or charge ability.
    //This should then generate or remove functionality, for example:
    //Passive abilities won't have charges or an icon to select for usage.
    //Recharge abilities, will have a varibale in which it shows how long it takes to fully charge the ability aka a cooldown.
    //Charge abilities will have an amount variable,
    //This could be a class however I am unsure if that would be wise, I am orginally planning to have this heirarchy:
    // Abilities -> 4 branches of ability (powerups, debuffs, traps, skills) -> Each individual ability
    //although the 4 branches of abilities might be arbitrary, the only difference is that traps can be placed in game. A better heirarchy might be:
    //Abilites -> 3 types(Recharge over time, Charges, Passive) -> individual abilities
    //This would allow very different class variables but.
    /// <summary>
    /// Abilities must be either an active ability: which either recharge over time uses or have limited uses where more uses can be picked up on the field, 
    /// or they will be a passive ability: always active
    /// </summary>
    public enum type : byte
    {
        Cooldown,
        Charge,
        Passive,
    }

    protected struct types
    {
        public void ROT()
        {

        }
    }
   
}
