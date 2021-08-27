using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerToPlayerInteraction : NetworkBehaviour
{

    [Tooltip("Have we recently been stolen from?")] [SyncVar] private bool hasBeenStolenFrom = false;
    [Tooltip("The player that is currently targeted to steal artefacts from")] private PlayerStates targetedPlayerToStealFrom;
    [Tooltip("NA")] private float currentStunAfterTimer;
    [Tooltip("Time player is stunned after being stolen from")] public float timeForStunAfterSteal = 10.0f;

    private ArtefactInventory artefactInventory;
    private PlayerStates player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStates>();
        artefactInventory = GetComponent<PlayerToArtefactInteraction>().GetArtefactInventory();
    }

    // Update is called once per frame
    void Update()
    {

        if (!hasAuthority) { return; };
        #region STEALING
        if (Input.GetKeyDown(KeyCode.F) && !hasBeenStolenFrom)
        {
            StealFromPlayer();
        }

        //Stunning timer from being stolen from
        if (hasBeenStolenFrom)
        {
            if (currentStunAfterTimer >= timeForStunAfterSteal)
            {
                currentStunAfterTimer = 0;
                player.SetStolenFrom(false);
            }
            else
            {
                currentStunAfterTimer += Time.deltaTime;
            }
        }
        #endregion
    }

    private void StealFromPlayer()
    {
        Debug.LogError(artefactInventory.GetAllArtefactNames());

        //If we are not full, they are no longer stunned and have artefacts, we steal
        if (targetedPlayerToStealFrom != null)
        {
            ArtefactInventory enemyPlayerArtefactInventory = targetedPlayerToStealFrom.playerToArtefactInteraction.GetArtefactInventory();
            if (artefactInventory.AvailableInventorySlot() && enemyPlayerArtefactInventory.InventoryNotEmpty() && targetedPlayerToStealFrom.hasBeenStolenFrom == false)
            {

                //Add to our inventory
                ItemArtefact randomArtefact = enemyPlayerArtefactInventory.GrabRandomItem();
                artefactInventory.AddToInventory(randomArtefact.name, randomArtefact.points);

                //remove from enemy inventory
                for (int indexToRemove = 0; indexToRemove < enemyPlayerArtefactInventory.GetInventory().Count; indexToRemove++)
                {
                    if (enemyPlayerArtefactInventory.GetInventory()[indexToRemove].name != string.Empty && enemyPlayerArtefactInventory.GetInventory()[indexToRemove].name == randomArtefact.name)
                    {
                        enemyPlayerArtefactInventory.RemoveFromInventory(indexToRemove, randomArtefact.name, randomArtefact.points);
                        targetedPlayerToStealFrom.SetStolenFrom(true);
                        break;
                    }
                }
            }
            else
            {
                FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot steal from player has no artefacts or stolen from recently");

            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!hasAuthority) { return; };

        //Allows us to interact with A player and shows hint message
        if (collider.gameObject.GetComponent<PlayerController>())
        {
            targetedPlayerToStealFrom = collider.gameObject.GetComponent<PlayerStates>();
            if (FindObjectOfType<CanvasUIManager>() != null) //&& NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press F to Steal");
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
           
            //Players
            if (targetedPlayerToStealFrom != null && collider.gameObject == targetedPlayerToStealFrom.gameObject)
            {
                targetedPlayerToStealFrom = null;
                if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
            }
        }
    }
}
