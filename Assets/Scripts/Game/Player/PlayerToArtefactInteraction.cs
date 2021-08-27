using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerToArtefactInteraction : NetworkBehaviour
{
    [Header("Stored Interactables")]
    [Tooltip("This is used for adding artefacts to the inventory temporarily while a Command is being sent to add artefacts to the real inventory. The reason for this was to allow us to check that we are not picking up the same artefact twice.")] public List<ArtefactBehaviour> tempArtefactStorage;
    [Tooltip("The artefacts that are in range for picking up")] readonly SyncList<ArtefactBehaviour> targetedArtefacts = new SyncList<ArtefactBehaviour>();
    [Tooltip("Artefact netId's that have been marked for destruction, don't add back anywhere")] private List<uint> artefactsForDestruction = new List<uint>();
    [SyncVar] private ArtefactInventory artefactInventory;

    [Tooltip("NA")] private Stash gameStash;
    [SyncVar] public string playerName;



    public override void OnStartAuthority()
    {
        //Setup player components and immobolise player
        CmdSetupPlayer();
        base.OnStartAuthority();
    }
    /// <summary>
    /// Resets some variables and sets up some components
    /// </summary>
    [Command]
    private void CmdSetupPlayer()
    {
        artefactInventory = GetComponent<ArtefactInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; };
        #region ARTEFACT_INTERACTION
        if (Input.GetKeyDown(KeyCode.E))
        {
            ArtefactPickup();
            StashArtefact();

        }
        #endregion
    }


    private void ArtefactPickup()
    {
        //If we have artefacts in range
        if (targetedArtefacts.Count != 0 && tempArtefactStorage.Count != 0)
        {
            //If we have an empty slot
            if (artefactInventory.GetInventoryCount() <= 7)
            {
                Debug.Log("Picking up Artefacts");
                // All artefacts that are in our range get added to our inventory and gameobject destroyed
                foreach (ArtefactBehaviour item in targetedArtefacts)
                {
                    Debug.Log("Looping now ");
                    artefactInventory.AddToInventory(item.GetArtefactName(), item.GetPoints());
                    FindObjectOfType<AudioManager>().PlaySound(item.GetRarity().ToString());
                    DestroyGameObject(item.gameObject);
                    artefactsForDestruction.Add(item.GetComponent<NetworkIdentity>().netId);

                }
                CmdClearTargetArtefacts();
                tempArtefactStorage.Clear();

                if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
            }
            else
            {
                FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot pickup artefact inventory is full (Max: 8 artefacts)");
            }
        }
    }

    private void StashArtefact()
    {
        if (gameStash != null && artefactInventory.InventoryNotEmpty())
        {
            //Todo: For consistancy, instead of clearing the artefact inventory elsewhere, let's clear it here
            gameStash.CmdAddToStashScores(this);
            tempArtefactStorage.Clear();
            artefactsForDestruction.Clear();
            CmdClearTargetArtefacts();
            FindObjectOfType<AudioManager>().PlaySound("Stash");
        }
        else if (gameStash != null && !artefactInventory.InventoryNotEmpty())
        {
            FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot deposit no artefacts in inventory");
        }
    }
    /// <summary>
    /// Calls CmdDestroyGameObject.
    /// </summary>
    [ClientCallback]
    public void DestroyGameObject(GameObject go)
    {
        CmdDestroyGameObject(go);
    }
    /// <summary>
    /// Destroys networked GameObjects.
    /// <para>Call DestroyGameObject(GameObject go) instead to destroy on all instances.</para>
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdDestroyGameObject(GameObject go)
    {
        NetworkServer.Destroy(go);
    }


    public void OnTriggerEnter(Collider collider)
    {
        if (!hasAuthority) { return; };
        //Allows us to interact with the gamestash and shows hint message
        if (collider.gameObject.GetComponent<Stash>())
        {
            gameStash = collider.gameObject.GetComponent<Stash>();
            if (FindObjectOfType<CanvasUIManager>() != null)// && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Deposit");
        }
    }
    /// <summary>
    /// Used for the targeted artefact as it doesn't work in OnTriggerEnter, as when the artefacts re-appear
    /// , they don't trigger the function.
    /// </summary>
    public void OnTriggerStay(Collider collider)
    {
        if (!hasAuthority) { return; };
        ArtefactBehaviour artefactBehaviour = collider.gameObject.GetComponent<ArtefactBehaviour>();
        if (artefactsForDestruction.Contains(artefactBehaviour.netId))
        {
            return;
        }
        //If it is available for pickup and it currently isn't in tempartefactstorage
        if (artefactBehaviour &&
            tempArtefactStorage.Contains(artefactBehaviour) == false && targetedArtefacts.Contains(artefactBehaviour) == false &&
            artefactBehaviour.IsAvaliableForPickup() &&
            targetedArtefacts.Count <= 4)
        {
            //Adds it temporarily
            tempArtefactStorage.Add(artefactBehaviour);
            //Sends command to add it to targeted artefact
            CmdAddToTargetedArtefacts(artefactBehaviour);

            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Pickup");
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
            //Artefacts
            if (targetedArtefacts.Count != 0 && collider.gameObject.GetComponent<ArtefactBehaviour>())
            {
                //Removes specific artefact that we exited.
                int i = 0;
                foreach (ArtefactBehaviour item in targetedArtefacts)
                {
                    if (item.GetInstanceID() == collider.gameObject.GetComponent<ArtefactBehaviour>().GetInstanceID())
                    {
                        tempArtefactStorage.Remove(item);
                        CmdTargetArtefactsRemoveAt(item);

                    }
                    i++;
                }
                if (targetedArtefacts.Count == 0)
                {
                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
                }
            }
            //Game Stash
            else if (gameStash != null && collider.gameObject == gameStash.gameObject)
            {
                gameStash = null;
                if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
            }
        }
    }

    #region GETTERS/SETTERS
    public ArtefactInventory GetArtefactInventory()
    {
        return artefactInventory;
    }
    public void SetArtefactInventory(ArtefactInventory inventory)
    {
        artefactInventory = inventory;
    }

    public Stash GetStash()
    {
        return gameStash;
    }
    public void SetStash(Stash stash)
    {
        gameStash = stash;
    }


    #endregion
    [Command]
    private void CmdClearTargetArtefacts()
    {
        targetedArtefacts.Clear();
    }
    [Command]
    private void CmdAddToTargetedArtefacts(ArtefactBehaviour artefact)
    {
        targetedArtefacts.Add(artefact);
    }
    [Command]
    private void CmdTargetArtefactsRemoveAt(ArtefactBehaviour artefact)
    {
        targetedArtefacts.Remove(artefact);
    }
    [Command]
    private void CmdTargetArtefactsRemoveAtI(int i)
    {
        targetedArtefacts.RemoveAt(i);
    }



}
