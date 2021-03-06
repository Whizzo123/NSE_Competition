using System;
public class Debuff : Ability
{
    private PlayerController target;

    public Debuff(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityType, float amountToCharge = 0) 
        : base(abilityName, abilityDescription, abilityCost, abilityType, amountToCharge)
    {

    }

    public void SetTarget(PlayerController player)
    {
        target = player;
    }

    public PlayerController GetTarget()
    {
        return target;
    }
}
