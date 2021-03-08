using System.Collections;
using UnityEngine;
using Bolt;

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
        if(evnt.Target.IsOwner)
        {
            if (!evnt.End)
                evnt.Target.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Speed = 1f;
            else
                evnt.Target.GetComponent<PlayerController>().entity.GetState<IGamePlayerState>().Speed = 4f;
        }
    }
}
