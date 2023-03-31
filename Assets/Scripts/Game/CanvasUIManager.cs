﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using T_Utils;
//Todo: Come back and think about the effectiveness of this method compared to compartmentalising everyhting instead.
//It just seems weird having all object stored in one script and activated in other scripts if they need to.
//For example, the winscreen I feel like should be contained within a EndGame script where it controls what happens when the game ends.
//The playerTrackIcon would be inside the actual ability script instead.

/// <summary>
/// Contains many, ui objects that can be activated by other scripts on events.
/// </summary>
public class CanvasUIManager : MonoBehaviour
{
    public Canvas canvas;
    public static GameObject playerNameTextPrefab;
    public GameObject artefactPickupPopup;
    public GameObject popupMessage;
    public GameObject hintMessage;
    public PlayerInventoryUI inventoryUI;
    public ScoreboardUI scoreboardUI;
    public float popupShowTime;
    public float popupMessageShowTime;
    public GameObject playerTextContainer;
    public GameObject loadoutScreen;
    public GameObject targetIconGO;
    public PlayerTrackIconUI playerTrackIcon;
    public WinScreenUI winScreen;
    public Text TimeText;
    public Text loadoutTimeText;
    public GameObject pauseMenu;

    /// <summary>
    /// Show a hint message with no time associated to it's visibility. Manually close it with <see cref="CloseHintMessage"/>.
    /// </summary>
    public void ShowHintMessage(string message)
    {
        hintMessage.SetActive(true);
        hintMessage.GetComponent<Text>().text = message;
    }

    /// <summary>
    /// Closes the hintMessage ui text
    /// </summary>
    public void CloseHintMessage()
    {
        hintMessage.SetActive(false);
    }

    /// <summary>
    /// Pops up message for 2 seconds before disappearing.
    /// </summary>
    /// <param name="message"></param>
    public void PopupMessage(string message)
    {
        popupMessage.SetActive(true);
        popupMessage.GetComponent<Text>().text = message;
        GenericTimer.Create(() => { popupMessage.SetActive(false); }, 2.0f, "PopupMessageTimer");
    }

    public void AddToInventoryScreen(ItemArtefact artefact)
    {
        inventoryUI.AddInventoryItem(artefact);
    }

    public void RemoveFromInventoryScreen(ItemArtefact artefact)
    {
        inventoryUI.UpdateInventoryItem(artefact, false);
    }

    public void SetTimeText(int timeLeft)
    {
        int mins = timeLeft / 60;
        int seconds = timeLeft % 60;
        TimeText.text = mins + ":" + seconds;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }
    }
    public void LeaveLobby()
    {
        MyNetworkManager.singleton.StopClient();
    }
}
