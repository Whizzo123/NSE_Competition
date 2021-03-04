using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardTabUI : MonoBehaviour
{
    #region Variables
    public Text playerName;
    private string storedName;
    public Text playerScore;
    #endregion 

    /// <summary>
    /// Called in order to change name on scoreboard item uielement
    /// </summary>
    /// <param name="name"></param>
    public void EditName(string name)
    {
        playerName.text = name;
        storedName = name;
    }

    /// <summary>
    /// Called in order to edit score on scoreboard item uielement
    /// </summary>
    /// <param name="score"></param>
    public void EditScore(int score)
    {
        playerScore.text = "" + score;
    }

    /// <summary>
    /// Called in order to get the players name linked to that scoreboard item uielement
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        return storedName;
    }

}
