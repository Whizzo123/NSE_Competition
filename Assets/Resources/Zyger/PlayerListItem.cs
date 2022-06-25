using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public int connectionID;
    public ulong playerSteamId;
    private bool avatarRecieved;

    public Text playerNameText;
    public RawImage playerIcon;

    public Image playerStatusImage;
    public bool ready;
    //protected Callback<AvatarImageLoaded_t> imageLoaded;

    public void SetPlayerValues()
    {
        playerNameText.text = playerName;
        ChangeReadyStatus();
    }

    public void ChangeReadyStatus()
    {
        if (ready)
        {
            playerStatusImage.color = Color.green;
        }
        else
        {
            playerStatusImage.color = Color.red;

        }
    }
}
