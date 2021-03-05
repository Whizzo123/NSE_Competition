using System;

public class Powerup : Ability
{

    private PlayerController playerToEmpower;

    public Powerup(string abilityName, string abilityDescription, int abilityCost, Action<Ability> onUse, AbilityUseTypes abilityType, float amountToCharge = 0) : base(abilityName, abilityDescription, abilityCost, onUse, abilityType, amountToCharge)
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

    public PlayerController GetPlayerToEmpower()
    {
        return playerToEmpower;
    }

    public void SetPlayerToEmpower(PlayerController player)
    {
        playerToEmpower = player;
    }
}
