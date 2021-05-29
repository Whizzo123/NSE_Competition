using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;

using UnityEngine.SceneManagement;

public class LobbyUIManager : GlobalEventListener
{
    #region Variables
    [SerializeField]
    public UIScreens[] screens;

    private CreateScreenUI createScreen;
    private BrowseScreenUI browseScreen;
    private RoomScreenUI roomScreen;

    public string gameSceneName;
    private string roomName;

    [Tooltip("Minimum Required Players To Start")]
    [SerializeField] private int minPlayers = 2;

    [Tooltip("Time in second between all players ready & match start")]
    [SerializeField]
    public float prematchCountdown = 5.0f;

    private bool randomJoin;
    public bool isCountdown = false;

    public GameObject playerLobbyPrefab;

    #endregion

    void Start()
    {
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
        roomScreen = FindScreenByName("Room").screen.GetComponent<RoomScreenUI>();
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
        browseScreen.OnClickJoinSession += JoinSessionEvent;
    }
    /// <summary>
    /// Called when creating a room on the network
    /// </summary>
    private void CreateRoomSession()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        this.roomName = createScreen.inputField.text;
        Debug.Log("CreatingRoomSession");
        //Launches server
        BoltLauncher.StartServer();
    }
    private void SwapToBrowseScreen()
    {
        //In order for client to view session list we need to connect them to the network
        FindObjectOfType<AudioManager>().PlaySound("Click");
        BoltLauncher.StartClient();
    }
    private void JoinRandomSession()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        //Set random join to true
        randomJoin = true;
        //Launch the client
        BoltLauncher.StartClient();
    }
    private void JoinSessionEvent(UdpSession session)
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        //Chech whether this is the client should only run on a client
        if (BoltNetwork.IsClient)
        {
            //Join given session we choose from the list
            BoltMatchmaking.JoinSession(session);
        }
    }


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
                isCountdown = true;
                StartCoroutine(ServerCountdownCoroutine());
                //Set connection to private
                //BoltMatchmaking.CurrentSession.ConnectionsMax;
            }
        }
    }

    /// <summary>
    /// Countdown through prematch countdown then load up game scene
    /// </summary>
    /// <returns></returns>
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
                countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
                countdown.Time = floorTime;
                countdown.Send();
            }
        }
        //Once we are basically at zero send a final one
        countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
        countdown.Time = 0;
        countdown.Send();
        //Tell network to load game scene for everyone
        BoltNetwork.LoadScene(gameSceneName);
    }



    /// <summary>
    /// Called once the bolt network has finished starting called whenever either BoltLauncher.StartServer() or BoltLauncher.StartClient() is done
    /// </summary>
    public override void BoltStartDone()
    {
        //Once either BoltLauncher.StartServer() or BoltLauncher.StartClient() has run
        if (BoltNetwork.IsServer)
        {
            //If we are the server create the session using the room name
            BoltMatchmaking.CreateSession(sessionID: roomName);

            //Add a way to create a private session
        }
        else if(BoltNetwork.IsClient)
        {
            //If we are a client connecting decide whether this is a random join or not
            if(randomJoin)
            {
                //Yes a random join tell Bolt to join a random session
                BoltMatchmaking.JoinRandomSession();
            }
            else
            {
                //No we want to choose a session so change to browse screen
                ChangeScreenTo("Browse");
            }
            //Reset random join to false
            randomJoin = false;
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

    //Run when the session is created 
    public override void SessionCreatedOrUpdated(UdpSession session)
    {
        Debug.Log("Inside SessionCreatedOrUpdated");
        //Change the screen to room
        ChangeScreenTo("Room");
        //Create a lobby player gameobject and assign it to local control
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayerInfo);
        player.TakeControl();
    }

    //Called when either server or client connects
    public override void Connected(BoltConnection connection)
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
    }

    //Called once new gameobject has been instantiated with the bolt entity component added to it
    public override void EntityAttached(BoltEntity entity)
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
    }

    private void ChangeScreenTo(string name)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].screen.SetActive(false);
            if(screens[i].screenName == name)
            {
                screens[i].screen.SetActive(true);
            }
        }
    }

    //Called once entity is detached from bolt gameobject most likely destroyed
    public override void EntityDetached(BoltEntity entity)
    {
        var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
        roomScreen.RemovePlayer(lobbyPlayer);
    }

    //Called on disconnect
    public override void Disconnected(BoltConnection connection)
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


    }
    

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
}


[Serializable]
public struct UIScreens
{
    public string screenName;
    public GameObject screen;
}
