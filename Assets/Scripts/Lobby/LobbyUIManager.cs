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
    public RoomScreenUI roomScreen;

    public Canvas BrowseCreateCanvas;
    public Canvas RoomCanvas;

    public string gameSceneName;

    [Tooltip("Minimum Required Players To Start")]
    [SerializeField] private int minPlayers = 2;

    [Tooltip("Time in second between all players ready & match start")]
    [SerializeField]
    public float prematchCountdown = 5.0f;

    [Tooltip("Do we use Steam for matchmaking or not?")]
    [SerializeField]
    public static bool useSteamMatchmaking = false;

    private bool randomJoin;
    public bool isCountdown = false;

    public GameObject playerLobbyPrefab;

    private const string HostAddressKey = "HostAddress";
    private const string LobbyNameKey = "LobbyName";

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
        if (useSteamMatchmaking)
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
        //roomScreen = RoomCanvas.GetComponent<RoomScreenUI>();
        if(roomScreen == null)
        {
            Debug.LogError("Failed to find the room screen");
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


    /// <summary>
    /// Called when creating a room on the network
    /// </summary>
    private void CreateRoomSession()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        roomName = createScreen.inputField.text;
        Debug.Log("CreatingRoomSession");

        if (useSteamMatchmaking)
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
        else
            CreateMirrorLobby();
    }

    private void CreateMirrorLobby()
    {
        BrowseCreateCanvas.gameObject.SetActive(false);
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    private void SwapToBrowseScreen()
    {
        //In order for client to view session list we need to connect them to the network
        FindObjectOfType<AudioManager>().PlaySound("Click");
        //BoltLauncher.StartClient();
        ChangeScreenTo("Browse");
        if(useSteamMatchmaking)
            SteamMatchmaking.RequestLobbyList();
        else
        {
            networkDiscovery.StartDiscovery();
        }
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        Debug.Log("ServerID: " + info.serverId);
        Debug.Log("Uri: " + info.uri.Host);
        Debug.Log("IPEndPoint: " + info.EndPoint.ToString());
        discoveredServers[info.serverId] = info;
    }

    void Connect(ServerResponse info)
    {
        networkManager.StartClient(info.uri);
    }

    private void JoinRandomSession()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        //Set random join to true
        randomJoin = true;
        //Launch the client
        BoltLauncher.StartClient();
    }
    /*private void JoinSessionEvent(UdpSession session, IProtocolToken token)
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        //Chech whether this is the client should only run on a client
        if (BoltNetwork.IsClient)
        {
            //Join given session we choose from the list
            BoltMatchmaking.JoinSession(session,token);
        }
    }*/
    #endregion

    private void FixedUpdate()
    {
        //Check whether this is running on server computer and if we are not counting down yet
        if (BoltNetwork.IsServer && isCountdown == false)
        {
            var allReady = true;
            var readyCount = 0;

            //Grab a list of all the bolt entities currently on the network
            foreach (var entity in BoltNetwork.Entities)
            {
                //Check if this entity is tied to LobbyPlayerInfo state if not loop back
                if (entity.StateIs<ILobbyPlayerInfoState>() == false) continue;
                //Grab the state
                var lobbyPlayer = entity.GetState<ILobbyPlayerInfoState>();
                //Check if lobby player has hit ready button and add to allReady condition
                allReady &= lobbyPlayer.Ready;
                //If we have hit a lobby player that isn't ready break out of foreach loop
                if (allReady == false) break;
                //Adding to ready count
                readyCount++;
            }
            //If all players hit are ready and our count of players is above the minimum we need to start a game
            if (allReady && readyCount >= minPlayers)
            {
                //UnjoinableNetwork();
                isCountdown = true;
                //StartCoroutine(ServerCountdownCoroutine());
            }
        }
    }
    /// <summary>
    /// Countdown through prematch countdown then load up game scene
    /// </summary>
    private IEnumerator ServerCountdownCoroutine()
    {
        var remainingTime = prematchCountdown;
        var floorTime = Mathf.FloorToInt(remainingTime);

        LobbyCountdown countdown;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;
                //Create lobbycountdown event to update everyone's time
              // countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
               // countdown.Time = floorTime;
               // countdown.Send();
            }
        }
        //Once we are basically at zero send a final one
        //countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
       // countdown.Time = 0;
        //countdown.Send();
        //Tell network to load game scene for everyone
        BoltNetwork.LoadScene(gameSceneName);
    }
    

    #region LOBBYCREATION_AND_RANDOMJOIN_AND_UPDATESESSION
    /// <summary>
    /// Called once the bolt network has finished starting called whenever either BoltLauncher.StartServer() or BoltLauncher.StartClient() is done
    /// </summary>
   /* public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer){ CreateLobby(joinableLobby ,roomName ,privateLobby ,password ); }
        else if (BoltNetwork.IsClient)
        {
            //If we are a client connecting decide whether this is a random join or not
            if (randomJoin) { JoinRandomPublicLobby();  }
            //No we want to choose a session so change to browse screen
            else{ ChangeScreenTo("Browse"); }

            //Reset random join to false
            randomJoin = false;
        }
    }*/

    /// <summary>
    /// Creates a lobby with the parameter details
    /// </summary>
   /* public void CreateLobby(bool joinable, string lobbyName, bool priv, string pass)
    {
        roomName = lobbyName;
        privateLobby = priv;
        password = pass;
        joinableLobby = joinable;
        //Uncomment for a private lobby
        //privateLobby = true;
        //password = "Fabcd";

        //Sets up a lobby with public parameters
        var customToken = new ServerInfo();
        customToken.joinableSession = joinable;
        customToken.privateSession = priv;
        customToken.password = pass;
        customToken.versionNumber = versionNo;

        BoltMatchmaking.CreateSession(sessionID: lobbyName, token: customToken);
    }*/
    //Run when the session is created 
   /* public override void SessionCreatedOrUpdated(UdpSession session)
    {
        Debug.Log("Inside SessionCreatedOrUpdated");
        //Change the screen to room
        ChangeScreenTo("Room");
        //Create a lobby player gameobject and assign it to local control
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayerInfo);
        player.TakeControl();
    }
    /// <summary>
    /// Joins any random public lobby
    /// </summary>
    public void JoinRandomPublicLobby()
    {
        //Joins a random lobby, that is public
        var customToken = new ServerInfo();
        customToken.joinableSession = true;
        customToken.privateSession = false;
        customToken.password = "null";
        customToken.versionNumber = versionNo;

        BoltMatchmaking.JoinRandomSession(customToken);
    }*/


    /// <summary>
    /// Lobby becomes unjoinable and unlisted.
    /// </summary>
    /*public void UnjoinableNetwork()
    {
        if (BoltNetwork.IsRunning && BoltNetwork.IsServer)
        {
            var updateToken = new ServerInfo();
            updateToken.joinableSession = false;
            updateToken.privateSession = true;
            string pass = System.DateTime.Now.ToString();
            updateToken.password = pass;
            updateToken.versionNumber = versionNo;

            password = pass;
            privateLobby = true;
            joinableLobby = false;

            //This createes a visual bug where it adds a player to the lobby screen
            BoltMatchmaking.UpdateSession(updateToken);

        }
    }*/
    /// <summary>
    /// Updates session details. This is for in case we allow that in the future.
    /// </summary>
    /*public void UpdateSession(bool joinable, bool priv, string pass)
    {
        if (BoltNetwork.IsRunning && BoltNetwork.IsServer)
        {
            joinableLobby = joinable;
            privateLobby = priv;
            password = pass;
            if (privateLobby == false)
            {
                password = "null";
            }


            var updateToken = new ServerInfo();
            updateToken.joinableSession = joinableLobby;
            updateToken.privateSession = privateLobby;
            updateToken.password = password;
            updateToken.versionNumber = versionNo;


            //This creates a visual bug where it adds a player to the lobby screen
            BoltMatchmaking.UpdateSession(updateToken);
        }
    }*/
    #endregion

    #region CONNECTIONHANDLING
    //Called when either server or client connects. This should only happen in LobbyUIManager
    /*public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.IsClient)
        {
            BoltLog.Info("Connected Client: {0}", connection);
            //Create new player object and add it to room
            LobbyPlayer player = new LobbyPlayer();
            player.connection = connection;
            roomScreen.AddPlayer(player);
            ChangeScreenTo("Room");
        }
        else if (BoltNetwork.IsServer)
        {
            BoltLog.Info("Connected Server: {0}", connection);
            //Create player on the server and assign control to local machine
            BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayerInfo);
            player.AssignControl(connection);
            BoltLog.Info("Server assign control connection: " + connection.ConnectionId);
        }
    }*/

    //Overriden from NetworkConnections
    /*public override void EntityDetached(BoltEntity entity)
    {
        var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
        roomScreen.RemovePlayer(lobbyPlayer);
    }*/
    //Called on disconnect, Overriden from NetworkConnections
   /* public override void Disconnected(BoltConnection connection)
    {
        foreach (var entity in BoltNetwork.Entities)
        {
            //If this entity is not a lobby state or is not the controller of this entity then loop again otherwise remove them
            if (entity.StateIs<ILobbyPlayerInfoState>() == false || entity.IsController(connection) == false) continue;

            var player = entity.GetComponent<LobbyPlayer>();

            if (player)
            {
                player.RemovePlayer();
            }
        }
    }*/
    //Called once new gameobject has been instantiated with the bolt entity component added to it, Overriden from NetworkConnections
    /*public override void EntityAttached(BoltEntity entity)
    {
        //Grab the lobby player component from the gameobject
        LobbyPlayer lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
        //Add player to the room
        roomScreen.AddPlayer(lobbyPlayer);

        if (lobbyPlayer != null)
        {
            //If entity is controlled a.k.a this code is running on client machine that owns this player object
            if (entity.IsControlled)
            {

                lobbyPlayer.SetupPlayer();
            }
            //If this is being run on another client machine or the server
            else
            {
                lobbyPlayer.SetupOtherPlayer();
            }
        }
    }*/
    #endregion

    public void BackToTitleScreen()
    {
        #region Just...justletmekeepthispain
        /*
        Disconnect

        So the issue is that We can't delete anything which is attached. This only happens when we re not host, if we are not hosting and we
        leave the lobby, it will not get rid of the LobbyPlayer UI.
        BoltNetwork.Destroy(FindObjectOfType<LobbyPlayer>().gameObject);
        LobbyPlayer[] t = FindObjectsOfType<LobbyPlayer>();
        foreach (LobbyPlayer lobbyPlayer in t)
        {
        if (lobbyPlayer.entity.HasControl)
        {
        lobbyPlayer.RemovePlayer();
         roomScreen.RemovePlayer(lobbyPlayer);
        BoltNetwork.Destroy(lobbyPlayer.gameObject);//Same as reason above CLient can't destroy the object that the host has created

        }
        }
        
        entity.owner is the host
        foreach (var entity in BoltNetwork.Entities)
        {
            if (entity.IsOwner)
            {
                Debug.LogError("OWNER OF: " + entity.gameObject.name + " | HAS CONTROL: " + entity.HasControl + " | NETWORK ID : " + entity.NetworkId);//entity.TakeControl);
            }
            else
            {
                Debug.LogError("NOT OWNER OF: " + entity.gameObject.name + " | HAS CONTROL: " + entity.HasControl + " | NETWORK ID : " + entity.NetworkId);
            }
        }
        foreach (GameObject item in BoltNetwork.SceneObjects)
        {
          BoltNetwork.Destroy(item);
        }
          BoltNetwork.Destroy(entity);
        var et = entity.GetComponent<LobbyPlayer>();
         et.RemovePlayer();

        BoltNetwork.Destroy(et.gameObject);
         }
        BoltNetwork.Destroy()
        */
        #endregion

        FindObjectOfType<AudioManager>().PlaySound("Click");

        //If we are not counting down to play, we will disconnect and/or return to the create screen or/and return to the title
        for (int i = 0; i < screens.Length; i++)
        {
            //If not on create screen return to create screen, otherwise go to TitleScene
            if (screens[i].screenName == "Browse" && screens[i].screen.activeInHierarchy)
            {
                BoltNetwork.Shutdown();
                ChangeScreenTo("Create");
                return;
            }
        }
        if (!isCountdown)
        {

            //In Lobby
            if (BoltNetwork.IsConnected || BoltNetwork.IsServer)
            {
                BoltNetwork.Shutdown();
            }

            for (int i = 0; i < screens.Length; i++)
            {
                //If not on create screen return to create screen, otherwise go to TitleScene
                if (screens[i].screenName == "Create" && screens[i].screen.activeInHierarchy)
                {
                    BoltNetwork.Shutdown();
                    SceneManager.LoadScene("TitleScene");
                    return;
                }
            }
            //Invoke("ChangeScreenTo(\"Create\")", 2);

            ChangeScreenTo("Create");
        }

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
