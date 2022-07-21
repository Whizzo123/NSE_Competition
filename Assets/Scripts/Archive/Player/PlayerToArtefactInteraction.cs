//using System.Collections;
//using UnityEngine;
//using Mirror;

//public class PlayerToArtefactInteraction : NetworkBehaviour
//{

//    public void InteractWithArtefact(PlayerController controller)
//    {
//        //If we have artefacts in range
//        if (controller.targetedArtefacts.Count != 0 && controller.tempArtefactStorage.Count != 0)
//        {
//            //If we have an empty slot
//            if (controller.GetArtefactInventory().GetInventoryCount() <= 7)
//            {
//                // All artefacts that are in our range get added to our inventory and gameobject destroyed
//                foreach (ArtefactBehaviour item in controller.targetedArtefacts)
//                {
//                    if (item.gameObject != null)
//                    {
//                        controller.GetArtefactInventory().AddToInventory(item.GetArtefactName(), item.GetPoints());
//                        FindObjectOfType<AudioManager>().PlaySound(item.GetRarity().ToString());
//                        DestroyGameObject(item.gameObject);
//                        controller.artefactsForDestruction.Add(item.GetComponent<NetworkIdentity>().netId);
//                    }
//                }
//                controller.CmdClearTargetArtefacts();
//                controller.tempArtefactStorage.Clear();

//                if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
//                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
//            }
//            else
//            {
//                FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot pickup artefact inventory is full (Max: 8 artefacts)");
//            }
//        }
//        if (controller.GetGameStash() != null && controller.GetArtefactInventory().InventoryNotEmpty())
//        {
//            //Todo: For consistancy, instead of clearing the artefact inventory elsewhere, let's clear it here
//            controller.GetGameStash().CmdAddToStashScores(controller);
//            controller.tempArtefactStorage.Clear();
//            controller.artefactsForDestruction.Clear();
//            controller.CmdClearTargetArtefacts();
//            FindObjectOfType<AudioManager>().PlaySound("Stash");
//        }
//        else if (controller.GetGameStash() != null && !controller.GetArtefactInventory().InventoryNotEmpty())
//        {
//            FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot deposit no artefacts in inventory");
//        }
//    }

//    public void TriggerEnterInteraction(PlayerController controller, Collider collider)
//    {
//        //Allows us to interact with the gamestash and shows hint message
//        if (collider.gameObject.GetComponent<Stash>())
//        {
//            controller.SetGameStash(collider.gameObject.GetComponent<Stash>());
//            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
//                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Deposit");
//        }
//    }

//    public void TriggerStayInteraction(PlayerController controller, Collider collider)
//    {
//        if (!collider.gameObject.GetComponent<ArtefactBehaviour>())
//        {
//            return;
//        }
//        ArtefactBehaviour artefactBehaviour = collider.gameObject.GetComponent<ArtefactBehaviour>();
//        if (controller.artefactsForDestruction.Contains(artefactBehaviour.netId))
//        {
//            return;
//        }
//        //If it is available for pickup and it currently isn't in tempartefactstorage
//        if (artefactBehaviour && controller.tempArtefactStorage.Contains(artefactBehaviour) == false && controller.targetedArtefacts.Contains(artefactBehaviour) == false 
//            && artefactBehaviour.IsAvaliableForPickup() && controller.targetedArtefacts.Count <= 4)
//        {
//            //Adds it temporarily
//            controller.tempArtefactStorage.Add(artefactBehaviour);
//            //Sends command to add it to targeted artefact
//            CmdAddToTargetedArtefacts(artefactBehaviour);

//            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
//                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Pickup");
//        }
//    }

//    public void TriggerExitInteraction(PlayerController controller, Collider collider)
//    {
//        //Artefacts
//        if (controller.targetedArtefacts.Count != 0 && collider.gameObject.GetComponent<ArtefactBehaviour>())
//        {
//            //Removes specific artefact that we exited.
//            int i = 0;
//            foreach (ArtefactBehaviour item in controller.targetedArtefacts)
//            {
//                if (item.GetInstanceID() == collider.gameObject.GetComponent<ArtefactBehaviour>().GetInstanceID())
//                {
//                    controller.tempArtefactStorage.Remove(item);
//                    CmdTargetArtefactsRemoveAt(item);

//                }
//                i++;
//            }
//            if (controller.targetedArtefacts.Count == 0)
//            {
//                FindObjectOfType<CanvasUIManager>().CloseHintMessage();
//            }
//        }
//        //Game Stash
//        else if (controller.GetGameStash() != null && collider.gameObject == controller.GetGameStash().gameObject)
//        {
//            controller.SetGameStash(null);
//            if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
//                FindObjectOfType<CanvasUIManager>().CloseHintMessage();
//        }
//    }

//    /// <summary>
//    /// Calls CmdDestroyGameObject.
//    /// </summary>
//    [ClientCallback]
//    public void DestroyGameObject(GameObject go)
//    {
//        CmdDestroyGameObject(go);
//    }
//    /// <summary>
//    /// Destroys networked GameObjects.
//    /// <para>Call DestroyGameObject(GameObject go) instead to destroy on all instances.</para>
//    /// </summary>
//    [Command(requiresAuthority = false)]
//    public void CmdDestroyGameObject(GameObject go)
//    {
//        NetworkServer.Destroy(go);
//    }


//    [Command]
//    private void CmdAddToTargetedArtefacts(ArtefactBehaviour artefact)
//    {
//        GetComponent<PlayerController>().targetedArtefacts.Add(artefact);
//    }
//    [Command]
//    private void CmdTargetArtefactsRemoveAt(ArtefactBehaviour artefact)
//    {
//        GetComponent<PlayerController>().targetedArtefacts.Remove(artefact);
//    }
//    [Command]
//    private void CmdTargetArtefactsRemoveAtI(int i)
//    {
//        GetComponent<PlayerController>().targetedArtefacts.RemoveAt(i);
//    }
//}
