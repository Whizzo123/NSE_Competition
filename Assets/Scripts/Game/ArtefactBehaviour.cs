using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public enum ArtefactRarity { Common, Rare, Exotic}

public class ArtefactBehaviour : EntityBehaviour<IArtefactState>
{

    private string name;
    private int points;
    private ArtefactRarity rarity;

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            state.Name = name;
            state.Points = points;
        }
    }


    public void Pickup(PlayerController player)
    {
        player.AddToInventory(state.Name, state.Points);
        var request = ArtefactDisable.Create();
        request.artefactToDisable = this.entity;
        request.Send();
    }

    public void PopulateData(string dataName, ArtefactRarity rarity)
    {
        state.Name = dataName;
        name = dataName;
        int dataPoints = 0;
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
