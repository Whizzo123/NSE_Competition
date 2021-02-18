using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScreenUI : MonoBehaviour
{

    public List<LobbyPlayer> players = new List<LobbyPlayer>();
    public GameObject playersList;
    public GameObject playerPrefab;

    public void AddPlayer(LobbyPlayer player)
    {
        if(player == null) { return; }

        if(players.Contains(player))
        {
            return;
        }

        players.Add(player);

        GameObject lobbyPlayer = Instantiate(playerPrefab, playersList.transform, false);
        lobbyPlayer.GetComponent<LobbyPlayersInfo>().Populate(player);
    }

}
