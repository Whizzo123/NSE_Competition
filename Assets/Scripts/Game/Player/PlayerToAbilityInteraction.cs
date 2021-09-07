using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerToAbilityInteraction : NetworkBehaviour
{

    public void InteractWithAbility(PlayerController controller)
    {
        if (controller.GetTargetedAbilityPickup() != null)
        {
            controller.GetTargetedAbilityPickup().PickupAbility(controller);
            controller.SetTargetedAbilityPickup(null);
        }
    }

    public void TriggerEnterInteraction(PlayerController controller, Collider collider)
    {
        if (collider.gameObject.GetComponent<AbilityPickup>())
        {
            controller.SetTargetedAbilityPickup(collider.gameObject.GetComponent<AbilityPickup>());
        }
    }

    public void TriggerExitInteraction(PlayerController controller, Collider collider)
    {
        if (controller.GetTargetedAbilityPickup() != null && collider.gameObject == controller.GetTargetedAbilityPickup().gameObject)
        {
            controller.SetTargetedAbilityPickup(null);
        }
    }

}
