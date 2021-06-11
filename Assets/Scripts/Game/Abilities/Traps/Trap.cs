using System;

public class Trap : Ability
{
    protected PlayerController placingPlayer;
    private int useCount;

    public Trap(string abilityName, string abilityDescription, int abilityCost, AbilityUseTypes abilityType, float amountToCharge = 0) 
        : base(abilityName, abilityDescription, abilityCost, abilityType, amountToCharge)
    {
        useCount = 0;
    }

    public override void Use()
    {
        //Do animaton
        var request = FireAnimatorPutDownTrigger.Create();
        request.Target = placingPlayer.entity;
        request.Send();
        base.Use();
        useCount++;
    }

    public override void UpdateAbility()
    {
        if(used && useCount >= 3)
        {
            //Destroy from inventory
            placingPlayer.abilityInventory.RemoveAbilityFromInventory(this);
        }
    }

    public void SetPlacingPlayer(PlayerController player)
    {
        placingPlayer = player;
    }

    public void ResetUseCount()
    {
        useCount = 0;
    }

}
