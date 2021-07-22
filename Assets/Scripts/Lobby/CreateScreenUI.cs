using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


//Todo: Rename Class?
/// <summary>
/// Manages and adds listeners to the create, browse, and rand join buttons.
/// </summary>
public class CreateScreenUI : MonoBehaviour
{
    #region Variables
    [Header("Create")]
    [Tooltip("Action to create a session")] public Action OnCreateButtonClick;
    [Tooltip("Name of room")] public InputField inputField;    //Todo: Rename variable 
    public Button createButton;
    [Space]

    [Header("Browse")]
    [Tooltip("Action to browse sessions")] public Action OnBrowseButtonClick;
    public Button browseButton;
    [Space]

    [Header("Random Join")]
    [Tooltip("Action to join a random session")] public Action OnRandomButtonClick;
    public Button randomButton;

    [Tooltip("Name of player")] public InputField playerNameInput;//Necessary?


    #endregion

    /// <summary>
    /// When create screen is switched to, adds Actions to the buttons.
    /// </summary>
    public void OnEnable()
    {
        createButton.onClick.RemoveAllListeners();
        createButton.onClick.AddListener(() =>
        {
            if (OnCreateButtonClick != null) OnCreateButtonClick();
        });
        browseButton.onClick.RemoveAllListeners();
        browseButton.onClick.AddListener(() =>
        {
            if (OnBrowseButtonClick != null) OnBrowseButtonClick();
        });
        randomButton.onClick.RemoveAllListeners();
        randomButton.onClick.AddListener(() =>
        {
            if (OnRandomButtonClick != null) OnRandomButtonClick();
        });
    }
}


//        OnJoinRoomClicked += () => JoinSteamLobby(info.lobbyID);
// randomButton.onClick.AddListener(() => { if (OnRandomButtonClick != null) OnRandomButtonClick(); })