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

    private CreateScreenUI createScreen;
    private BrowseScreenUI browseScreen;
    private RoomScreenUI roomScreen;
    private string roomName;
    private string playerName;

    private bool randomJoin;

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


    private void CreateRoomSession()
    {
        if (createScreen.playerNameInput.text == "")
            return;
        else
            playerName = createScreen.playerNameInput.text;
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
        if (createScreen.playerNameInput.text == "")
            return;
        else
            playerName = createScreen.playerNameInput.text;
        BoltLauncher.StartClient();
    }

    private void JoinRandomSession()
    {
        if (createScreen.playerNameInput.text == "")
            return;
        else
            playerName = createScreen.playerNameInput.text;
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
        LobbyPlayer player = new LobbyPlayer();
        player.playerName = playerName;
        roomScreen.AddPlayer(player);
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

            //var entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerInfo);
            //entity.AssignControl(connection);
            LobbyPlayer player = new LobbyPlayer();
            player.playerName = playerName;
            roomScreen.AddPlayer(player);
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

}

[Serializable]
public struct UIScreens
{
    public string screenName;
    public GameObject screen;
}
