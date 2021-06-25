using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomScreenUI : MonoBehaviour
{
    #region Variables
    public List<LobbyPlayer> players = new List<LobbyPlayer>();
    public GameObject playersList;
    public GameObject playerPrefab;
    #endregion 


    public void ResetUI()
    {
        foreach(LobbyPlayersInfo child in playersList.GetComponentsInChildren<LobbyPlayersInfo>())
        {
            Destroy(child.gameObject);
        }
    }
    /// <summary>
    /// Add player to the room list of players
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(LobbyPlayer player)
    {

        if(player == null) { return; }

        if(players.Contains(player))
        {
            return;
        }

        players.Add(player);

        //player.gameObject.transform.SetParent(playersList.transform, false);
    }

    public void UpdateDisplay()
    {
        foreach (LobbyPlayer player in players)
        {
           // player.name = player.
        }
    }

    /// <summary>
    /// Remove player from the room list
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayer(LobbyPlayer player)
    {
        if (player == null) { return; }

        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

}
