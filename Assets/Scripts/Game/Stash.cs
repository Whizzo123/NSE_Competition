﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;


public class Stash : EntityBehaviour<IStashState>
{
    private Dictionary<string, int> nameToScore;

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            for (int i = 0; i < state.StashedScores.Length; i++)
            {
                state.StashedScores[i].Name = "";
                state.StashedScores[i].Score = 0;
            }
        }
        nameToScore = new Dictionary<string, int>();
    }

    public void AddToStashScores(PlayerController player)
    {
        var request = ScoreUpdate.Create();
        request.PlayerName = player.state.Name;
        int score = 0;
        foreach (InventoryItem item in player.state.Inventory)
        {
            score += item.ItemPoints;
        }
        request.Score = score;
        request.StashEntity = entity;
        request.Send();
        player.ClearInventory();
    }

    public void UpdateState(string name, int points)
    {
        int index = Contains(name);
        if(index < 0)
        {
            BoltLog.Info("Index was less than zero");
            for (int i = 0; i < state.StashedScores.Length; i++)
            {
                BoltLog.Info("StaashedScore[Name] is: " + state.StashedScores[i].Name);
                if (state.StashedScores[i].Name == "")
                {
                    BoltLog.Info("Found a stashed score index with null");
                    state.StashedScores[i].Name = name;

                    index = i;
                    break;
                }
            }
            BoltLog.Info("If haven't found one yet there isn't one so still -1");
        }
        if (index < 0)
            BoltLog.Error("Name does not exist in score list and no space to add it");
        else
        {
             state.StashedScores[index].Score += points;
        }
    }

    private int Contains(string name)
    {
        for (int i = 0; i < state.StashedScores.Length; i++)
        {
            if(state.StashedScores[i].Name == name)
            {
                return i;
            }
        }
        return -1;
    }
}
