using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Steamworks;
using Mirror.Discovery;

/// <summary>
/// Script to attatch to a server room prefab.
/// <para>Edits the information and initiates joining</para>
/// </summary>
public class ServerListRoomUI : MonoBehaviour
{

    #region Variables
    [SerializeField] [Tooltip("Action for when user wants to join")] public Action OnJoinRoomClicked;
    //Maybe have an action for clicksession, this would display any information about the session, such as map, time etc if we decide to inlcude that in the future

    [Header("Room element properties")]
    [SerializeField] [Tooltip("Room name, not unique")] public Text roomNameText;
    [SerializeField] [Tooltip("No. of players in lobby out of max players")] public Text concurrentPlayersText;
    [SerializeField] [Tooltip("Button to join")] public Button joinRoom;//As said above, we would have to have a sessionButton to go along with the joinRoom button
    //The joinRoom button could be there always or it could be viewed when expanded from the sessionButton
    [Space]

    private LobbyUIManager lobbyUIManager;
    private ServerResponse my_response;

    #endregion 


    void Awake()
    {
        lobbyUIManager = GameObject.FindObjectOfType<LobbyUIManager>();
        
    }
    /// <summary>
    /// Updates the UIElement with info and color. Adds a JoinSteamLobby action to the button.
    /// </summary>
    public void Populate(LobbyInfo info, Color backgroundColor)
    {
        roomNameText.text = info.lobbyName;
        concurrentPlayersText.text = string.Format("{0}/{1}", info.playerCount, FindObjectOfType<MyNetworkManager>().maxConnections);

        //Add listener to buttons
        OnJoinRoomClicked += () => JoinSteamLobby(info.lobbyID);
        joinRoom.onClick.AddListener(() =>
        {
            if (OnJoinRoomClicked != null) OnJoinRoomClicked();
        });
    }

    private void JoinSteamLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }
    public void RandomFunction()
    {
        Debug.Log("This is the random function");
    }

}
