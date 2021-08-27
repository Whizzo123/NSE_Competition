using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerToAbilityInteraction : NetworkBehaviour
{

    [Tooltip("Our abilities that we've selected")] [SyncVar] public AbilityInventory abilityInventory;
    [Tooltip("In devlopment: The ability pickups that are in range for picking up")] private AbilityPickup targetedAbilityPickup;


    public override void OnStartAuthority()
    {
        CmdSetupPlayer();
        base.OnStartAuthority();
    }
    /// <summary>
    /// Resets some variables and sets up some components
    /// </summary>
    [Command]
    private void CmdSetupPlayer()
    {
        //Components
        abilityInventory = new AbilityInventory();
    }
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; };

        abilityInventory.Update();


        if (targetedAbilityPickup != null)
        {
            targetedAbilityPickup.PickupAbility(this);
            targetedAbilityPickup = null;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!hasAuthority) { return; };

        if (collider.gameObject.GetComponent<AbilityPickup>())
        {
            targetedAbilityPickup = collider.gameObject.GetComponent<AbilityPickup>();

        }
    }

    /// <summary>
    /// Used to disable all interactions and disbable text interactions.
    /// </summary>
    public void OnTriggerExit(Collider collider)
    {
        if (!hasAuthority) { return; };
        if (collider != null)
        {
            if (targetedAbilityPickup != null && collider.gameObject == targetedAbilityPickup.gameObject)
            {
                targetedAbilityPickup = null;
            }

        }
    }
    #region GETTERS/SETTERS
    public AbilityInventory GetAbilityInventory()
    {
        return abilityInventory;
    }
    public AbilityPickup GetAbilityPickup()
    {
        return targetedAbilityPickup;
    }
    public void SetAbilityPickup(AbilityPickup ability)
    {
        targetedAbilityPickup = ability;
    }
    #endregion
}
