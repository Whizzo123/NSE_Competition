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

    public override void Attached()
    {
        currentDuration = 0;
        sprung = false;
        disabled = false;
    }
    public void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.GetComponent<PlayerController>())
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
        var request = SpringBearTrap.Create();
        request.Victim = trappedPlayer.entity;
        request.End = false;
        request.Trap = entity;
        request.Send();
        sprung = true;
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
