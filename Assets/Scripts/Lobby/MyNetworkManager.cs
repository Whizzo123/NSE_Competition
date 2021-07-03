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

    void Start()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("NetworkSpawningPrefabs");
        for (int i = 0; i < prefabs.Length; i++)
        {
            spawnPrefabs.Add(prefabs[i]);
        }
    }

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
        if (newSceneName.Contains("Mirror"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                GameObject gameplayInstance = Instantiate(spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "Player"), new Vector3(i * 5, 0, 0), Quaternion.identity);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayInstance.gameObject);
                gameplayInstance.gameObject.GetComponent<PlayerController>().playerName = PlayerPrefs.GetString("username");
                NetworkServer.Spawn(gameplayInstance, conn);
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

            ServerChangeScene("MirrorTest");
        }
    }
}
