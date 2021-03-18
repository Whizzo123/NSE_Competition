using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class BearTrapBehaviour : EntityBehaviour<IBearTrap>
{
    private PlayerController trappedPlayer;
    public float trapDuration;
    private float currentDuration;
    private bool sprung;
    private bool disabled;

    public GameObject openTrap;
    public GameObject closedTrap;

    public override void Attached()
    {
        currentDuration = 0;
        sprung = false;
        disabled = false;
    }
    public void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.GetComponent<PlayerController>() && collider.isTrigger == false)
        {
            if (collider.gameObject.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Name != state.PlacingPlayer.GetState<IGamePlayerState>().Name)
            {
                trappedPlayer = collider.gameObject.GetComponent<PlayerController>();
                SpringTrap();
            }
        }
    }

    private void SpringTrap()
    {
        FindObjectOfType<AudioManager>().PlaySound("BearTrapClose");
        var request = SpringBearTrap.Create();
        request.Victim = trappedPlayer.entity;
        request.End = false;
        request.Trap = entity;
        request.Send();
        sprung = true;
    }

    public void Disable()
    {
        closedTrap.SetActive(false);
    }

    public void Close()
    {
        openTrap.SetActive(false);
        closedTrap.SetActive(true);
    }

    public override void SimulateOwner()
    {
        if (sprung && !disabled)
        {
            if (trapDuration > -1)
            {
                if (currentDuration < trapDuration)
                {
                    currentDuration += Time.deltaTime;
                }
                else
                {
                    var request = SpringBearTrap.Create();
                    request.Victim = trappedPlayer.entity;
                    request.End = true;
                    request.Trap = entity;
                    request.Send();
                    disabled = true;
                }
            }
        }
    }

}
