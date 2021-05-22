using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinElementUI : MonoBehaviour
{

    public Text nameText;
    public Text positionText;
    public Text scoreText;

    public void PopulateFields(string name, int position, int score)
    {
        nameText.text = name;
        positionText.text = position + ".";
        scoreText.text = score + "";
    }

}
