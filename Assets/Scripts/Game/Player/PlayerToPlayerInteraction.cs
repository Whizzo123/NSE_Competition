using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerToPlayerInteraction : NetworkBehaviour
{

    [Tooltip("Have we recently been stolen from?")] [SyncVar] private bool hasBeenStolenFrom = false;
    [Tooltip("The player that is currently targeted to steal artefacts from")] private PlayerToPlayerInteraction targetedPlayerToStealFrom;
    [Tooltip("NA")] private float currentStunAfterTimer;
    [Tooltip("Time player is stunned after being stolen from")] public float timeForStunAfterSteal = 10.0f;

    [SyncVar] private ArtefactInventory artefactInventory;
    private PlayerStates player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStates>();
    }

    public override void OnStartAuthority()
    {
        CmdSetArtefactInventory();
        base.OnStartAuthority();
    }
    [Command]
    private void CmdSetArtefactInventory()
    {
        artefactInventory = GetComponent<PlayerToArtefactInteraction>().GetArtefactInventory();
    }

    [ClientCallback]
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
                targetedPlayerToStealFrom.GetComponent<PlayerToAbilityInteraction>().CmdSetStolenFrom(false);
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
            ArtefactInventory enemyPlayerArtefactInventory = targetedPlayerToStealFrom.GetArtefactInventory();
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
                        targetedPlayerToStealFrom.GetComponent<PlayerToAbilityInteraction>().CmdSetStolenFrom(true);
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
        if (collider.gameObject.GetComponent<PlayerToPlayerInteraction>())
        {
            targetedPlayerToStealFrom = collider.gameObject.GetComponent<PlayerToPlayerInteraction>();
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
                if (NetworkClient.localPlayer.GetComponent<PlayerToPlayerInteraction>() == this)
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
    public string GetPlayerName()
    {
        return player.playerName;
    }
    #endregion
}
