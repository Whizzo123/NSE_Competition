using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UdpKit;

public class ServerListRoomUI : MonoBehaviour
{
    public Action OnJoinRoomClicked;

    public Text roomNameText;
    public Text concurrentPlayersText;
    private Button joinRoom;

    void Start()
    {
        joinRoom = GetComponent<Button>();
        joinRoom.onClick.RemoveAllListeners();
        joinRoom.onClick.AddListener(() => 
        {
            if (OnJoinRoomClicked != null) OnJoinRoomClicked();
        });
    }

    public void Populate(UdpSession match, Color backgroundColor, Action clickAction)
    {
        roomNameText.text = match.HostName;
        concurrentPlayersText.text = string.Format("{0}/{1}", match.ConnectionsCurrent, match.ConnectionsMax);

        OnJoinRoomClicked += clickAction;
    }
}
