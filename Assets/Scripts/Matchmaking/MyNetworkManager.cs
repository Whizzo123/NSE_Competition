using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Inherits from most functions of NetworkManager, where NetworkManager has a lot of doing functions,
/// MySceneManager has a lot of reactive functions - what happens when something happens.
/// <para>Manages a lot of Mirror backend things such as connecting clients together and managing scene changes</para>
/// </summary>
public class MyNetworkManager : NetworkManager
{

    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Lobby")]
    [SerializeField] [Tooltip("Prefab used to spawn the lobby player")] private MirrorRoomPlayerLobby lobbyPlayerPrefab = null;
    [SerializeField][Tooltip("All players inside the lobby")] public List<MirrorRoomPlayerLobby> RoomPlayers = new List<MirrorRoomPlayerLobby>();    
    [SerializeField] [Tooltip("Prefab used to spawn the lobby player")] private PlayerObjectController matchmakingPlayerPrefab = null;
    [SerializeField] public List<PlayerObjectController> matchmakingPlayers { get; } = new List<PlayerObjectController>();
    [SerializeField] [Tooltip("Minimum players to start a game")] public int minPlayers;
    [Header("Game")]
    [SerializeField] [Tooltip("Prefab used to spawn the controllable player")] private PlayerController gamePlayerPrefab = null;
    [SerializeField] [Tooltip("Do we use Steam for matchmaking or not?")] public bool useSteamMatchmaking;
 

    //List<LobbyPlayer> RoomPlayers = new List<LobbyPlayer>();


    void Start()
    {
        //Loads all gameobjects in resources that have NetworkIdentities attatched to them.
        var networkIdentities = Resources.LoadAll<NetworkIdentity>("").Cast<NetworkIdentity>().ToArray();
        List<GameObject> gameObjectsFromNetworked = new List<GameObject>();
        foreach (var item in networkIdentities)
        {
            gameObjectsFromNetworked.Add(item.gameObject);
        }
        gameObjectsFromNetworked.CopyTo(spawnPrefabs);
    }

    #region CLIENT_CONNECTION_FUNCTIONS
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
    #endregion

    #region SERVER_CONNECTION_FUNCTIONS
    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
    }
    /// <summary>
    /// If the lobby is full, disconnect the most recent connnection. todo: We should also add in a way to remove the lobby if it's full
    /// </summary>
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect");
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
    }
    public override void OnStopServer()
    {
        Debug.Log("OnStopServer");
        base.OnStopServer();
    }
    #endregion

    #region SERVER_CLIENT_EVENTS
    public override void OnServerAddPlayer(NetworkConnection conn) 
    {
        //Debug.Log("Inside OnServerAddPlayer");

        //if (SceneManager.GetActiveScene().name == "Matchmaking")
        //{
        //    MirrorRoomPlayerLobby lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);

        //    //Host?
        //    bool isLeader = RoomPlayers.Count == 0;
        //    lobbyPlayerInstance.IsLeader = isLeader;

        //    //Steam setup
        //    if (FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
        //    {
        //        Debug.Log("JOE: Setting up steam and grabbing ID");
        //        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(LobbyUIManager.LobbyId, RoomPlayers.Count);
        //        lobbyPlayerInstance.SetSteamId(steamId.m_SteamID);
        //    }
        //    else
        //    {
        //        lobbyPlayerInstance.SetSteamId(0);
        //    }

        //    NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        //}
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            Debug.Log("Creating PlayerObjectController");
            PlayerObjectController gamePlayerInstance = Instantiate(matchmakingPlayerPrefab);

            gamePlayerInstance.connectionID = conn.connectionId;
            gamePlayerInstance.playerIDNumber = matchmakingPlayers.Count + 1;
            gamePlayerInstance.playerSteamID = 
                (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyId, matchmakingPlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);

        }
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");
        if(conn.identity != null && SceneManager.GetActiveScene().name == "Matchmaking")
        {
            if (SceneManager.GetActiveScene().name == "Matchmaking")
            {
                var player = conn.identity.GetComponent<MirrorRoomPlayerLobby>();

                RoomPlayers.Remove(player);

                NotifyPlayersofReadyState();
            } 
        }
        base.OnServerDisconnect(conn);
    }
    
    public override void ServerChangeScene(string newSceneName)
    {
        if (newSceneName.Contains("Plains") || newSceneName.Contains("Quarantine"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                //For each player, replaces their lobby identity with the game identity
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
        if(sceneName.Contains("Quarantine"))
            ChangeMusic();
        SteamMatchmaking.LeaveLobby(LobbyUIManager.LobbyId);
        base.OnServerSceneChanged(sceneName);

        Debug.Log("OnServerSceneChanged");
    }
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        RoomPlayers.Clear();
        if (newSceneName.Contains("Quarantine"))
            ChangeMusic();
        SteamMatchmaking.LeaveLobby(LobbyUIManager.LobbyId);
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        Debug.Log("OnServerReady");
    }
    #endregion

    #region READY_START
    /// <summary>
    /// Handles telling all players whether the Lobby is ready to proceed to game
    /// </summary>
    public void NotifyPlayersofReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }
    /// <summary>
    /// If all players are ready and there is more than the minimum amount of players required to start - 
    /// return true
    /// </summary>
    /// <returns></returns>
    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    /// <summary>
    /// If everyone is ready and we're in lobby; change everyone's scene to the 'GameScene'
    /// </summary>
    public void StartGame()
    {
        if (!IsReadyToStart()) { return; }
        ServerChangeScene("Plains");
    }
    #endregion

    /// <summary>
    /// Handles the activation of music on scene changes and the such
    /// </summary>
    void ChangeMusic()
    {
        FindObjectOfType<AudioManager>().ActivateGameMusic();
    }

}
