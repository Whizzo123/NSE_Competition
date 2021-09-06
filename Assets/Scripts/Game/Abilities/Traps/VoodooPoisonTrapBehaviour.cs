using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class VoodooPoisonTrapBehaviour : NetworkBehaviour
{
    private PlayerController trappedPlayer;

    public float trapDuration = 2.5f; 
    private float currentDuration;
    
    [SyncVar]
    private bool sprung;
    private bool disabled;

    public GameObject openTrap;
    public GameObject closedTrap;

    [SyncVar]
    private string placingPlayerName;

    void Start()
    {
        currentDuration = 0;
        sprung = false;
        disabled = false;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (placingPlayerName != null && sprung == false && collider.gameObject.GetComponent<PlayerController>().playerName != placingPlayerName)
        {
            if (collider.gameObject.GetComponent<PlayerController>() && collider.isTrigger == false)
            {
                trappedPlayer = collider.gameObject.GetComponent<PlayerController>();
                sprung = true;
                if (!trappedPlayer.IsVoodooPoisoned())
                {
                    trappedPlayer.CmdSetVoodooPoisoned(true);
                    CmdCreateAbilityEffectTimer("Voodoo Poison Trap", trappedPlayer.playerName, trapDuration);
                }
                CmdSpringTrap();
            }
        }
    }

    public void SetPlacingPlayer(PlayerController controller)
    {
        placingPlayerName = controller.playerName;
    }

    [Command (requiresAuthority = false)]
    private void CmdSpringTrap()
    {
        RpcSpringTrap();
    }

    [ClientRpc]
    private void RpcSpringTrap()
    {
        Close();
    }

    public void Close()
    {
        openTrap.SetActive(false);
        closedTrap.SetActive(true);
    }

    void Update()
    {
        if (trappedPlayer == null)
            Debug.Log("Trapped player is null inside the Update Loop at: " + Time.realtimeSinceStartup);
        UpdateTrap();
    }

    [Server]
    private void UpdateTrap()
    {
        if (sprung)
        {
            if (trapDuration > -1)
            {
                if (currentDuration < trapDuration)
                {
                    currentDuration += Time.deltaTime;
                    if(trappedPlayer != null)
                        CmdUpdateTargetTimer(trappedPlayer.playerName, "Voodoo Poison Trap", currentDuration);
                    else
                    {
                        Debug.LogError("Trapped player is null when in UpdateTrap and trying to UpdateTargetTimer but it's now not affecting us since we moved sprung to be set sooner in the OnTriggerEnterMethod");
                        return;
                    }
                }
                else
                {
                    trappedPlayer.CmdSetVoodooPoisoned(false); 
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
        if (NetworkClient.localPlayer.GetComponent<PlayerController>().playerName == targetPlayerName && FindObjectOfType<AbilityTimerContainer>().Contains(abilityName) == false)
            Ability.CreateLocalAbilityEffectTimer(abilityName, fullDuration, true);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateTargetTimer(string targetPlayerName, string abilityName, float duration)
    {
        RpcUpdateTargetTimer(targetPlayerName, abilityName, duration);
    }

    [ClientRpc]
    private void RpcUpdateTargetTimer(string targetPlayerName, string abilityName, float duration)
    {
        if (NetworkClient.localPlayer.GetComponent<PlayerController>().playerName == targetPlayerName)
            GameObject.FindObjectOfType<AbilityTimerContainer>().UpdateTimer(abilityName, duration);
    }
}
