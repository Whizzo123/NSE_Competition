using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityPickup : NetworkBehaviour
{

    public string abilityName;
    public bool enabledForPickup = true;
    private Quaternion rotation = Quaternion.Euler(45, 45, 0);
    public void FixedUpdate()
    {
        rotation *= Quaternion.Euler(2, 2, 2);
        this.gameObject.transform.rotation = rotation;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        enabledForPickup = true;
    }


    public void SetAbilityOnPickup(string name)
    {
        abilityName = name; 
        enabledForPickup = true;
    }

    public void PickupAbility(PlayerController player)
    {
        if (enabledForPickup)
        {
            FindObjectOfType<AbilitySlotBarUI>().AddAbilityToLoadoutBar(abilityName);
            player.abilityInventory.AddAbilityToInventory(FindObjectOfType<AbilityRegister>().Clone(abilityName));
            NetworkServer.Destroy(this.gameObject);
        }
    }


}
