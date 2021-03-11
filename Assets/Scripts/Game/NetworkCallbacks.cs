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
        FindObjectOfType<CanvasUIManager>().scoreboardUI.UpdateBoard(evnt.PlayerName, evnt.Score);
    }

    public override void OnEvent(InventoryRemove evnt)
    {
        BoltLog.Info("Called OnEvent InventoryRemove");
        if (evnt.InventoryEntity.IsOwner)
        {
            evnt.InventoryEntity.GetComponent<PlayerController>().RemoveFromInventory(evnt.ItemIndex, evnt.ItemName, evnt.ItemPoints);
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
                evnt.Target.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Speed = 4f;
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
            evnt.Trap.GetComponent<MeshRenderer>().enabled = false;
            evnt.Trap.GetComponent<SphereCollider>().enabled = false;
        }
        else
        {
            evnt.Trap.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public override void OnEvent(ObstacleDisable evnt)
    {
        BoltLog.Info("Called OnEvent ObstacleDisable");
        evnt.Obstacle.gameObject.GetComponent<MeshRenderer>().enabled = false;
        evnt.Obstacle.gameObject.GetComponent<BoxCollider>().enabled = false;
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
        int lastMinScore = int.MaxValue;
        while (scoreList.Count > 0)
        {
            StashedScore minScore = null;
            foreach (StashedScore score in scoreList)
            {
                if(score.Score <= lastMinScore)
                {
                    minScore = score;
                    lastMinScore = score.Score;
                }
            }
            for (int i = 0; i < results.Length; i++)
            {
                if(results == null)
                {
                    results[i] = minScore;
                }
            }
            scoreList.Remove(minScore);
        }
        FindObjectOfType<CanvasUIManager>().winScreen.gameObject.SetActive(true);
        for (int i = 0; i < results.Length; i++)
        {
            FindObjectOfType<CanvasUIManager>().winScreen.AddToContent(results[i].Name, results[i].Score);
        }
        
    }
}
