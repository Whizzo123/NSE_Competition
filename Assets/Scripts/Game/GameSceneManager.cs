using System.Collections;
using UnityEngine;
using Mirror;

/// <summary>
/// Controls the game ending
/// </summary>
public class GameSceneManager : NetworkBehaviour
{
    [SyncVar][Tooltip("Has the game ended via MyLobbyCountdown or Stash")] private bool endedGame = false;



    void Start()
    {
        
    }

    void LateUpdate()
    {
        //JoeReply -> The reason it is here rather than in the countdown is because it isn't really time related and only really time related stuff exists in MyLobbyCountdown.cs
        if(FindObjectOfType<Stash>().HasPlayerReachedWinningPointsThreshold() && !endedGame)
        {
            endedGame = true;
            FindObjectOfType<TheCountdown>().CmdCallGameToFinish();
        }
    }
}
