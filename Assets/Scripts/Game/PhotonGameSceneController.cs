using System.Collections;
using UnityEngine;
using Bolt;

[BoltGlobalBehaviour("GameScene")]
public class PhotonGameSceneController : GlobalEventListener
{

    private int minPlayers = 2;
    private bool loadoutChoiceComplete;
    //Max time for game to 8 mins - 480
    private float totalAllottedGameTime = 45;
    private float currentRunningGameTime = 0;
    private bool displayedWinScreen = false;

    public override void SceneLoadLocalDone(string scene)
    {
        PlayerController.Spawn();
    }

    void Start()
    {
        loadoutChoiceComplete = false;
        if (BoltNetwork.IsServer)
        {
            BoltEntity artefact = BoltNetwork.Instantiate(BoltPrefabs.Artefect);
            artefact.transform.position = new Vector3(1, -1, -16);
            artefact.GetComponent<ArtefactBehaviour>().PopulateData("Mayan Jar", ArtefactRarity.Common);
            artefact.TakeControl();

            BoltEntity secondArtefact = BoltNetwork.Instantiate(BoltPrefabs.Artefect);
            secondArtefact.transform.position = new Vector3(-15, -1, 4);
            secondArtefact.GetComponent<ArtefactBehaviour>().PopulateData("Empty Tube Of Toothpaste", ArtefactRarity.Exotic);
            secondArtefact.TakeControl();

            BoltEntity stash = BoltNetwork.Instantiate(BoltPrefabs.Stash);
            stash.transform.position = new Vector3(-19 , -1.5f, -20);
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
            if(currentRunningGameTime < totalAllottedGameTime)
            {
                currentRunningGameTime += BoltNetwork.FrameDeltaTime;
                BoltLog.Info("Running time is: " + currentRunningGameTime);
            }
            else
            {
                //End game
                var request = DisplayWinScreen.Create();
                request.Send();
                displayedWinScreen = true;
            }

            NetworkArray_Objects<StashedScore> scores = FindObjectOfType<Stash>().entity.GetState<IStashState>().StashedScores;
            foreach (StashedScore score in scores)
            {
                if(score.Score > 10000)
                {
                    //End game
                    var request = DisplayWinScreen.Create();
                    request.Send();
                    displayedWinScreen = true;
                }
            }
        }
    }
 

}
