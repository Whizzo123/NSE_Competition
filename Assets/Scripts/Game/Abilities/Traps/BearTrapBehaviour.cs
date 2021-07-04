using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BearTrapBehaviour : NetworkBehaviour
{
    private PlayerController trappedPlayer;

    public float trapDuration = 5;
    private float currentDuration;
    
    private bool sprung;

    public GameObject openTrap;
    public GameObject closedTrap;

    private string placingPlayerName;

    void Start()
    {
        currentDuration = 0;
        sprung = false;
    }
    public void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.GetComponent<PlayerController>() && collider.isTrigger == false)
        {
            if(collider.gameObject.GetComponent<PlayerController>().playerName != placingPlayerName)
            {
                trappedPlayer = collider.gameObject.GetComponent<PlayerController>();
                trappedPlayer.SetImmobilized(true);
                trappedPlayer.transform.position = new Vector3(this.transform.position.x, trappedPlayer.transform.position.y, this.transform.position.z);
                CmdSpringTrap();
            }
        }
    }

    public void SetPlacingPlayer(PlayerController controller)
    {
        placingPlayerName = controller.playerName;
    }

    [Command]
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

    /// <summary>
    /// Cal from server to all clients to finish the trap by releasing player and destroying itself
    /// </summary>
    [ClientRpc]
    private void RpcFinishTrap()
    {
        Destroy(this);
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
                    trappedPlayer.SetImmobilized(false);
                    RpcFinishTrap();
                }
            }
        }
    }
}
