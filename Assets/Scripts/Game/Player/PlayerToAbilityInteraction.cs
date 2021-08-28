using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerToAbilityInteraction : NetworkBehaviour
{

    [Tooltip("Our abilities that we've selected")] [SyncVar] private AbilityInventory abilityInventory;
    [Tooltip("In devlopment: The ability pickups that are in range for picking up")] private AbilityPickup targetedAbilityPickup;

    private PlayerStates playerStates;

    private void Start()
    {
        playerStates = GetComponent<PlayerStates>();
    }
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
    #region Immobolise
    public void SetImmobolised(bool immobolise)
    {
        playerStates.GetPlayerMovement().enabled = immobolise;
    }
    public bool GetImmobolised()
    {
        return playerStates.GetPlayerMovement().enabled;
    }
    #endregion

    #region Camo
    public bool GetCamouflaged()
    {
        return !transform.GetChild(0).transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled;
    }
    [Command]
    public void CmdToggleCamouflage(PlayerStates player)
    {
        Debug.Log("CmdToggleCamouflage: local player: " + NetworkClient.localPlayer.GetComponent<PlayerStates>().playerName);
        //GetPlayerToEmpower().ToggleMesh(toggle);
        RpcToggleCamouflage(player);
    }
    [ClientRpc]
    private void RpcToggleCamouflage(PlayerStates player)
    {
        Debug.Log("ClientRpc call toggling camouflage for: " + player.playerName);
        if (NetworkClient.localPlayer.GetComponent<PlayerStates>() != player)
        {
            Debug.Log("RpcToggleCamouflage the ClientRpc is hitting another player: " + NetworkClient.localPlayer.GetComponent<PlayerStates>().playerName);
            player.GetPlayerToAbilityInteraction().ToggleMesh();
        }
        else
        {
            Debug.Log("RpcToggleCamouflage the ClientRpc is hitting client called: " + NetworkClient.localPlayer.GetComponent<PlayerStates>().playerName);

        }
    }
    /// <summary>
    /// Toggles the mesh on and off for invisibility effect
    /// </summary>
    private void ToggleMesh()
    {
        //Player -> _scaleTest -> FULL.002
        transform.GetChild(0).transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = !transform.GetChild(0).transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled;
    }
    #endregion

    public void SetVoodooPlayer(bool voodood)
    {
        if (voodood)
        {
            playerStates.GetPlayerMovement().voodooSpeed = -1;
        }
        else
        {
                playerStates.GetPlayerMovement().voodooSpeed = 1;
        }
    }
    public bool GetVoodooPlayer()
    {
        return (playerStates.GetPlayerMovement().voodooSpeed == -1);
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
    [Command(requiresAuthority = false)]
    public void CmdModifySpeed(float newSpeed)
    {
        playerStates.GetPlayerMovement().speed = newSpeed;
    }
    public bool IsMortal()
    {
        return playerStates.mortal;
    }
    [Command(requiresAuthority = false)]
    public void CmdSetMortal(bool mortal)
    {
        playerStates.mortal = mortal;
    }
    [Command(requiresAuthority = false)]
    public void CmdMovePlayer(Vector3 position, string playerName)
    {
        transform.position = position;
        RpcMovePlayer(position, playerName);
    }
    [ClientRpc]
    public void RpcMovePlayer(Vector3 position, string playerName)
    {
        if (playerStates.playerName == playerName)
        {
            transform.position = position;
        }
    }
    [Command]
    public void CmdSetStolenFrom(bool stolen)
    {
        GetComponent<PlayerToPlayerInteraction>().enabled = !stolen;
        GetComponent<PlayerToAbilityInteraction>().SetImmobolised(stolen);
        if (stolen == true)
        {
            //Todo: Start timer for wear off period

        }
    }
    #endregion
}
