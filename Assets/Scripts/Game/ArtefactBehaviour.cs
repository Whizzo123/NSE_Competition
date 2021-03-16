using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public enum ArtefactRarity { Common, Rare, Exotic}

public class ArtefactBehaviour : EntityBehaviour<IArtefactState>
{

    private string artefactName;
    private int points;
    private ArtefactRarity rarity;
    private bool availableForPickup;


    public override void Attached()
    {
        if (entity.IsOwner)
        {
            state.Name = artefactName;
            state.Points = points;
        }
        availableForPickup = false;
    }


    public void Pickup(PlayerController player)
    {
        if (availableForPickup)
        {
            player.AddToInventory(state.Name, state.Points);
            var request = ArtefactDisable.Create();
            request.artefactToDisable = this.entity;
            request.Send();
        }
    }

    public void EnableForPickup()
    {
        availableForPickup = true;
        //GetComponent<MeshRenderer>().enabled = true;
    }

    public void PopulateData(string dataName, ArtefactRarity rarity)
    {
        state.Name = dataName;
        artefactName = dataName;
        int dataPoints = 0;
        this.rarity = rarity;
        switch(rarity)
        {
            case ArtefactRarity.Common:
                dataPoints = 200;
                break;
            case ArtefactRarity.Rare:
                dataPoints = 1000;
                break;
            case ArtefactRarity.Exotic:
                dataPoints = 5000;
                break;
        }
        state.Points = dataPoints;
        points = dataPoints;
    }


}
