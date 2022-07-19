﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The individual player element on the winning scoreboard. Contains information for text and a function for populating it.
/// </summary>
public class WinElementUI : MonoBehaviour
{

    [Tooltip("Name of player")] public Text nameText;
    [Tooltip("Position on leaderboard (#1, #2, #3)")]public Text positionText;
    [Tooltip("Score of player")]public Text scoreText;

    /// <summary>
    /// Populates the WinElement with a name and score and position on leaderboard
    /// </summary>
    public void PopulateFields(string name, int position, int score)
    {
        nameText.text = name;
        positionText.text = position + ".";
        scoreText.text = score + "";
    }

}