﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Stash : NetworkBehaviour
{
    readonly SyncDictionary<string, int> StashedScores = new SyncDictionary<string, int>();

    public int winningPointsThreshold;

    /// <summary>
    /// Called when entity attached in network like unity start method
    /// </summary>
    public override void OnStartAuthority()
    {

    }

    /// <summary>
    /// Called when adding to stash from player inventory
    /// </summary>
    /// <param name="player"></param>
    [Command(requiresAuthority = false)]
    public void CmdAddToStashScores(PlayerController player)
    {
        string playerName = player.playerName;
        int score = 0;
        if (StashedScores.ContainsKey(playerName))
            score = StashedScores[playerName];
        foreach (ItemArtefact item in player.GetComponent<ArtefactInventory>().GetInventory())
        {
            score += item.points;
        }
        StashedScores[playerName] = score;
        ScoreUpdate(playerName);
        RpcClearInventory(player);
    }

    [ClientRpc]
    private void RpcClearInventory(PlayerController player)
    {
        player.GetComponent<ArtefactInventory>().ClearInventory();
    }

    [ClientRpc]
    private void ScoreUpdate(string playerName)
    {
        FindObjectOfType<CanvasUIManager>().scoreboardUI.UpdateBoard(playerName);
    }

    /// <summary>
    /// Used to update the Stash state
    /// </summary>
    /// <param name="name"></param>
    /// <param name="points"></param>
    public void UpdateScore(string name, int points)
    {
        if(StashedScores.ContainsKey(name))
        {
            StashedScores[name] += points;
        }
    }

    public bool HasPlayerReachedWinningPointsThreshold()
    {
        if (StashedScores.Count > 0)
        {
            foreach (string playerName in StashedScores.Keys)
            {
                if (StashedScores[playerName] >= winningPointsThreshold)
                    return true;
            }
        }
        return false;
    }

    public int FindScoreForPlayer(string name)
    {
        if(StashedScores.ContainsKey(name))
        {
            return StashedScores[name];
        }
        Debug.LogError("ERROR: COULDN'T FIND SCORE IN STASH FOR PLAYER: " + name);
        return 0;
    }

    public Dictionary<string, int> GetStashedScores()
    {
        Dictionary<string, int> toReturn = new Dictionary<string, int>();
        foreach (string key in StashedScores.Keys)
        {
            toReturn.Add(key, StashedScores[key]);
        }
        return toReturn;
    }
}
