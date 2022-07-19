using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerToAbilityInteraction : NetworkBehaviour
{

    [Tooltip("In devlopment: The ability pickups that are in range for picking up")] private AbilityPickup targetedAbilityPickup;

    public void InteractWithAbility(PlayerController controller)
    {
        if (targetedAbilityPickup != null)
        {
            targetedAbilityPickup.PickupAbility(controller);
            targetedAbilityPickup = null;
        }
    }

    public void TriggerEnterInteraction(PlayerController controller, Collider collider)
    {
        if (collider.gameObject.GetComponent<AbilityPickup>())
        {
            targetedAbilityPickup = collider.gameObject.GetComponent<AbilityPickup>();
        }
    }

    public void TriggerExitInteraction(PlayerController controller, Collider collider)
    {
        if (targetedAbilityPickup != null && collider.gameObject == targetedAbilityPickup.gameObject)
        {
            targetedAbilityPickup = null;
        }
    }

}
