using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class ArtefactBehaviour : EntityBehaviour<IArtefactState>
{

    private string name;
    private int points;

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

    public void PopulateData(string dataName, int dataPoints)
    {
        state.Name = dataName;
        name = dataName;
        state.Points = dataPoints;
        points = dataPoints;
    }


}
