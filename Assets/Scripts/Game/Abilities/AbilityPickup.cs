using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class AbilityPickup : EntityBehaviour<IAbilityPickup>
{

    public string abilityName;
    public bool enabledForPickup = false;
    private Quaternion rotation = Quaternion.Euler(45, 45, 0);
    public void FixedUpdate()
    {
        rotation *= Quaternion.Euler(2, 2, 2);
        this.gameObject.transform.rotation = rotation;
    }

    public override void Attached()
    {
        BoltLog.Info("Attached the ability pickup dude");
        if (entity.gameObject == null)
            BoltLog.Error("Issue no game object attached to this entity");
    }


    public void SetAbilityOnPickup(string abilityName)
    {
        this.abilityName = abilityName;    //This line was just for testing right?
        state.AbilityName = abilityName;
    }

    public void PickupAbility(PlayerController player)
    {
        if (enabledForPickup)
        {
            player.abilityInventory.AddAbilityToInventory(FindObjectOfType<AbilityRegister>().Clone(abilityName));
            Destroy(this);
        }

        
        //Send off event to get rid of this thing
        var request = AbilityPickupDisable.Create();
        request.AbilityEntity = this.entity;
        request.Send();
    }


}
