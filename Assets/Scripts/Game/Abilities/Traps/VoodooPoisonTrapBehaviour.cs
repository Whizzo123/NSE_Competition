using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class VoodooPoisonTrapBehaviour : NetworkBehaviour
{
    private PlayerController trappedPlayer;

    public float trapDuration = 5;
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
                if (collider.gameObject.GetComponent<PlayerController>().playerName != placingPlayerName)
                {
                    trappedPlayer = collider.gameObject.GetComponent<PlayerController>();
                    trappedPlayer.transform.position = new Vector3(this.transform.position.x, trappedPlayer.transform.position.y, this.transform.position.z);
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
        trappedPlayer.SetVoodooPoisoned(true);
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
                    trappedPlayer.SetVoodooPoisoned(false);
                    NetworkServer.Destroy(this.gameObject);
                }
            }
        }
    }
}
