using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class VoodooPoisonTrapBehaviour : NetworkBehaviour
{
    private PlayerController trappedPlayer;

    public float trapDuration = 2.5f; // Test this please
    private float currentDuration;
    
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
        if (placingPlayerName != null)
        {
            if (collider.gameObject.GetComponent<PlayerController>() && collider.isTrigger == false)
            {
                if (collider.gameObject.GetComponent<PlayerController>().playerName != placingPlayerName && !collider.gameObject.GetComponent<PlayerController>().IsVoodooPoisoned())
                {
                    trappedPlayer = collider.gameObject.GetComponent<PlayerController>();
                    trappedPlayer.CmdSetVoodooPoisoned(true);
                    if (!trappedPlayer.IsVoodooPoisoned())
                        CmdCreateAbilityEffectTimer("Voodoo Poison Trap", trappedPlayer.playerName, trapDuration);
                    CmdSpringTrap();
                }
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
        sprung = true;
    }

    [ClientRpc]
    private void RpcSpringTrap()
    {
        Close();
    }

    public void Close()
    {
        //GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        openTrap.SetActive(false);
        closedTrap.SetActive(true);
    }

    void Update()
    {
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
                    CmdUpdateTargetTimer(trappedPlayer.playerName, "Voodoo Poison Trap", currentDuration);
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
        if (NetworkClient.localPlayer.GetComponent<PlayerController>().playerName == targetPlayerName)
            GameObject.FindObjectOfType<AbilityTimerContainer>().UpdateTimer(abilityName, duration);
    }
}
