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

    [SyncVar]
    private string placingPlayerName;

    void Start()
    {
        currentDuration = 0;
        sprung = false;
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
                    if (!trappedPlayer.IsImmobilized())
                    {
                        trappedPlayer.CmdSetImmobilized(true);
                        Vector3 movePos = new Vector3(this.transform.position.x, trappedPlayer.transform.position.y, this.transform.position.z);
                        trappedPlayer.CmdMovePlayer(movePos, trappedPlayer.playerName);
                    }
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
                    if(trappedPlayer.IsImmobilized() == false)
                        trappedPlayer.CmdSetImmobilized(true);
                    currentDuration += Time.deltaTime;
                }
                else
                {
                    if(trappedPlayer.IsImmobilized())
                        trappedPlayer.CmdSetImmobilized(false);
                    NetworkServer.Destroy(this.gameObject);
                }
            }
        }
    }
}
