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
/// Handles all the UI and logic for any in game timers(loadout and game timer). Also handles the scoreboard logic at the end of the game.
/// </summary>
public class TheCountdown : NetworkBehaviour
{
    //Todo: Rename variables, I assume lobby time isn't the actual lobby
    [Header("Variables")]
    public float winScreenDisplayTime;
    private float currentWinScreenDisplayTime;
    [SerializeField] [Tooltip("Total loadout menu time")] public float lobbyTime;
    [Tooltip("Time elapsed in loadout menu")] private float currentLobbyTime;
    [SerializeField] [Tooltip("Total game time")] public float gameTime;
    [Tooltip("Elapsed time in the game scene")] private float currentGameTime;
    private bool playersMovedToSpawnPos;


    void Start()
    {
        currentLobbyTime = lobbyTime;
        currentGameTime = gameTime;
        currentWinScreenDisplayTime = winScreenDisplayTime;
        playersMovedToSpawnPos = false;
        StartCoroutine(Countdown());
    }

    /// <summary>
    /// Initiates the Loadout timer and on completion disables loadout and starts game timer.
    /// <para>Also updates UI</para>
    /// </summary>
    [Server]
    private IEnumerator Countdown()
    {
        var floorTime = Mathf.FloorToInt(lobbyTime);
        //Ticking down loadout screen time and updates the clock UI
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
            //Once time is below 30 on lobby clock shift players to inside temple
            if(currentLobbyTime < 30 && playersMovedToSpawnPos == false)
            {
                playersMovedToSpawnPos = true;
                //FindObjectOfType<MyNetworkManager>().MovePlayersToSpawnPosInTemple();
            }
        }
        //When loadout screen time is completed, disable the loadout screen and start the game couroutine
        DisableLoadoutScreen();
        StartCoroutine(GameTimer());
    }

    /// <summary>
    /// Goes through game timer and ends game.
    /// <para>Also updates UI</para>
    /// </summary>
    /// <returns></returns>
    [Server]
    private IEnumerator GameTimer()
    {
        var floorTime = Mathf.FloorToInt(gameTime);

        //Ticks down game timer and updates the clock UI
        while (currentGameTime > 0)
        {
            yield return null;

            currentGameTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(currentGameTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                UpdateGameClock(floorTime);
            }
        }

        EndGame();
    }


    [ClientRpc]
    private void RpcDisconnectPlayer()
    {
        MyNetworkManager.singleton.StopClient();
    }

    /// <summary>
    /// Loads in abilites selected, and disables the laodout screen and disables player immobolisation.
    /// </summary>
    [ClientRpc]
    private void DisableLoadoutScreen()
    {
        FindObjectOfType<AbilitySlotBarUI>().LoadInAbilitiesFromLoadout(FindObjectOfType<LoadoutSelectionBoxUI>().GetLoadoutForAbilitySlotBar());
        FindObjectOfType<CanvasUIManager>().loadoutScreen.SetActive(false);
        //Try and disable slot here
        foreach (AbilityPickBarIconUI icon in FindObjectsOfType<AbilityPickBarIconUI>())
        {
            icon.gameObject.SetActive(false);
        }
        NetworkClient.localPlayer.GetComponent<PlayerController>().SetLoadoutReleased(true);
    }

    /// <summary>
    /// Simply updates the text inside loadoutTimeText for the loadout timer
    /// </summary>
    [ClientRpc]
    private void UpdateCountdown(int time)
    {
        //Todo: For consistancy, either call a function or set the text(referencing the UpdateGameClock)
        FindObjectOfType<CanvasUIManager>().loadoutTimeText.text = "" + time;
    }

    /// <summary>
    /// Simply updates the text inside timeText for the game timer
    /// </summary>
    [ClientRpc]
    private void UpdateGameClock(int time)
    {
        FindObjectOfType<CanvasUIManager>().SetTimeText(time);
    }

    /// <summary>
    /// Displays the scoreboard, doesn't end the game! Todo: Rename function?
    /// </summary>
    [ClientRpc]
    private void EndGame()
    {
        //Sets up info from Stash to use here
        Dictionary<string, int> scores = FindObjectOfType<Stash>().GetStashedScores();
        int countOfScoringPlayers = scores.Count;
        LeaderboardScore[] results = new LeaderboardScore[scores.Count];

        //Foreach player that scored,
        for (int i = 0; i < countOfScoringPlayers; i++)
        {
            //Finding the next highest scorer
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
            //Add the next highest scorer to the results and remove them from the next loop(scores.remove)
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
        //Add the results to the scoreboard.
        FindObjectOfType<CanvasUIManager>().winScreen.gameObject.SetActive(true);
        for (int i = 0; i < results.Length; i++)
        {
            Debug.Log("Adding to results: " + results[i].name + " with score of: " + results[i].score);
            FindObjectOfType<CanvasUIManager>().winScreen.AddToContent(results[i].name, results[i].score);
        }
        CmdStartWinScreenDisplayTimer();
    }

    /// <summary>
    /// Calls EndGame(), - currently called when stash score is reached.
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdCallGameToFinish()
    {
        EndGame();
    }
    /// <summary>
    /// Starts the coroutine that counts down how long we display the win screen for
    /// </summary>
    [Command(requiresAuthority = false)]
    private void CmdStartWinScreenDisplayTimer()
    {
        StartCoroutine(WinScreenDisplayTimer());
    }

    /// <summary>
    /// Runs an enumeration for the time of winScreenDisplayTime and then disconnects all players from the game
    /// </summary>
    /// <returns>IEnumerator</returns>
    [Server]
    private IEnumerator WinScreenDisplayTimer()
    {
        var floorTime = Mathf.FloorToInt(winScreenDisplayTime);

        while (currentWinScreenDisplayTime > 0)
        {
            yield return null;

            currentWinScreenDisplayTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(currentWinScreenDisplayTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;
            }
        }
        RpcDisconnectPlayer();
        MyNetworkManager.singleton.StopHost();
    }
}
