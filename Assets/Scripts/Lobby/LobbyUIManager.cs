using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;

public class LobbyUIManager : GlobalEventListener
{
    [SerializeField]
    public UIScreens[] screens;

    public string gameSceneName;

    private CreateScreenUI createScreen;
    private BrowseScreenUI browseScreen;
    private RoomScreenUI roomScreen;
    private string roomName;
    private string playerName;

    [SerializeField] private int minPlayers = 2;


    [Tooltip("Time in second between all players ready & match start")]
    [SerializeField]
    private float prematchCountdown = 5.0f;

    private bool randomJoin;
    private bool isCountdown = false;

    public GameObject playerLobbyPrefab;

    void Start()
    {
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
        randomJoin = false;
        StartUI();
    }

    private void StartUI()
    {
        createScreen.OnCreateButtonClick += CreateRoomSession;
        createScreen.OnBrowseButtonClick += SwapToBrowseScreen;
        createScreen.OnRandomButtonClick += JoinRandomSession;
        browseScreen.OnClickJoinSession += JoinSessionEvent;
    }

    private void FixedUpdate()
    {
        if (BoltNetwork.IsServer && isCountdown == false)
        {
            VerifyReady();
        }
    }

    private void VerifyReady()
    {
        var allReady = true;
        var readyCount = 0;

        foreach (var entity in BoltNetwork.Entities)
        {
            if (entity.StateIs<ILobbyPlayerInfoState>() == false) continue;

            var lobbyPlayer = entity.GetState<ILobbyPlayerInfoState>();

            allReady &= lobbyPlayer.Ready;

            if (allReady == false) break;
            readyCount++;
        }

        if (allReady && readyCount >= minPlayers)
        {
            isCountdown = true;
            StartCoroutine(ServerCountdownCoroutine());
        }
    }

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

                countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
                countdown.Time = floorTime;
                countdown.Send();
            }
        }

        countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
        countdown.Time = 0;
        countdown.Send();

        BoltNetwork.LoadScene(gameSceneName);
    }

    private void CreateRoomSession()
    {
        this.roomName = createScreen.inputField.text;
        Debug.Log("CreatingRoomSession");
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            BoltMatchmaking.CreateSession(sessionID: roomName);
        }
        else if(BoltNetwork.IsClient)
        {

            if(randomJoin)
            {

            }
            else
            {
                ClientStaredUIHandler();
            }

            randomJoin = false;
        }
    }

    private void SwapToBrowseScreen()
    {
        BoltLauncher.StartClient();
    }

    private void JoinRandomSession()
    {
        randomJoin = true;
        BoltLauncher.StartClient();
    }

    private void JoinSessionEvent(UdpSession session)
    {
        JoinEventHandler(session);
    }

    private void JoinEventHandler(UdpSession session)
    {
        if (BoltNetwork.IsClient)
        {
            BoltMatchmaking.JoinSession(session);
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

    public override void SessionCreatedOrUpdated(UdpSession session)
    {
        Debug.Log("Inside SessionCreatedOrUpdated");
        SessionCreatedUIHandler(session);
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayerInfo);
        player.TakeControl();
    }

    public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.IsClient)
        {
            BoltLog.Info("Connected Client: {0}", connection);
            ClientConnectedUIHandler();
        }
        else if (BoltNetwork.IsServer)
        {
            BoltLog.Info("Connected Server: {0}", connection);

            BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayerInfo);
            player.AssignControl(connection);
        }
    }

    public override void EntityAttached(BoltEntity entity)
    {
        LobbyPlayer lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
        roomScreen.AddPlayer(lobbyPlayer);

        if (lobbyPlayer != null)
        {
            if (entity.IsControlled)
            {
                lobbyPlayer.SetupPlayer();
            }
            else
            {
                lobbyPlayer.SetupOtherPlayer();
            }
        }
    }

    private void ClientConnectedUIHandler()
    {
        LobbyPlayer player = new LobbyPlayer();
        player.playerName = playerName;
        roomScreen.AddPlayer(player);
        ChangeScreenTo("Room");
    }

    private void SessionCreatedUIHandler(UdpSession session)
    {
        ChangeScreenTo("Room");   
    }

    private void ClientStaredUIHandler()
    {
        ChangeScreenTo("Browse");
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

    public override void EntityDetached(BoltEntity entity)
    {
        var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
        roomScreen.RemovePlayer(lobbyPlayer);
    }

    public override void Disconnected(BoltConnection connection)
    {
        foreach (var entity in BoltNetwork.Entities)
        {
            if (entity.StateIs<ILobbyPlayerInfoState>() == false || entity.IsController(connection) == false) continue;

            var player = entity.GetComponent<LobbyPlayer>();

            if (player)
            {
                player.RemovePlayer();
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
