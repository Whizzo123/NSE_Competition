using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScreenUI : MonoBehaviour
{

    public List<LobbyPlayer> players = new List<LobbyPlayer>();
    public GameObject playersList;
    public GameObject playerPrefab;

    public void ResetUI()
    {
        foreach(LobbyPlayersInfo child in playersList.GetComponentsInChildren<LobbyPlayersInfo>())
        {
            Destroy(child.gameObject);
        }
    }

    public void AddPlayer(LobbyPlayer player)
    {

        if(player == null) { return; }

        if(players.Contains(player))
        {
            return;
        }

        players.Add(player);

        player.gameObject.transform.parent = playersList.transform;


    }

}
