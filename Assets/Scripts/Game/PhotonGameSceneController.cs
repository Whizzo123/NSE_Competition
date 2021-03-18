using System.Collections;
using UnityEngine;
using Bolt;

[BoltGlobalBehaviour("GameScene")]
public class PhotonGameSceneController : GlobalEventListener
{

    private int minPlayers = 2;
    private bool loadoutChoiceComplete;
    //Max time for game to 8 mins - 480
    private float totalAllottedGameTime = 480;
    private float currentRunningGameTime = 0;
    private int pointGoal = 5100; //10000
    private bool displayedWinScreen = false;
    private bool inCountdown = false;

    public override void SceneLoadLocalDone(string scene)
    {
        PlayerController.Spawn();
        FindObjectOfType<PlayerTestSuite>().InitializeTest();
    }

    void Start()
    {
        loadoutChoiceComplete = false;
        if (BoltNetwork.IsServer)
        {
            BoltEntity artefact = BoltNetwork.Instantiate(BoltPrefabs.Artefect);
            artefact.transform.position = new Vector3(1, -3, -16);
            artefact.GetComponent<ArtefactBehaviour>().PopulateData("Mayan Jar", ArtefactRarity.Common);
            artefact.TakeControl();

            BoltEntity secondArtefact = BoltNetwork.Instantiate(BoltPrefabs.Artefect);
            secondArtefact.transform.position = new Vector3(-15, -3, 4);
            secondArtefact.GetComponent<ArtefactBehaviour>().PopulateData("Empty Tube Of Toothpaste", ArtefactRarity.Exotic);
            secondArtefact.TakeControl();

            BoltEntity thirdArtefact = BoltNetwork.Instantiate(BoltPrefabs.Artefect);
            thirdArtefact.transform.position = new Vector3(10, -3, -1);
            thirdArtefact.GetComponent<ArtefactBehaviour>().PopulateData("Tooth Pick", ArtefactRarity.Common);
            thirdArtefact.TakeControl();

            BoltEntity stash = BoltNetwork.Instantiate(BoltPrefabs.Stash);
            stash.transform.position = new Vector3(4.24f , 0.57f, -18.93f);
            stash.TakeControl();

            BoltEntity abilityPickup = BoltNetwork.Instantiate(BoltPrefabs.AbilityPickup);
            abilityPickup.transform.position = new Vector3(5, -1, 18);
            abilityPickup.GetComponent<AbilityPickup>().SetAbilityOnPickup("Speed");
            abilityPickup.TakeControl();
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
                }
            }
        }
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
    }
}
