using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{

    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Lobby")]
    [SerializeField] private MirrorRoomPlayerLobby lobbyPlayerPrefab = null;
    [SerializeField] public int minPlayers;
    [Header("Game")]
    [SerializeField] private PlayerController gamePlayerPrefab = null;


    //List<LobbyPlayer> RoomPlayers = new List<LobbyPlayer>();

    public List<MirrorRoomPlayerLobby> RoomPlayers = new List<MirrorRoomPlayerLobby>();

    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
    }

    public override void OnStartClient()
    {
        Debug.Log("OnStartClient");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        Debug.Log("OnClientConnect");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        Debug.Log("OnClientDisconnect");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect");
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn) 
    {
        Debug.Log("Inside OnServerAddPlayer");
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            MirrorRoomPlayerLobby lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);

            bool isLeader = RoomPlayers.Count == 0;

            lobbyPlayerInstance.IsLeader = isLeader;
            if (LobbyUIManager.useSteamMatchmaking)
            {
                CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(LobbyUIManager.LobbyId, RoomPlayers.Count);
                lobbyPlayerInstance.SetSteamId(steamId.m_SteamID);
            }
            else
            {
                lobbyPlayerInstance.SetSteamId(0);
            }
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<MirrorRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersofReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Debug.Log("OnStopServer");
        base.OnStopServer();
    }

    public override void ServerChangeScene(string newSceneName)
    {
        Debug.Log("ServerChangeScene");
        if(SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Game"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayInstance = Instantiate(gamePlayerPrefab);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayInstance.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        Debug.Log("OnServerSceneChanged");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        Debug.Log("OnServerReady");
    }

    public void NotifyPlayersofReadyState()
    {
        foreach(var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if(numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if(!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if(SceneManager.GetActiveScene().name == "LobbyScene")
        {
            if(!IsReadyToStart()) { return; }

            ServerChangeScene("GameScene");
        }
    }
}
