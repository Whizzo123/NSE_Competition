using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using System.Linq;
using UnityEngine.SceneManagement;

[BoltGlobalBehaviour("GameScene")]
public class NetworkCallbacks : GlobalEventListener
{
    #region Artefact
    public override void OnEvent(ArtefactDisable evnt)
    {
        BoltLog.Info("Called OnEvent ArtefactDisable");
        evnt.artefactToDisable.gameObject.GetComponent<BoxCollider>().enabled = false;
        evnt.artefactToDisable.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public override void OnEvent(ArtefactEnable evnt)
    {
        evnt.artefact.gameObject.GetComponent<BoxCollider>().enabled = true;
        evnt.artefact.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    #endregion

    #region AbilityPickup/Score/Inventory/Loadout
    public override void OnEvent(AbilityPickupDisable evnt)
    {
        evnt.AbilityEntity.gameObject.GetComponent<MeshRenderer>().enabled = false;
        evnt.AbilityEntity.gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    public override void OnEvent(ScoreUpdate evnt)
    {
        BoltLog.Info("Called OnEvent ScoreUpdate");
        if(evnt.StashEntity.IsOwner)
        {
            evnt.StashEntity.GetComponent<Stash>().UpdateState(evnt.PlayerName, evnt.Score);
        }
        FindObjectOfType<CanvasUIManager>().scoreboardUI.UpdateBoard(evnt.PlayerName);
    }

    public override void OnEvent(InventoryRemove evnt)
    {
        BoltLog.Info("Called OnEvent InventoryRemove");
        if (evnt.InventoryEntity.IsOwner)
        {
            evnt.InventoryEntity.GetComponent<PlayerController>().RemoveFromInventory(evnt.ItemIndex, evnt.ItemName, evnt.ItemPoints);
            //evnt.InventoryEntity.GetComponent<PlayerController>().state.HasBeenStolenFrom = true;
            var request = StunEnemyPlayer.Create();
            request.Target = evnt.InventoryEntity;
            request.End = false;
            request.Send();
        }
    }

    public override void OnEvent(LoadoutScreenDisable evnt)
    {
        BoltLog.Info("Called OnEvent LoadoutScreenDisable");
        FindObjectOfType<AbilitySlotBarUI>().LoadInAbilitiesFromLoadout(FindObjectOfType<LoadoutBarUI>().GetLoadoutForAbilitySlotBar());
        FindObjectOfType<CanvasUIManager>().loadoutScreen.SetActive(false);
        PlayerController.localPlayer.SetLoadoutReleased(true);
    }

    #endregion

    public override void OnEvent(AbilityPickupSpawn evnt)
    {
        if (BoltNetwork.IsServer)
        {
            BoltEntity entityOne = BoltNetwork.Instantiate(BoltPrefabs.AbilityPickup, new Vector3(evnt.SpawnLocationOne.x, -2f, evnt.SpawnLocationOne.y), Quaternion.identity);
            entityOne.GetComponent<AbilityPickup>().SetAbilityOnPickup(evnt.AbilityOneName);
            BoltEntity entityTwo = BoltNetwork.Instantiate(BoltPrefabs.AbilityPickup, new Vector3(evnt.SpawnLocationTwo.x, -2f, evnt.SpawnLocationTwo.y), Quaternion.identity);
            entityTwo.GetComponent<AbilityPickup>().SetAbilityOnPickup(evnt.AbilityTwoName);
            BoltEntity entityThree = BoltNetwork.Instantiate(BoltPrefabs.AbilityPickup, new Vector3(evnt.SpawnLocationThree.x, -2f, evnt.SpawnLocationThree.y), Quaternion.identity);
            entityThree.GetComponent<AbilityPickup>().SetAbilityOnPickup(evnt.AbilityThreeName);
        }
    }

    public override void OnEvent(StunEnemyPlayer evnt)
    {
        BoltLog.Info("Called OnEvent StunEnemyPlayer");
        if (evnt.Target.IsOwner)
        {
            if (!evnt.End)
            {
                Debug.Log("Slowing");
                SpeedBoost spd = (SpeedBoost)evnt.Target.GetComponent<PlayerController>().abilityInventory.FindAbility("Speed");
                if (spd != null)
                    spd.SetOppositeDebuffActivated(true);
                //evnt.Target.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Speed = 1f;
                //Instantiate(Resources.Load("SlowBombExplosion_PA", typeof(GameObject)) as GameObject, evnt.Target.transform.position, Quaternion.identity);//Instantiates it on all other machines besides the thrower
            }
            else
            {
                Debug.Log("quickening");
                SpeedBoost spd = (SpeedBoost)evnt.Target.GetComponent<PlayerController>().abilityInventory.FindAbility("Speed");
                if (spd != null)
                    spd.SetOppositeDebuffActivated(false);
                //evnt.Target.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Speed = FindObjectOfType<PlayerController>().speed;
            }
        }
    }

    public override void OnEvent(ParalyzePlayerEvent evnt)
    {
        if(evnt.Target.IsOwner)
        {
            if(!evnt.End)
            {
                evnt.Target.GetState<IGamePlayerState>().Paralyzed = true;
            }
            else
            {
                evnt.Target.GetState<IGamePlayerState>().Paralyzed = false;
            }
        }
    }

    public override void OnEvent(MortalSpellEvent evnt)
    {
        if(evnt.Target.IsOwner)
        {
            if(!evnt.End)
            {
                evnt.Target.GetState<IGamePlayerState>().Mortal = true;
            }
            else
            {
                evnt.Target.GetState<IGamePlayerState>().Mortal = false;
            }
        }
    }
    public override void OnEvent(SpringBearTrap evnt)
    {

        if(evnt.Victim.IsOwner)
        {
            Debug.LogError("envt Victim is owner START");
            if (!evnt.End)
            {
                Debug.LogError("evo immobolise");
                evnt.Victim.GetComponent<PlayerController>().immobilize = true;
            }
            else
            {
                Debug.LogError("evo !immobolise");
                evnt.Victim.GetComponent<PlayerController>().immobilize = false;
            }
        }
        Debug.LogError("evo end");
        if (evnt.End)
        {
            Debug.LogError("evnt end");
            evnt.Trap.GetComponent<BearTrapBehaviour>().Disable();
            evnt.Trap.GetComponent<SphereCollider>().enabled = false;
        }
        else
        {
            Debug.LogError("Close");
            evnt.Trap.GetComponent<BearTrapBehaviour>().Close();
        }
    }

    public override void OnEvent(PoisonPlayer evnt)
    {
        if(evnt.Target.IsOwner)
        {
            Debug.LogError("Voodoo START");
            if (!evnt.End)
            {
                Debug.LogError("Voodoo POISON");
                //evnt.Target.GetComponent<PlayerController>().state.Poisoned = true;
            }
            else
            {
                Debug.LogError("Voodoo NOTPOISON");
                //evnt.Target.GetComponent<PlayerController>().state.Poisoned = false;
            }
        }
        Debug.LogError("Voodoo EVNT END");
        if (evnt.End)
        {
            evnt.Trap.GetComponent<VoodooPoisonTrapBehaviour>().Disable();
            //evnt.Trap.GetComponent<SphereCollider>().enabled = false;
        }
        else
        {
            evnt.Trap.GetComponent<VoodooPoisonTrapBehaviour>().Close();
        }
    }

    public override void OnEvent(ToggleCamouflage evnt)
    {        
        if(!evnt.Target.IsOwner)
            evnt.Target.GetComponent<PlayerController>().ToggleMesh(evnt.Toggle);
    }

    public override void OnEvent(ObstacleDisable evnt)
    {
        /*BoltLog.Info("Called OnEvent ObstacleDisable");
        evnt.Obstacle.gameObject.GetComponent<MeshRenderer>().enabled = false;
        evnt.Obstacle.gameObject.GetComponent<BoxCollider>().enabled = false;
        evnt.Obstacle.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        evnt.Obstacle.gameObject.GetComponent<ArtefactBehaviour>().EnableForPickup();*/

        Ray ray = new Ray(evnt.position, evnt.forward);
        RaycastHit[] hit;
        PlayerController ob = FindObjectOfType<PlayerController>();
        hit = Physics.SphereCastAll(ray,ob.radiusOfSphere, ob.lengthOfSphere, ob.obstacles);
        foreach (RaycastHit item in hit)
        {
            if (item.transform.GetComponent<ArtefactBehaviour>())
            {
                item.transform.gameObject.GetComponent<ArtefactBehaviour>().EnableForPickup();
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                /*ab = item.transform.gameObject.GetComponent<ArtefactBehaviour>();
                var req = ArtefactEnable.Create();
                req.artefact = ab.entity;
                req.Send();*/
                //item.transform.gameObject.GetComponentInChildren<ArtefactBehaviour>().transform.SetParent(null);
            }
            else if (item.transform.GetComponent<AbilityPickup>())
            {
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                item.transform.GetComponent<AbilityPickup>().enabledForPickup = true;
            }
            else
            {
                Destroy(item.transform.gameObject);
            }

        }
    }

    public override void OnEvent(DisplayWinScreen evnt)
    {
        BoltLog.Info("Calling DisplayWinScreen event");
        NetworkArray_Objects<StashedScore> scores = FindObjectOfType<Stash>().entity.GetState<IStashState>().StashedScores;
        StashedScore[] results = new StashedScore[scores.Length];
        for (int i = 0; i < results.Length; i++)
        {
            results[i] = null;
        }
        List<StashedScore> scoreList = scores.ToList<StashedScore>();
        for(int i = 0; i < scoreList.Count; i++)
        {
            int lastMaxScore = int.MinValue;
            StashedScore maxScore = null;
            foreach (StashedScore score in scoreList)
            {
                if (score.Name == string.Empty) continue;
                Debug.Log("Testing score: " + score.Name + " with a score of " + score.Score);
                Debug.Log("Against lastMaxScore of: " + lastMaxScore);
                if(score.Score > lastMaxScore)
                {
                    Debug.Log("Setting as min score: " + score.Score + " : " + score.Name);
                    maxScore = score;
                    lastMaxScore = score.Score;
                }
            }
            if (maxScore != null)
            {
                for (int j = 0; j < results.Length; j++)
                {
                    if (results[j] == null)
                    {
                        Debug.Log("Adding into results: " + maxScore.Name);
                        results[j] = maxScore;
                        break;
                    }
                    else
                    {
                        Debug.Log("Getting results[" + j + "]: " + results[j].Name);
                    }
                }
                scoreList.Remove(maxScore);
            }
        }
        FindObjectOfType<CanvasUIManager>().winScreen.gameObject.SetActive(true);
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] != null)
            {
                Debug.Log("Adding to results: " + results[i].Name + " with score of: " + results[i].Score);
                FindObjectOfType<CanvasUIManager>().winScreen.AddToContent(results[i].Name, results[i].Score);
            }
        }
        
    }

    public override void OnEvent(GameCountdown evnt)
    {
        FindObjectOfType<CanvasUIManager>().SetTimeText(evnt.Time);
    }

    public override void OnEvent(ChangeAnimatorMovementParameter evnt)
    {
        evnt.Target.transform.GetChild(0).GetComponent<Animator>().SetBool("moving", evnt.Value);
    }

    public override void OnEvent(FireAnimatorCutTriggerParameter evnt)
    {
        AnimatorStateInfo state = evnt.Target.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (!state.IsName("StandCut") || !state.IsName("RunCut"))
            evnt.Target.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Cut");
    }

    public override void OnEvent(FireAnimatorThrowTrigger evnt)
    {
        AnimatorStateInfo state = evnt.Target.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if(!state.IsName("Throw"))
        {
            evnt.Target.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Throw");
        }
    }

    public override void OnEvent(FireAnimatorPutDownTrigger evnt)
    {
        AnimatorStateInfo state = evnt.Target.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (!state.IsName("PutDown"))
        {
            evnt.Target.transform.GetChild(0).GetComponent<Animator>().SetTrigger("PutDown");
        }
    }

    public override void OnEvent(SpawnObstacle evnt)
    {
        FindObjectOfType<GenerateAllGen>().GenerateCall(evnt.gen, evnt.seedString);


        /*MapGenerator[] gens = FindObjectsOfType<MapGenerator>();
        foreach (MapGenerator item in gens)
        {
            item.seed = evnt.seedString;
        }*/
    }

    public override void OnEvent(LoadoutCountdown evnt)
    {
        FindObjectOfType<CanvasUIManager>().loadoutTimeText.text = "" + evnt.Time;
    }

    public override void OnEvent(ReturnEveryoneToTitle evnt)
    {
        BoltNetwork.Shutdown();
        //SceneLoadLocalBegin("TitleScene");
        SceneManager.LoadScene("TitleScene");
    }

}
