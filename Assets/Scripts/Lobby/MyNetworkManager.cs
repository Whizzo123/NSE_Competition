using System.Collections;
using UnityEngine;
using Mirror;
using Steamworks;

public class MyNetworkManager : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(LobbyUIManager.LobbyId, numPlayers - 1);

        Debug.Log("Inside OnServerAddPlayer");

        foreach (LobbyPlayer playerInfos in FindObjectsOfType<LobbyPlayer>())
        {
            if (playerInfos.name.text == string.Empty)
                playerInfos.SetSteamId(steamId.m_SteamID);
        }
    }

}
