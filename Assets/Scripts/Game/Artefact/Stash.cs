using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

/// <summary>
/// Controls the stash, adding to the stash, changing the scoreboard and removing artefacts from playerinventory.
/// </summary>
public class Stash : NetworkBehaviour
{
    [SerializeField][Tooltip("Playeres name and their respective scores")] readonly SyncDictionary<string, int> StashedScores = new SyncDictionary<string, int>();

    [SerializeField][Tooltip("Amount of points a player must reach to win and end the game")] public int winningPointsThreshold;

    #region ADDING_TO_SCORE
    /// <summary>
    /// Called when adding to stash from player inventory
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdAddToStashScores(PlayerToArtefactInteraction player)
    {
        string playerName = player.GetComponent<PlayerStates>().playerName;

        //Sets score
        int score = 0;
        if (StashedScores.ContainsKey(playerName))
            score = StashedScores[playerName];

        //Add all artefacts in player inventory to score, then StashedScore
        foreach (ItemArtefact item in player.GetArtefactInventory().GetInventory())
        {
            score += item.points;
        }
        StashedScores[playerName] = score;
        ScoreUpdate(playerName);

        RpcClearInventory(player);
    }
    /// <summary>
    /// Removes all artefacts from inventory
    /// </summary>
    [ClientRpc]
    private void RpcClearInventory(PlayerToArtefactInteraction player)
    {
        player.GetArtefactInventory().ClearInventory(player.GetComponent<PlayerStates>().playerName);
    }
    /// <summary>
    /// Updates the the scoreboard ui
    /// </summary>
    [ClientRpc]
    private void ScoreUpdate(string playerName)
    {
        FindObjectOfType<CanvasUIManager>().scoreboardUI.UpdateBoard(playerName);
    }
    #endregion


    /// <summary>
    /// Check all players and see if any have reached the winning threshold.
    /// </summary>
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

    /// <summary>
    /// Find the score for the player of 'name' in StashedScores
    /// </summary>
    public int FindScoreForPlayer(string name)
    {
        if(StashedScores.ContainsKey(name))
        {
            return StashedScores[name];
        }
        Debug.LogError("ERROR: COULDN'T FIND SCORE IN STASH FOR PLAYER: " + name);
        return 0;
    }
    /// <summary>
    /// Used to convert SyncDictionary to normal Dictionary for easier handling
    /// </summary>
    /// <returns></returns>
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
