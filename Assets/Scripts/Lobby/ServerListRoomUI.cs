using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Steamworks;
using Mirror.Discovery;

public class ServerListRoomUI : MonoBehaviour
{

    #region Variables
    public Action OnJoinRoomClicked;

    public Text roomNameText;
    public Text concurrentPlayersText;
    private Button joinRoom;

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
    /// Create uiElement for server in the server list
    /// </summary>
    /// <param name="match"></param>
    /// <param name="backgroundColor"></param>
    public void Populate(LobbyInfo info, Color backgroundColor)
    {
        roomNameText.text = info.lobbyName;
        concurrentPlayersText.text = string.Format("{0}/{1}", info.playerCount, FindObjectOfType<MyNetworkManager>().maxConnections);
        OnJoinRoomClicked += () => JoinSteamLobby(info.lobbyID);
    }

    public void Populate(ServerResponse response, Color backgroundColor)
    {
        roomNameText.text = response.EndPoint.Address.ToString();
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
