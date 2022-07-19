using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Spawns the winning scoreboard 
/// </summary>
public class WinScreenUI : MonoBehaviour
{
    [Tooltip("The container for the player scoreboard elements")] public GameObject winElementPrefab;
    [Tooltip("Container UI for win leaderboard")] public GameObject content;

    /// <summary>
    /// Spawns an element on the leaderboards and populates it with parameter information.
    /// </summary>
    public void AddToContent(string name, int points)
    {
        GameObject go = Instantiate(winElementPrefab, content.transform);
        go.GetComponent<WinElementUI>().PopulateFields(name, content.transform.childCount, points);
    }

}
