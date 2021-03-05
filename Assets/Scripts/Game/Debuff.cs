using System;
public class Debuff : Ability
{
    private PlayerController target;

    public Debuff(string abilityName, string abilityDescription, int abilityCost, Action<Ability> onUse, AbilityUseTypes abilityType, float amountToCharge = 0) : base(abilityName, abilityDescription, abilityCost, onUse, abilityType, amountToCharge)
    {

    }

    public override void Use(Ability ability)
    {
        if (useType == AbilityUseTypes.RECHARGE)
        {
            if (currentCharge == fullCharge)
            {
                base.Use(ability);
            }
        }
        else
        {
            base.Use(ability);
        }
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
