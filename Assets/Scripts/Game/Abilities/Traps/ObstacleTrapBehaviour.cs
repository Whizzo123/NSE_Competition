using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bolt;


public class ObstacleTrapBehaviour : EntityBehaviour<IObstacleTrap>
{
    private PlayerController trappedPlayer;

    public float trapDuration = 5;
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

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerController>() && other.isTrigger == false)
        {
            if(other.gameObject.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Name != state.PlacingPlayer.GetState<IGamePlayerState>().Name)
            {
                trappedPlayer = other.gameObject.GetComponent<PlayerController>();
                SpringTrap();
            }
        }
    }

    private void SpringTrap()
    {
        //FindObjectOfType<AudioManager>().PlaySound("ObstacleTrapSprung");
        var request = SpringObstacleTrap.Create();
        request.Victim = trappedPlayer.entity;
        request.End = false;
        request.Trap = entity;
        request.Send();
        sprung = true;
    }

    public void Disable()
    {
        closedTrap.SetActive(false);
        openTrap.SetActive(false);
    }


}

