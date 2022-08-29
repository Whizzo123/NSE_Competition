using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BearTrapBehaviour : NetworkBehaviour
{
    private PlayerController trappedPlayer;

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
            Debug.Log("<color=red>BearTrapSprung : IF1</color>");
            if (collider.gameObject.GetComponent<PlayerController>() && collider.isTrigger == false 
                && collider.gameObject.GetComponent<PlayerController>().playerName != placingPlayerName)
            {
                Debug.Log("<color=red>BearTrapSprung : IF2</color>");

                trappedPlayer = collider.gameObject.GetComponent<PlayerController>();
                Debug.Log("<color=red>BearTrapSprung : IF2 2</color>");

                if (!trappedPlayer.IsImmobilized())
                {
                    Debug.Log("<color=red>BearTrapSprung : IF3</color>");

                    trappedPlayer.CmdSetImmobilized(true);
                    Vector3 movePos = new Vector3(this.transform.position.x, trappedPlayer.transform.position.y, this.transform.position.z);
                    trappedPlayer.gameObject.GetComponent<PlayerController>().CmdMovePlayer(movePos, trappedPlayer.playerName);
                    CmdCreateAbilityEffectTimer("Bear Trap", trappedPlayer.playerName, trapDuration);
                    CmdSpringTrap();
                }
                //If two traps are stacked, the commands will send twice. making it trap infinitley
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

    float safeBreak = 10.0f;//GEt rid of this when cleaner solution is found
    public void Update()
    {
        UpdateTrap();
    }

    [ServerCallback]
    private void UpdateTrap()
    {
        if (sprung)
        {
            safeBreak -= Time.deltaTime;
            if (trapDuration > -1)
            {
                if (currentDuration < trapDuration)
                {
                    if(trappedPlayer.IsImmobilized() == false)
                        trappedPlayer.CmdSetImmobilized(true);
                    currentDuration += Time.deltaTime;
                    CmdUpdateTargetTimer(trappedPlayer.playerName, "Bear Trap", currentDuration);
                }
                else
                {
                    if(trappedPlayer.IsImmobilized())
                        trappedPlayer.CmdSetImmobilized(false);
                    NetworkServer.Destroy(this.gameObject);
                }
            }
            if (safeBreak < 0)
            {
                trappedPlayer.CmdSetImmobilized(false);
                NetworkServer.Destroy(this.gameObject);
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
