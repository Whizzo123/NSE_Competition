using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.FizzySteam;
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
    [Tooltip("Do we use Steam for matchmaking or not?")]
    [SerializeField] public bool useSteamMatchmaking;


    //List<LobbyPlayer> RoomPlayers = new List<LobbyPlayer>();

    public List<MirrorRoomPlayerLobby> RoomPlayers = new List<MirrorRoomPlayerLobby>();

    void Start()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("NetworkSpawningPrefabs");
        for (int i = 0; i < prefabs.Length; i++)
        {
            spawnPrefabs.Add(prefabs[i]);
        }
        if(useSteamMatchmaking)
        {
            transport = GetComponent<FizzySteamworks>();
        }
        else
        {
            transport = GetComponent<TelepathyTransport>();
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

            if (FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
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
        if (newSceneName.Contains("Game"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                Vector3 spawnPos = new Vector3(Random.Range(2.26f, 3.86f), 0.6f, Random.Range(-26.13f, -11.94f));
                GameObject gameplayInstance = Instantiate(spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "Player"), spawnPos, Quaternion.identity);
                gameplayInstance.GetComponent<PlayerController>().playerName = RoomPlayers[i].DisplayName;
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gameplayInstance.gameObject);
                NetworkServer.Spawn(gameplayInstance, conn);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        ChangeMusic();
        base.OnServerSceneChanged(sceneName);

        Debug.Log("OnServerSceneChanged");
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        ChangeMusic();
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    }

    void ChangeMusic()
    {
        FindObjectOfType<AudioManager>().ActivateGameMusic();
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
