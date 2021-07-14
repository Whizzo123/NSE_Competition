using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.SceneManagement;
using Steamworks;
using Mirror;
using Mirror.Discovery;

public class LobbyUIManager : MonoBehaviour
{
    #region Variables

    [SerializeField]
    public UIScreens[] screens;

    private CreateScreenUI createScreen;
    private BrowseScreenUI browseScreen;

    public Canvas BrowseCreateCanvas;
    public Canvas RoomCanvas;

    public string gameSceneName;

    [Tooltip("Minimum Required Players To Start")]
    [SerializeField] private int minPlayers = 2;

    [Tooltip("Time in second between all players ready & match start")]
    [SerializeField]
    public float prematchCountdown = 5.0f;



    private bool randomJoin;

    private const string HostAddressKey = "HostAddress";
    private const string LobbyNameKey = "LobbyNameTH";

    private MyNetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;

    public readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    public static CSteamID LobbyId { get; private set; }

    private string roomName;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyMatchList_t> lobbyMatchList;

    #endregion

    #region START_ACTIONMAPPING
    void Start()
    {
        networkManager = FindObjectOfType<MyNetworkManager>();
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        if (FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
            InitializeSteam();

        //Setup all the screens for the lobby
        createScreen = FindScreenByName("Create").screen.GetComponent<CreateScreenUI>();
        if(createScreen == null)
        {
            Debug.LogError("Failed to find the create screen");
        }
        browseScreen = FindScreenByName("Browse").screen.GetComponent<BrowseScreenUI>();
        if(browseScreen == null)
        {
            Debug.LogError("Failed to find the browse screen");
        }
        //Set random join to false by default
        randomJoin = false;
        StartUI();
    }
    /// <summary>
    /// Sets up click handlers for the menu buttons
    /// </summary>
    private void StartUI()
    {
        createScreen.OnCreateButtonClick += CreateRoomSession;
        createScreen.OnBrowseButtonClick += SwapToBrowseScreen;
        createScreen.OnRandomButtonClick += JoinRandomSession;
        //browseScreen.OnClickJoinSession += JoinSessionEvent;
    }

    #endregion

    #region SteamLobbyLogic
    private void InitializeSteam()
    {
        if (!SteamManager.Initialized) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchListGrab);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Lobby could not be created");
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(LobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(LobbyId, LobbyNameKey, roomName);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        ChangeScreenTo("Room");
        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        Debug.LogError("LobbyId: " + LobbyId);
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(LobbyId, HostAddressKey);

        networkManager.networkAddress = hostAddress;
        Debug.LogError("NetworkAddress: " + hostAddress);
        networkManager.StartClient();
    }

    private void OnLobbyMatchListGrab(LobbyMatchList_t callback)
    {
        Debug.Log("Callback number: " + callback.m_nLobbiesMatching);
        List<LobbyInfo> lobbies = new List<LobbyInfo>();
        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            if (SteamMatchmaking.GetLobbyData(lobbyId, LobbyNameKey) != "")
            {
                LobbyInfo info = new LobbyInfo();
                info.lobbyID = lobbyId;
                info.lobbyName = SteamMatchmaking.GetLobbyData(lobbyId, LobbyNameKey);
                info.playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyId);
                lobbies.Add(info);
                Debug.Log("LobbyID: " + lobbyId);
            }
        }
        browseScreen.SessionListUpdated(lobbies);
    }
    #endregion

    #region MirrorLobbyLogic
    private void CreateMirrorLobby()
    {
        BrowseCreateCanvas.gameObject.SetActive(false);
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        Debug.Log("ServerID: " + info.serverId);
        Debug.Log("Uri: " + info.uri.Host);
        Debug.Log("IPEndPoint: " + info.EndPoint.ToString());
        discoveredServers[info.serverId] = info;
    }

    #endregion
    /// <summary>
    /// Called when creating a room on the network
    /// </summary>
    private void CreateRoomSession()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        roomName = createScreen.inputField.text;
        Debug.Log("CreatingRoomSession");

        if (FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
        else
            CreateMirrorLobby();
    }

    private void SwapToBrowseScreen()
    {
        //In order for client to view session list we need to connect them to the network
        FindObjectOfType<AudioManager>().PlaySound("Click");
        ChangeScreenTo("Browse");
        if(FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
            SteamMatchmaking.RequestLobbyList();
        else
            networkDiscovery.StartDiscovery();
    }

    //RandomJoinNotSupportedYet----------------------------------------------------------------
    private void JoinRandomSession()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        //Set random join to true
        randomJoin = true;
        //Launch the client
    }
    //------------------------------------------------------------------------------------------

    public void BackToTitleScreen()
    {
        networkManager.StopClient();
    }

    private UIScreens FindScreenByName(string name)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if(screens[i].screenName == name)
            {
                return screens[i];
            }
        }
        Debug.LogError("Name: " + name + " does not belong to any UIScreen object");
        return new UIScreens();
    }
    public void ChangeScreenTo(string name)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if(screens[i].screen == null) { return; }
            screens[i].screen.SetActive(false);
            if(screens[i].screenName == name)
            {
                if(screens[i].screenName == "Room")
                {
                    RoomCanvas.gameObject.SetActive(true);
                    BrowseCreateCanvas.gameObject.SetActive(false);
                }
                screens[i].screen.SetActive(true);
            }
        }
    }



}


[Serializable]
public struct UIScreens
{
    public string screenName;
    public GameObject screen;
}

#region BOLTNETWORK_START_AUTHORISATION_JOIN_REFUSAL

/// <summary>
/// If we're on the same version and we're searching for a public lobby, return true, or if we're searching for a private lobby and have entered the correct password, return true.
/// </summary>
//private bool AuthUser(bool priv, string pass, double vers)
//{
//    if (vers == versionNo)
//    {
//        if (priv == privateLobby)
//        {
//            if (pass == password)
//            {
//                return true;
//            }
//        }
//    }

//    return false;
//}

////Called when a client wants to connect, by the host
//public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
//{
//    //This is called in by the host
//    BoltLog.Warn("Connect Request");

//    //If the client info sent is the same as the host server info, accept the client else send an error message
//    var userToken = token as ServerInfo;
//    if (userToken != null)
//    {
//        if (AuthUser(userToken.privateSession, userToken.password, userToken.versionNumber) && joinableLobby)
//        {
//            BoltNetwork.Accept(endpoint, userToken);
//            return;
//        }
//    }

//    ErrorMessages error = new ErrorMessages();
//    error.Error = "Unknown";

//    if (BoltMatchmaking.CurrentSession.ConnectionsCurrent == BoltMatchmaking.CurrentSession.ConnectionsMax)
//    {
//        error.Error = "The lobby is full";
//    }
//    else if (password != userToken.password)
//    {
//        error.Error = "The password was wrong";
//    }
//    if (!joinableLobby)
//    {
//        error.Error = "The lobby has now entered the game";
//    }
//    if (versionNo != userToken.versionNumber)
//    {
//        error.Error = "Tried joining a game that was on a different version, sorry for the situation.";
//    }

//    BoltNetwork.Refuse(endpoint, error);

//}

////Called when the client is refused, by the client
//public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
//{
//    //This is called by the refused client
//    BoltLog.Warn("Connection refused!");

//    var authToken = token as ErrorMessages;
//    if (authToken != null)
//    {
//        //Display to user as text box
//        Debug.LogError(authToken.Error);
//    }
//    BoltNetwork.Shutdown();
//}


#endregion