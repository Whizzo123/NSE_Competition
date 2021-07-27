﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Steamworks;
using Mirror.Discovery;

/// <summary>
/// The UI Element (Button) for each session. Also has joining functions.
/// </summary>
public class ServerListRoomUI : MonoBehaviour
{

    #region Variables
    [SerializeField] [Tooltip("Action for when user wants to join")] public Action OnJoinRoomClicked;
    //Maybe have an action for clicksession, this would display any information about the session, such as map, time etc if we decide to inlcude that in the future

    [Header("Room element properties")]
    [SerializeField] [Tooltip("Room name, not unique")] public Text roomNameText;
    [SerializeField] [Tooltip("No. of players in lobby out of max players")] public Text concurrentPlayersText;
    [SerializeField] [Tooltip("Button to join")] private Button joinRoom;//As said above, we would have to have a sessionButton to go along with the joinRoom button
    //The joinRoom button could be there always or it could be viewed when expanded from the sessionButton
    [Space]

    private LobbyUIManager lobbyUIManager;

    #endregion 


    void Start()
    {
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();
        joinRoom = GetComponent<Button>();
        joinRoom.onClick.RemoveAllListeners();
        joinRoom.onClick.AddListener(() =>
        { 
            if (OnJoinRoomClicked != null) OnJoinRoomClicked();
        });
    }
    /// <summary>
    /// Updates the UIElement with info and color. Adds a JoinSteamLobby action to the button.
    /// </summary>
    public void Populate(LobbyInfo info, Color backgroundColor)
    {
        roomNameText.text = info.lobbyName;
        concurrentPlayersText.text = string.Format("{0}/{1}", info.playerCount, FindObjectOfType<MyNetworkManager>().maxConnections);
        OnJoinRoomClicked += () => JoinSteamLobby(info.lobbyID);
    }
    /// <summary>
    /// Updates the UIElement with info and color. Adds a JoinMirrorLobby action to the button.
    /// </summary>
    public void Populate(ServerResponse response, Color backgroundColor)
    {
        roomNameText.text = response.EndPoint.Address.ToString();
        //Setup a call back here that when clicked in the server list we call our join mirror lobby function which then decides whether we are joining through 
        //SteamMatchmaking or just a LAN connection
        OnJoinRoomClicked += () => JoinMirrorLobby(lobbyUIManager.discoveredServers[response.serverId]);
    }


    private void JoinSteamLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    private void JoinMirrorLobby(ServerResponse response)
    {
        MyNetworkManager.singleton.StartClient(response.uri);
        lobbyUIManager.ChangeScreenTo("Room");
    }
}
