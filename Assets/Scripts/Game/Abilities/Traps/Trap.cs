using System;

public class Trap : Ability
{
    private float proximityTriggerDistance;

    public Trap(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityType, float amountToCharge = 0) 
        : base(abilityName, abilityDescription, abilityCost, abilityType, amountToCharge)
    {

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
