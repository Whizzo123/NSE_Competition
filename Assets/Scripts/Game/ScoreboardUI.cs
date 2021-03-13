using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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
