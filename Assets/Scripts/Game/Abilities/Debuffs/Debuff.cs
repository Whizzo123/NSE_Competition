using System;
public class Debuff : Ability
{
    private PlayerController target;
    protected float effectDuration;
    protected float currentDuration;

    public Debuff(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityType, float amountToCharge = 0, float amountToLast = 0) 
        : base(abilityName, abilityDescription, abilityCost, abilityType, amountToCharge)
    {
        effectDuration = amountToLast;
        currentDuration = 0;
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
