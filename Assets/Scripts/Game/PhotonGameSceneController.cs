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
    private bool abilityPickupsSpawned = false;
    private float abilitySpawnTime = 30;
    private float currentAbilitySpawnTime = 0;
    private int counter = 0;

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

                    StartCoroutine(WaitThenReturnToTitle(5.0f));
                }
            }
        }

        if(BoltNetwork.IsServer && displayedWinScreen)
        {
            currentWinWaitTime = winWaitTime;
            StartCoroutine(RunWinCountdown());
        }

        if (BoltNetwork.IsServer && !abilityPickupsSpawned)
        {
            Debug.LogWarning("Calling method");
            currentAbilitySpawnTime = abilitySpawnTime;
            StartCoroutine(RunAbilityPickupSpawnCountdown());
        }
        else if (abilityPickupsSpawned)
            StopCoroutine(RunAbilityPickupSpawnCountdown());
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

    private IEnumerator RunAbilityPickupSpawnCountdown()
    {
        yield return new WaitForSeconds(30);
        //Do event stuff y -> -3.5
        if (abilityPickupsSpawned)
            yield return null;
        else
        {
            SpawnAbilityCharges();
        }
    }

    private void SpawnAbilityCharges()
    {
        string[] traps = FindObjectOfType<AbilityRegister>().GetTrapList();
        AbilityPickupSpawn request = AbilityPickupSpawn.Create();
        request.SpawnLocationOne = FindRandomPointOnCircle(new Vector2(4, -20), 11, Random.Range(0, 360));
        request.AbilityOneName = traps[Random.Range(0, traps.Length - 1)];
        request.SpawnLocationTwo = FindRandomPointOnCircle(new Vector2(4, -20), 11, Random.Range(0, 360));
        request.AbilityTwoName = traps[Random.Range(0, traps.Length - 1)];
        request.SpawnLocationThree = FindRandomPointOnCircle(new Vector2(4, -20), 11, Random.Range(0, 360));
        request.AbilityThreeName = traps[Random.Range(0, traps.Length - 1)];
        request.Send();
        abilityPickupsSpawned = true;
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

        StartCoroutine(WaitThenReturnToTitle(5.0f));
    }

    private IEnumerator WaitThenReturnToTitle(float waitTime)
    {
        float time = waitTime;

        while (time > 0)
        {
            yield return null;

            time -= Time.deltaTime;
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

    private Vector2 FindRandomPointOnCircle(Vector2 centerCirclePoint, float circleRadius, int angle)
    {
        float x = circleRadius * Mathf.Cos(angle) + centerCirclePoint.x;
        float y = circleRadius * Mathf.Sin(angle) + centerCirclePoint.y;

        return new Vector2(x, y);
    }
}
