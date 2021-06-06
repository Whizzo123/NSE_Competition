using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ScoreboardUI : MonoBehaviour
{

    private List<ScoreboardTabUI> tabs;
    public GameObject tabPrefab;

    void Start()
    {
        tabs = new List<ScoreboardTabUI>();
    }

    void Update()
    {
        foreach (ScoreboardTabUI tabUI in tabs)
        {
            UpdateBoard(tabUI.GetName());
        }
    }

    /// <summary>
    /// Updates the score board with name and score
    /// </summary>
    /// <param name="name"></param>
    /// <param name="score"></param>
    public void UpdateBoard(string name)
    {
        ScoreboardTabUI tab = GetTab(name);
        tab.EditName(name);
        tab.EditScore(FindObjectOfType<Stash>().FindScoreForPlayer(name));
        ReshuffleScoreboard();
    }

    private void ReshuffleScoreboard()
    {
        int siblingIndex = 0;
        List<ScoreboardTabUI> children = transform.GetComponentsInChildren<ScoreboardTabUI>().ToList<ScoreboardTabUI>();
        for (int i = 0; i < transform.childCount; i++)
        {
            int maxScore = 0;
            int index = 0;
            for (int j = 0; j < children.Count; j++)
            {
                if(Int32.Parse(children[j].playerScore.text) > maxScore)
                {
                    maxScore = Int32.Parse(children[j].playerScore.text);
                    index = j;
                }
            }
            children[index].transform.SetSiblingIndex(siblingIndex);
            children.RemoveAt(index);
            siblingIndex++;
        }
    }

    /// <summary>
    /// Grab uielement for on scoreboard that is linked to player name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ScoreboardTabUI GetTab(string name)
    {
        foreach (ScoreboardTabUI item in tabs)
        {
            if(item.GetName() == name)
            {
                return item;
            }
        }
        GameObject newTab = (GameObject)Instantiate(tabPrefab, transform);
        tabs.Add(newTab.GetComponent<ScoreboardTabUI>());
        return newTab.GetComponent<ScoreboardTabUI>();
    }
}
