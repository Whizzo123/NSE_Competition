using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bolt;

public class PlayerTracker : Powerup
{



    public PlayerTracker() : base("PlayerTracker", "Track other players on the map", 5, AbilityUseTypes.RECHARGE, 30.0f, 25.0f)
    {

    }

    public override void Use()
    {
        //Display player track icon
        GameObject.FindObjectOfType<CanvasUIManager>().playerTrackIcon.SetIconTarget(FindHighestPlayerTarget());
        inUse = true;
    }

    protected override void EndEffect()
    {
        //Get rid of player track icon
        Debug.Log("Ending effect");
        GameObject.FindObjectOfType<CanvasUIManager>().playerTrackIcon.SetIconTarget(null);
        base.EndEffect();
    }

    private PlayerController FindHighestPlayerTarget()
    {
        int highestInventoryStash = 0;
        BoltEntity playerWithHighestStashOnPerson = null;
        foreach (BoltEntity boltEntity in BoltNetwork.Entities)
        {
            //if (boltEntity.StateIs<IGamePlayerState>() == false || boltEntity == GetPlayerToEmpower().entity) continue;
            List<InventoryItem> inventory = boltEntity.GetState<IGamePlayerState>().Inventory.ToList<InventoryItem>();
            int playerInventoryStash = 0;
            foreach (InventoryItem item in inventory)
            {
                playerInventoryStash += item.ItemPoints;
            }
            if(playerInventoryStash > highestInventoryStash)
            {
                highestInventoryStash = playerInventoryStash;
                playerWithHighestStashOnPerson = boltEntity;
            }
        }
        if (playerWithHighestStashOnPerson == null)
        {
            Debug.LogError("COULDN'T FIND A HIGHER STASH THAN 0 ON ANOTHER PLAYER IN THE GAME");
            return null;
        }
        else
        {
            return playerWithHighestStashOnPerson.GetComponent<PlayerController>();
        }
    }

    public override Ability Clone()
    {
        return new PlayerTracker();
    }

}
