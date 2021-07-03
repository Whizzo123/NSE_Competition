using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using System.Linq;
using UnityEngine.SceneManagement;

[BoltGlobalBehaviour("GameScene")]
public class NetworkCallbacks : GlobalEventListener
{



    public override void OnEvent(InventoryRemove evnt)
    {
        BoltLog.Info("Called OnEvent InventoryRemove");
        if (evnt.InventoryEntity.IsOwner)
        {
           // evnt.InventoryEntity.GetComponent<PlayerController>().RemoveFromInventory(evnt.ItemIndex, evnt.ItemName, evnt.ItemPoints);
           // evnt.InventoryEntity.GetComponent<PlayerController>().state.HasBeenStolenFrom = true;
            var request = StunEnemyPlayer.Create();
            request.Target = evnt.InventoryEntity;
            request.End = false;
            request.Send();
        }
    }

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
        //NetworkArray_Objects<StashedScore> scores = FindObjectOfType<Stash>().entity.GetState<IStashState>().StashedScores;
        //StashedScore[] results = new StashedScore[scores.Length];
        //for (int i = 0; i < results.Length; i++)
        //{
          //  results[i] = null;
        //}
        //List<StashedScore> scoreList = scores.ToList<StashedScore>();
        /*for(int i = 0; i < scoreList.Count; i++)
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
        }*/
        
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
