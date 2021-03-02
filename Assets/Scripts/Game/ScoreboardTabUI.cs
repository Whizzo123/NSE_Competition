using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardTabUI : MonoBehaviour
{

    public Text playerName;
    private string storedName;
    public Text playerScore;

    public void EditName(string name)
    {
        playerName.text = name;
        storedName = name;
    }

    public void EditScore(int score)
    {
        playerScore.text = "" + score;
    }

    public string GetName()
    {
        return storedName;
    }

}
