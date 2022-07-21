//using System.Collections;
//using UnityEngine;
//using Mirror;

//public class PlayerToPlayerInteraction : NetworkBehaviour
//{

//    [Tooltip("The player that is currently targeted to steal artefacts from")] private PlayerController targetedPlayerToStealFrom;
//    [Tooltip("NA")] private float currentStunAfterTimer;
//    [Tooltip("Time player is stunned after being stolen from")] public float timeForStunAfterSteal; // 3

//    public void Steal(PlayerController controller)
//    {
//        //If we are not full, they are no longer stunned and have artefacts, we steal
//        if (targetedPlayerToStealFrom != null)
//        {
//            if (controller.GetArtefactInventory().AvailableInventorySlot() && targetedPlayerToStealFrom.GetArtefactInventory().InventoryNotEmpty() 
//                && targetedPlayerToStealFrom.HasPlayerBeenStolenFrom() == false)
//            {

//                //Add to our inventory
//                ItemArtefact randomArtefact = targetedPlayerToStealFrom.GetArtefactInventory().GrabRandomItem();
//                controller.GetArtefactInventory().AddToInventory(randomArtefact.name, randomArtefact.points);

//                //remove from enemy inventory
//                for (int indexToRemove = 0; indexToRemove < targetedPlayerToStealFrom.GetArtefactInventory().GetInventory().Count; indexToRemove++)
//                {
//                    if (targetedPlayerToStealFrom.GetArtefactInventory().GetInventory()[indexToRemove].name != string.Empty && targetedPlayerToStealFrom.GetArtefactInventory().GetInventory()[indexToRemove].name == randomArtefact.name)
//                    {
//                        targetedPlayerToStealFrom.GetArtefactInventory().RemoveFromInventory(indexToRemove, randomArtefact.name, randomArtefact.points);
//                        targetedPlayerToStealFrom.CmdSetImmobilized(true);
//                        targetedPlayerToStealFrom.GetComponent<PlayerToPlayerInteraction>().CmdSetHasBeenStolenFrom(targetedPlayerToStealFrom, true);
//                        break;
//                    }
//                }
//            }
//            else
//            {
//                FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot steal from player has no artefacts or stolen from recently");

//            }
//        }
//    }

//    public void UpdateSteal(PlayerController controller)
//    {
//        //Stunning timer from being stolen from
//        if (controller.HasPlayerBeenStolenFrom())
//        {
//            if (currentStunAfterTimer >= timeForStunAfterSteal)
//            {
//                currentStunAfterTimer = 0;
//                CmdSetHasBeenStolenFrom(controller, false);
//                controller.CmdSetImmobilized(false);
//            }
//            else
//                currentStunAfterTimer += Time.deltaTime;
//        }
//    }

//    [Command(requiresAuthority = false)]
//    public void CmdSetHasBeenStolenFrom(PlayerController controller, bool value)
//    {
//        controller.SetHasBeenStolenFrom(value);
//    }

//    public void TriggerEnterInteraction(PlayerController controller, Collider collider)
//    {
//        //Allows us to interact with A player and shows hint message
//        if (collider.gameObject.GetComponent<PlayerController>())
//        {
//            if (collider.GetComponentInChildren<SkinnedMeshRenderer>().enabled == false)
//            {
//                return;
//            }
//            targetedPlayerToStealFrom = collider.gameObject.GetComponent<PlayerController>();
//            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
//            {
//                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press F to Steal");
//            }
//        }
//    }

//    public void TriggerExitInteraction(PlayerController controller, Collider collider)
//    {
//        //Players
//        if (targetedPlayerToStealFrom != null && collider.gameObject == targetedPlayerToStealFrom.gameObject)
//        {
//            targetedPlayerToStealFrom = null;
//            if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
//                FindObjectOfType<CanvasUIManager>().CloseHintMessage();
//        }
//    }
//}
