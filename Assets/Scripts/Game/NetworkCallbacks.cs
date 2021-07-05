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

    public override void OnEvent(ReturnEveryoneToTitle evnt)
    {
        BoltNetwork.Shutdown();
        //SceneLoadLocalBegin("TitleScene");
        SceneManager.LoadScene("TitleScene");
    }

}
