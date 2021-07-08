using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityPickup : NetworkBehaviour
{
    [SyncVar]
    private string abilityName;
    [SyncVar]
    private bool enabledForPickup;
    private Quaternion rotation = Quaternion.Euler(45, 45, 0);
    public void FixedUpdate()
    {
        rotation *= Quaternion.Euler(2, 2, 2);
        this.gameObject.transform.rotation = rotation;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        CmdSetEnabledForPickup(true);
    }


    public void SetAbilityOnPickup(string name)
    {
        CmdSetAbilityName(name);
        CmdSetEnabledForPickup(true);
    }

    public string GetAbilityName()
    {
        return abilityName;
    }

    public bool IsEnabledForPickup()
    {
        return enabledForPickup;
    }

    [Command (requiresAuthority = false)]
    public void CmdSetAbilityName(string name)
    {
        abilityName = name;
    }

    [Command (requiresAuthority = false)]
    public void CmdSetEnabledForPickup(bool value)
    {
        enabledForPickup = value;
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
