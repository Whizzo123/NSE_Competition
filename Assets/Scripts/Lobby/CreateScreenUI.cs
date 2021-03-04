using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CreateScreenUI : MonoBehaviour
{
    #region Variables
    public Action OnCreateButtonClick;
    public Action OnBrowseButtonClick;
    public Action OnRandomButtonClick;

    public InputField inputField;
    public InputField playerNameInput;
    public Button createButton;
    public Button browseButton;
    public Button randomButton;
    #endregion

    /// <summary>
    /// When create screen is switched to
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
