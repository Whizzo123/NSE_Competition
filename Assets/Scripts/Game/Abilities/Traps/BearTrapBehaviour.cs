using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BearTrapBehaviour : NetworkBehaviour
{
    private PlayerToAbilityInteraction trappedPlayer;

    public float trapDuration = 5;
    private float currentDuration;
    
    [SyncVar]
    private bool sprung;

    public GameObject openTrap;
    public GameObject closedTrap;

    [SyncVar]
    private string placingPlayerName;

    void Start()
    {
        currentDuration = 0;
        sprung = false;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (placingPlayerName != null && sprung == false)
        { 
            if (collider.gameObject.GetComponent<PlayerToAbilityInteraction>() && collider.isTrigger == false)
            {
                if (collider.gameObject.GetComponent<PlayerStates>().playerName != placingPlayerName)
                {
                    trappedPlayer = collider.gameObject.GetComponent<PlayerToAbilityInteraction>();
                    if (!trappedPlayer.GetImmobolised())
                    {
                        trappedPlayer.SetImmobolised(true);
                        Vector3 movePos = new Vector3(this.transform.position.x, trappedPlayer.transform.position.y, this.transform.position.z);
                        trappedPlayer.CmdMovePlayer(movePos, trappedPlayer.GetComponent<PlayerStates>().playerName);
                        CmdCreateAbilityEffectTimer("Bear Trap", trappedPlayer.GetComponent<PlayerStates>().playerName, trapDuration);
                    }
                    CmdSpringTrap();
                    
                }
            }
        }
    }

    public void SetPlacingPlayer(string playerName)
    {
        placingPlayerName = playerName;
    }

    [Command (requiresAuthority = false)]
    private void CmdSpringTrap()
    {
        FindObjectOfType<AudioManager>().PlaySound("BearTrapClose");
        RpcSpringBearTrap();
        sprung = true;
    }

    /// <summary>
    /// Call from server to all clients to spring the bear trap
    /// </summary>
    [ClientRpc]
    private void RpcSpringBearTrap()
    {  
        Close();
    }

    public void Close()
    {
        openTrap.SetActive(false);
        closedTrap.SetActive(true);
    }

    public void Update()
    {
        UpdateTrap();
    }

    [ServerCallback]
    private void UpdateTrap()
    {
        if (sprung)
        {
            if (trapDuration > -1)
            {
                if (currentDuration < trapDuration)
                {
                    if(trappedPlayer.GetImmobolised() == false)
                        trappedPlayer.SetImmobolised(true);
                    currentDuration += Time.deltaTime;
                    CmdUpdateTargetTimer(trappedPlayer.GetComponent<PlayerStates>().playerName, "Bear Trap", currentDuration);
                }
                else
                {
                    if(trappedPlayer.GetImmobolised())
                        trappedPlayer.SetImmobolised(false);
                    NetworkServer.Destroy(this.gameObject);
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdCreateAbilityEffectTimer(string abilityName, string targetPlayerName, float fullDuration)
    {
        RpcCreateAbilityEffectTimer(abilityName, targetPlayerName, fullDuration);
    }

    [ClientRpc]
    private void RpcCreateAbilityEffectTimer(string abilityName, string targetPlayerName, float fullDuration)
    {
        if (NetworkClient.localPlayer.GetComponent<PlayerStates>().playerName == targetPlayerName && FindObjectOfType<AbilityTimerContainer>().Contains(abilityName) == false)
            Ability.CreateLocalAbilityEffectTimer(abilityName, fullDuration);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateTargetTimer(string targetPlayerName, string abilityName, float duration)
    {
        RpcUpdateTargetTimer(targetPlayerName, abilityName, duration);
    }

    [ClientRpc]
    private void RpcUpdateTargetTimer(string targetPlayerName, string abilityName, float duration)
    {
        if (NetworkClient.localPlayer.GetComponent<PlayerStates>().playerName == targetPlayerName)
            GameObject.FindObjectOfType<AbilityTimerContainer>().UpdateTimer(abilityName, duration);
    }
}
