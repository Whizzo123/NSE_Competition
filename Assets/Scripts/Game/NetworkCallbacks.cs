using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using System.Linq;

[BoltGlobalBehaviour("GameScene")]
public class NetworkCallbacks : GlobalEventListener
{

    public override void OnEvent(ArtefactDisable evnt)
    {
        BoltLog.Info("Called OnEvent ArtefactDisable");
        evnt.artefactToDisable.gameObject.GetComponent<SphereCollider>().enabled = false;
        evnt.artefactToDisable.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public override void OnEvent(ArtefactEnable evnt)
    {
        evnt.artefact.gameObject.GetComponent<SphereCollider>().enabled = true;
        evnt.artefact.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
    public override void OnEvent(AbilityPickupDisable evnt)
    {
        BoltLog.Info("Called OnEvent AbilityPickupDisable");
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
            evnt.InventoryEntity.GetComponent<PlayerController>().state.HasBeenStolenFrom = true;
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
    public override void OnEvent(StunEnemyPlayer evnt)
    {
        BoltLog.Info("Called OnEvent StunEnemyPlayer");
        if (evnt.Target.IsOwner)
        {
            if (!evnt.End)
            {
                SpeedBoost spd = (SpeedBoost)evnt.Target.GetComponent<PlayerController>().abilityInventory.FindAbility("Speed");
                if (spd != null)
                    spd.SetOppositeDebuffActivated(true);
                evnt.Target.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Speed = 1f;
            }
            else
            {
                SpeedBoost spd = (SpeedBoost)evnt.Target.GetComponent<PlayerController>().abilityInventory.FindAbility("Speed");
                if (spd != null)
                    spd.SetOppositeDebuffActivated(false);
                evnt.Target.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Speed = FindObjectOfType<PlayerController>().speed;
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
            if(!evnt.End)
            {
                evnt.Victim.GetComponent<PlayerController>().immobilize = true;
            }
            else
            {
                evnt.Victim.GetComponent<PlayerController>().immobilize = false;
            }
        }
        if(evnt.End)
        {
            evnt.Trap.GetComponent<BearTrapBehaviour>().Disable();
        }
        else
        {
            evnt.Trap.GetComponent<BearTrapBehaviour>().Close();
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
                item.transform.gameObject.GetComponent<SphereCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                /*ab = item.transform.gameObject.GetComponent<ArtefactBehaviour>();
                var req = ArtefactEnable.Create();
                req.artefact = ab.entity;
                req.Send();*/
                //item.transform.gameObject.GetComponentInChildren<ArtefactBehaviour>().transform.SetParent(null);
            }
            else if (item.transform.GetComponent<AbilityPickup>())
            {
                item.transform.gameObject.GetComponent<SphereCollider>().enabled = true;
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
}
