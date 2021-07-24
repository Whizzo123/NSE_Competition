using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct LeaderboardScore
{
    public string name;
    public int score;
}

/// <summary>
/// 
/// </summary>
public class MyLobbyCountdown : NetworkBehaviour
{
    [Header("Variables")]
    public float lobbyTime;
    private float currentLobbyTime;
    public float gameTime;
    private float currentGameTime;
   

    // Update is called once per frame
    void Start()
    {
        currentLobbyTime = lobbyTime;
        currentGameTime = gameTime;
        StartCoroutine(Countdown());
    }

    [Server]
    private IEnumerator Countdown()
    {
        Debug.Log("InsideCountdown");
        var floorTime = Mathf.FloorToInt(lobbyTime);

        while (currentLobbyTime > 0)
        {
            yield return null;

            currentLobbyTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(currentLobbyTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                UpdateCountdown(floorTime);
            }
        }
        DisableLoadoutScreen();
        StartCoroutine(GameTimer());
    }

    [Server]
    private IEnumerator GameTimer()
    {
        var floorTime = Mathf.FloorToInt(gameTime);

        while(currentGameTime > 0)
        {
            yield return null;

            currentGameTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(currentGameTime);

            if(newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                UpdateGameClock(floorTime);
            }
        }
        EndGame();
    }

    [ClientRpc]
    private void DisableLoadoutScreen()
    {
        FindObjectOfType<AbilitySlotBarUI>().LoadInAbilitiesFromLoadout(FindObjectOfType<LoadoutBarUI>().GetLoadoutForAbilitySlotBar());
        FindObjectOfType<CanvasUIManager>().loadoutScreen.SetActive(false);
        NetworkClient.localPlayer.GetComponent<PlayerController>().SetLoadoutReleased(true);
    }

    [ClientRpc]
    private void UpdateCountdown(int time)
    {
        FindObjectOfType<CanvasUIManager>().loadoutTimeText.text = "" + time;
    }

    [ClientRpc]
    private void UpdateGameClock(int time)
    {
        FindObjectOfType<CanvasUIManager>().SetTimeText(time);
    }

    [ClientRpc]
    private void EndGame()
    {
        Dictionary<string, int> scores = FindObjectOfType<Stash>().GetStashedScores();
        int countOfScoringPlayers = scores.Count;
        LeaderboardScore[] results = new LeaderboardScore[scores.Count];
        for (int i = 0; i < countOfScoringPlayers; i++)
        {
            int lastMaxScore = int.MinValue;
            string highestScoring = null;
            foreach (string key in scores.Keys)
            {
                Debug.Log("Testing score: " + key + " with a score of " + scores[key]);
                Debug.Log("Against lastMaxScore of: " + lastMaxScore);
                if (scores[key] > lastMaxScore)
                {
                    Debug.Log("Setting as min score: " + scores[key] + " : " + key);
                    highestScoring = key;
                    lastMaxScore = scores[key];
                }
            }
            if (highestScoring != null)
            {
                for (int j = 0; j < results.Length; j++)
                {
                    if (results[j].name == null || results[j].name == string.Empty)
                    {
                        Debug.Log("Adding into results: " + highestScoring);
                        results[j].name = highestScoring;
                        results[j].score = FindObjectOfType<Stash>().FindScoreForPlayer(highestScoring);
                        break;
                    }
                    else
                    {
                        Debug.Log("Getting results[" + j + "]: " + results[j].name);
                    }
                }
                scores.Remove(highestScoring);
            }
        }
        FindObjectOfType<CanvasUIManager>().winScreen.gameObject.SetActive(true);
        for (int i = 0; i < results.Length; i++)
        {
            Debug.Log("Adding to results: " + results[i].name + " with score of: " + results[i].score);
            FindObjectOfType<CanvasUIManager>().winScreen.AddToContent(results[i].name, results[i].score);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdCallGameToFinish()
    {
        EndGame();
    }
}
