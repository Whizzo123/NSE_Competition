using System.Collections;
using UnityEngine;
using Bolt;

public class AbilityPickup : EntityBehaviour<IAbilityPickup>
{

    private string abilityName;

    public override void Attached()
    {
        BoltLog.Info("Attached the ability pickup dude");
        if (entity.gameObject == null)
            BoltLog.Error("Issuse no game object attached to this entity");
    }

    public void SetAbilityOnPickup(string abilityName)
    {
        this.abilityName = abilityName;
        state.AbilityName = abilityName;
    }

    public void PickupAbility(PlayerController player)
    {
        player.abilityInventory.AddAbilityToInventory(FindObjectOfType<AbilityRegister>().Clone(state.AbilityName));

        //Send off event to get rid of this thing
        var request = AbilityPickupDisable.Create();
        request.AbilityEntity = this.entity;
        request.Send();
    }


}
