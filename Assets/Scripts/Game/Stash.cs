using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Stash : NetworkBehaviour
{
    readonly SyncDictionary<string, int> StashedScores = new SyncDictionary<string, int>();
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
    public void AddToStashScores(PlayerController player)
    {
        int score = 0;
        string playerName = player.playerNameText.GetComponent<Text>().text;
        foreach (ItemArtefact item in player.GetComponent<ArtefactInventory>().GetInventory())
        {
            score += item.points;
        }
        StashedScores[playerName] = score;
        ScoreUpdate(playerName);
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

    public int FindScoreForPlayer(string name)
    {
        if(StashedScores.ContainsKey(name))
        {
            return StashedScores[name];
        }
        Debug.LogError("ERROR: COULDN'T FIND SCORE IN STASH FOR PLAYER: " + name);
        return 0;
    }
}
