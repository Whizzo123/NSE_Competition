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

    public void UpdateBoard(string name, int score)
    {
        ScoreboardTabUI tab = GetTab(name);
        tab.EditName(name);
        tab.EditScore(score);
    }

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
