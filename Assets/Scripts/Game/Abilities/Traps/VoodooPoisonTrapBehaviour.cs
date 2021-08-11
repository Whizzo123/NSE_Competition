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
        GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
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
                }
                else
                {
                    trappedPlayer.CmdSetVoodooPoisoned(false);
                    NetworkServer.Destroy(this.gameObject);
                }
            }
        }
    }
}
