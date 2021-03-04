using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyPlayersInfo : MonoBehaviour
{
    #region Variables
    public Text playerNameText;
    public Button readyButton;

    public Color notReady;
    public Color ready;

    private LobbyPlayer myPlayer;
    private Action OnReadyButtonClicked;
    #endregion 

    public void Populate(LobbyPlayer player)
    {
        myPlayer = player;
        playerNameText.text = myPlayer.playerName;
        ChangeButtonColor(notReady);
        OnReadyButtonClicked += ReadyButtonClick;
        readyButton.onClick.AddListener(() =>
        {
            if (OnReadyButtonClicked != null) OnReadyButtonClicked();
        });
    }

    private void ChangeButtonColor(Color color)
    {
        readyButton.gameObject.GetComponent<Image>().color = color;
    }

    private void ReadyButtonClick()
    {
        if(myPlayer.isReady)
        {
            myPlayer.isReady = false;
            ChangeButtonColor(notReady);
        }
        else if(!myPlayer.isReady)
        {
            myPlayer.isReady = true;
            ChangeButtonColor(ready);
        }
    }

}
