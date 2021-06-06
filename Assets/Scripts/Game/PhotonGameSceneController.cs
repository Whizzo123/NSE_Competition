using System.Collections;
using UnityEngine;
using Bolt;

[BoltGlobalBehaviour("GameScene")]
public class PhotonGameSceneController : GlobalEventListener
{

    private int minPlayers = 2;
    private bool loadoutChoiceComplete;
    //Max time for game to 8 mins - 480
    private float totalAllottedGameTime = 300;
    private float currentRunningGameTime = 0;
    private float readyTime = 30;
    private float currentReadyTime = 0;
    private int pointGoal = 9999; //10000
    private bool displayedWinScreen = false;
    private bool inCountdown = false;
    private float winWaitTime = 10;
    private float currentWinWaitTime = 0;

    public override void SceneLoadLocalDone(string scene)
    {
        FindObjectOfType<AudioManager>().ActivateGameMusic();
        PlayerController.Spawn();
        //FindObjectOfType<PlayerTestSuite>().InitializeTest();
        if (BoltNetwork.IsServer)
        {
            currentReadyTime = readyTime;
            StartCoroutine(RunLobbyReadyCountdown());
        }
    }

    void Start()
    {
        loadoutChoiceComplete = false;
        displayedWinScreen = false;
        inCountdown = false;
        if (BoltNetwork.IsServer)
        {
            BoltEntity stash = BoltNetwork.Instantiate(BoltPrefabs.Stash);
            stash.transform.position = new Vector3(4.24f , 0.57f, -18.93f);
            stash.TakeControl();
        }
    }

    void FixedUpdate()
    {
        if (BoltNetwork.IsServer && loadoutChoiceComplete == false)
        {
            var allReady = true;
            var readyCount = 0;

            foreach (var entity in BoltNetwork.Entities)
            {
                if (entity.StateIs<IGamePlayerState>() == false) continue;

                var playerController = entity.GetState<IGamePlayerState>();
                allReady &= playerController.LoadoutReady;

                if (allReady == false) break;
                readyCount++;
            }

            if (allReady && readyCount >= minPlayers)
            {
                //Disable loadout screens
                var request = LoadoutScreenDisable.Create();
                request.Send();
                loadoutChoiceComplete = true;
            }
        }
        if(BoltNetwork.IsServer && !displayedWinScreen && loadoutChoiceComplete)
        {
            if (inCountdown == false)
            {
                inCountdown = true;
                currentRunningGameTime = totalAllottedGameTime;
                StartCoroutine(RunGameCountdown());
            }
            NetworkArray_Objects<StashedScore> scores = FindObjectOfType<Stash>().entity.GetState<IStashState>().StashedScores;
            foreach (StashedScore score in scores)
            {
                if (score.Score > pointGoal)
                {
                    //End game
                    var request = DisplayWinScreen.Create();
                    request.Send();
                    displayedWinScreen = true;

                    //////////////////////////////////////////////Add in return to title
                }
            }
        }

        if(BoltNetwork.IsServer && displayedWinScreen)
        {
            currentWinWaitTime = winWaitTime;
            StartCoroutine(RunWinCountdown());
        }
    }

    private IEnumerator RunLobbyReadyCountdown()
    {
        var floorTime = Mathf.FloorToInt(readyTime);

        LoadoutCountdown countdown;
        while(currentReadyTime > 0)
        {
            yield return null;

            currentReadyTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(currentReadyTime);

            if(newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                countdown = LoadoutCountdown.Create();
                countdown.Time = floorTime;
                countdown.Send();
            }
        }

        LoadoutScreenDisable request = LoadoutScreenDisable.Create();
        request.Send();
        loadoutChoiceComplete = true;
    }

    private IEnumerator RunGameCountdown()
    {
        var floorTime = Mathf.FloorToInt(totalAllottedGameTime);
        GameCountdown countdown;
        while (currentRunningGameTime > 0)
        {
            yield return null;

            currentRunningGameTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(currentRunningGameTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;
                //Create lobbycountdown event to update everyone's time
                countdown = GameCountdown.Create();
                countdown.Time = floorTime;
                countdown.Send();
            }
        }
        countdown = GameCountdown.Create();
        countdown.Time = 0;
        countdown.Send();
        //End game
        var request = DisplayWinScreen.Create();
        request.Send();
        displayedWinScreen = true;


        currentRunningGameTime = 5;

        while (currentRunningGameTime > 0)
        {
            yield return null;

            currentRunningGameTime -= Time.deltaTime;
        }

        var end = ReturnEveryoneToTitle.Create();
        end.Send();

    }

    private IEnumerator RunWinCountdown()
    {
        var floorTime = Mathf.FloorToInt(winWaitTime);

        while (currentWinWaitTime > 0)
        {
            yield return null;

            currentWinWaitTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(currentWinWaitTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;
                //Create lobbycountdown event to update everyone's time
            }
        }
        BoltNetwork.LoadScene("GameScene");
    }
}
