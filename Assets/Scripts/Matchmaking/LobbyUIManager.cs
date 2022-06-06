using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.SceneManagement;
using Steamworks;
using Mirror;
using Mirror.Discovery;

/// <summary>
/// Identifying name accompanied by the gameobject tied to it.
/// </summary>
[Serializable]
public struct UIScreens
{
    public string screenName;
    public GameObject screen;
}

/// <summary>
/// <para>This class manages both Steam and Mirror's creation of lobbies, initialisation of network, and join requests.</para>
/// <para>It also manages a lot of UI in the 'Matchmaking'</para>
/// </summary>
public class LobbyUIManager : MonoBehaviour
{
    #region Variables

    [Header("Canvas and Screens")]
    [SerializeField][Tooltip("All screens that we can switch to")]public UIScreens[] screens;

    [Tooltip("Creation Screen for Lobbies")] private CreateScreenUI createScreen;
    [Tooltip("Browse Screen for choosing Servers")] private BrowseScreenUI browseScreen;

    [SerializeField] [Tooltip("The main canvas")] public Canvas BrowseCreateCanvas;
    [Tooltip("Seperate canvas for Lobby screen")] public Canvas RoomCanvas;
    [Space]

    [Header("Lobby settings")]
    [Tooltip("Room name specified by player")] private string roomName;//Do we need this at all, does it add to the user experience

    public static CSteamID LobbyId { get; private set; }//LobbyID is the lobbies unique identifier

    [SerializeField] [Tooltip("Holds address for host")] private const string HostAddressKey = "HostAddress";
    [Tooltip("Holds name for lobby")] private const string LobbyNameKey = "LobbyName";

    [SerializeField][Tooltip("Minimum Required Players To Start")] private int minPlayers = 2;
    [SerializeField][Tooltip("Time in second between all players ready & match start")]public float prematchCountdown = 5.0f;
    [Space]


    [Header("Network")]
    [Tooltip("Reference to the MyNetworkManager")] private MyNetworkManager networkManager;
    [Tooltip("Reference to the NetworkDiscovery")] private NetworkDiscovery networkDiscovery;

    [Tooltip("Dictionary of all discovered servers")] public readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    [SerializeField] [Tooltip("Are we joining randomly?")] private bool randomJoin;
    [Space]


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
        //Starts steam
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
    }

    #endregion

    #region LOBBY_CREATION_OR_JOINING
    /// <summary>
    /// Called when creating a room on the network, decides whether to create mirror or steam lobby.
    /// </summary>
    private void CreateRoomSession()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        roomName = createScreen.inputField.text;
        Debug.Log("CreatingRoomSession");
        FindObjectOfType<BackTemp>().SwitchingTo(ActiveScreen.LOBBY);
        if (FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
        else
            CreateMirrorLobby();
    }
    /// <summary>
    /// Switches to browse screen, decides whether to discover Steam lobbies or Mirror lobbies.
    /// </summary>
    private void SwapToBrowseScreen()
    {
        //In order for client to view session list we need to connect them to the network in order to discover servers it's not so much a connection as it is putting the client online
        FindObjectOfType<AudioManager>().PlaySound("Click");
        ChangeScreenTo("Browse");

        if (FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
        {
            SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
            SteamMatchmaking.RequestLobbyList();
        }
        else
            networkDiscovery.StartDiscovery();
    }
    //RandomJoinNotSupportedYet----------------------------------------------------------------
    /// <summary>
    /// Joins any available session, decides whether to join steam or mirror lobbies
    /// </summary>
    private void JoinRandomSession()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        //Set random join to true
        randomJoin = true;
        //Launch the client
    }
    //------------------------------------------------------------------------------------------
    #endregion

    #region STEAMLOGICLOBBY

    /// <summary>
    /// Setup all the callbacks and stuff for passing between Steam and Mirror
    /// </summary>
    private void InitializeSteam()
    {
        if (!SteamManager.Initialized) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchListGrab);
    }

    #region Callbacks
    /// <summary>
    /// Called when a lobby is created through SteamMatchmaking
    /// </summary>
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Lobby could not be created");
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        //Start Steam Networking
        networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(LobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(LobbyId, LobbyNameKey, roomName);//JoeReply -> it's setting 2 different things hostaddress and lobbyname
    }

    /// <summary>
    /// Joins Steam Lobby with callback.m_steamIDLobby
    /// </summary>
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    /// <summary>
    /// When player has connected to Lobby, they trigger this function.
    /// <para>Change the screen to Room and sets up local lobby data, and starts a client connection to the host</para>
    /// </summary>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        ChangeScreenTo("Room");
        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        //If we are the server don't bother going further
        if (NetworkServer.active) { return; }

        //Starts a client connection by connecting to the host address
        string hostAddress = SteamMatchmaking.GetLobbyData(LobbyId, HostAddressKey);
        networkManager.networkAddress = hostAddress;
        Debug.LogError("NetworkAddress: " + hostAddress);
        networkManager.StartClient();
    }
    /// <summary>
    /// Callback for when we ask for all steam lobbies matching filter
    /// </summary>
    private void OnLobbyMatchListGrab(LobbyMatchList_t callback)
    {

        //Loop through returned callback which is just a number
        List<LobbyInfo> lobbies = new List<LobbyInfo>();
        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            //Grab that lobby and get it's data into a struct to be added to list
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

        //Update the UI
        browseScreen.SessionListUpdated(lobbies);
    }

    public Callback<LobbyMatchList_t> GetLobbyMatchList_T()
    {
        return lobbyMatchList;
    }
    #endregion
    #endregion

    #region MIRRORLOBBYLOGIC
    /// <summary>
    /// Disables BrowseCreateCanvas and starts hosting locally.
    /// </summary>
    private void CreateMirrorLobby()
    {
        Debug.Log("CreatingMirrorLobby");
        BrowseCreateCanvas.gameObject.SetActive(false);
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();
    }
    /// <summary>
    /// Adds server to discoveredServers
    /// </summary>
    /// <param name="info"></param>
    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note: that you can check the versioning to decide if you can connect to the server or not using this method
        Debug.Log("ServerID: " + info.serverId);
        Debug.Log("Uri: " + info.uri.Host);
        Debug.Log("IPEndPoint: " + info.EndPoint.ToString());
        discoveredServers[info.serverId] = info;
    }

    #endregion



 

    /// <summary>
    /// Stops client connection
    /// </summary>
    public void BackToTitleScreen()
    {
        networkManager.StopClient();
    }

    #region SCREEN_MANAGEMENT
    /// <summary>
    /// Find screen in UIScreens[], does not load it.
    /// </summary>
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

    /// <summary>
    /// Changes screen to the specified name, in UIScreens[]
    /// </summary>
    /// <param name="name"></param>
    public void ChangeScreenTo(string name)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if(name == "Room")
            {
                Debug.Log("Switching to lobby screen");
                FindObjectOfType<BackTemp>().SwitchingTo(ActiveScreen.LOBBY);
                BrowseCreateCanvas.gameObject.SetActive(false);
            }
            if(screens[i].screen == null) { return; }
            screens[i].screen.SetActive(false);
            if(screens[i].screenName == name)
            {
                if(screens[i].screenName == "Room")
                {
                    ///
                }
                else if(screens[i].screenName == "Create")
                {
                    Debug.Log("Switching to create screen");
                    BrowseCreateCanvas.gameObject.SetActive(true);
                    FindObjectOfType<BackTemp>().SwitchingTo(ActiveScreen.CREATE);
                }
                else if(screens[i].screenName == "Browse")
                {
                    Debug.Log("Switching to browse screen");
                    FindObjectOfType<BackTemp>().SwitchingTo(ActiveScreen.BROWSE);
                }
                screens[i].screen.SetActive(true);
            }
        }
    }
    #endregion

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