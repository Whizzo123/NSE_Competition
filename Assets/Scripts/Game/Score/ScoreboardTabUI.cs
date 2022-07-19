using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//Todo: Maybe change name convention with an element suffix, inherit from monobehaviour needed?

/// <summary>
/// The individual element for the ongoing game scoreboard. Contains information regarding player and score.
/// </summary>
public class ScoreboardTabUI : MonoBehaviour
{
    #region Variables
    [Tooltip("The text element for player name")]public Text playerName;
    [Tooltip("Thhe stored string for player name")] private string storedName;
    [Tooltip("The text element for player score")] public Text playerScore;
    #endregion 

    /// <summary>
    /// Called in order to change name on scoreboard item uielement
    /// </summary>
    public void EditName(string name)
    {
        playerName.text = name;
        storedName = name;
    }

    /// <summary>
    /// Called in order to edit score on scoreboard item uielement
    /// </summary>
    public void EditScore(int score)
    {
        playerScore.text = "" + score;
    }

    /// <summary>
    /// Called in order to get the players name linked to that scoreboard item uielement
    /// </summary>
    public string GetName()
    {
        return storedName;
    }

}
