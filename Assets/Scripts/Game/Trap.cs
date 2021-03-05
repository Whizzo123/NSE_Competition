using System;

public class Trap : Ability
{
    private float proximityTriggerDistance;

    public Trap(string abilityName, string abilityDescription, int abilityCost, Action<Ability> onUse, AbilityUseTypes abilityType, float amountToCharge = 0) : base(abilityName, abilityDescription, abilityCost, onUse, abilityType, amountToCharge)
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

    public float GetProximityTriggerDistance()
    {
        return proximityTriggerDistance;
    }

    public void SetProximityTriggerDistance(float triggerDistance)
    {
        proximityTriggerDistance = triggerDistance;
    }

}
