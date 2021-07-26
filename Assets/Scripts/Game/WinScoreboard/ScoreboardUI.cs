using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Container for in game scoreboard, and updates and reshuffles the scoreboard.
/// </summary>
public class ScoreboardUI : MonoBehaviour
{

    [Tooltip("All the player elements for scoreboard")]private List<ScoreboardTabUI> tabs;
    [Tooltip("The Gameobject element for tabs")]public GameObject tabPrefab;

    void Start()
    {
        tabs = new List<ScoreboardTabUI>();
    }

    void LateUpdate()
    {
        foreach (ScoreboardTabUI tabUI in tabs)
        {
            UpdateBoard(tabUI.GetName());
        }
    }

    /// <summary>
    /// Updates the score board with name and score
    /// </summary>
    public void UpdateBoard(string name)
    {
        ScoreboardTabUI tab = GetTab(name);
        tab.EditName(name);
        tab.EditScore(FindObjectOfType<Stash>().FindScoreForPlayer(name));

        ReshuffleScoreboard();
    }

    /// <summary>
    /// Reshuffles the board to display the players score in descending order
    /// </summary>
    private void ReshuffleScoreboard()
    {
        int siblingIndex = 0;
        List<ScoreboardTabUI> children = transform.GetComponentsInChildren<ScoreboardTabUI>().ToList<ScoreboardTabUI>();

        for (int i = 0; i < transform.childCount; i++)
        {
            int maxScore = 0;
            int index = 0;
            //Compares all current players to find max
            for (int j = 0; j < children.Count; j++)
            {
                if(Int32.Parse(children[j].playerScore.text) > maxScore)
                {
                    maxScore = Int32.Parse(children[j].playerScore.text);
                    index = j;
                }
            }
            //Removes current max player for next loop
            children[index].transform.SetSiblingIndex(siblingIndex);
            children.RemoveAt(index);
            siblingIndex++;
        }
    }

    /// <summary>
    /// Grab uielement for on scoreboard that is linked to player name
    /// </summary>
    public ScoreboardTabUI GetTab(string name)
    {
        foreach (ScoreboardTabUI item in tabs)
        {
            if(item.GetName() == name)
            {
                return item;
            }
        }
        //JoeComment
        GameObject newTab = (GameObject)Instantiate(tabPrefab, transform);
        tabs.Add(newTab.GetComponent<ScoreboardTabUI>());
        return newTab.GetComponent<ScoreboardTabUI>();
    }
}
