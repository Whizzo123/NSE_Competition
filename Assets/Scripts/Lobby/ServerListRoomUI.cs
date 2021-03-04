using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UdpKit;

public class ServerListRoomUI : MonoBehaviour
{

    #region Variables
    public Action OnJoinRoomClicked;

    public Text roomNameText;
    public Text concurrentPlayersText;
    private Button joinRoom;

    #endregion 

    void Start()
    {
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
    /// <param name="clickAction"></param>
    public void Populate(UdpSession match, Color backgroundColor, Action clickAction)
    {
        roomNameText.text = match.HostName;
        concurrentPlayersText.text = string.Format("{0}/{1}", match.ConnectionsCurrent, match.ConnectionsMax);

        OnJoinRoomClicked += clickAction;
    }
}
