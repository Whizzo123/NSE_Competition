using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

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
        GameObject.FindObjectOfType<CanvasUIManager>().playerTrackIcon.SetIconTarget(null);
        base.EndEffect();
    }

    private PlayerController FindHighestPlayerTarget()
    {
        int highestInventoryStash = 0;
        PlayerController playerWithHighestStashOnPerson = null;
        foreach (PlayerController player in GameObject.FindObjectsOfType<PlayerController>())
        {
            if (player == NetworkClient.localPlayer.gameObject.GetComponent<PlayerController>()) continue;
            List<ItemArtefact> inventory = player.GetComponent<ArtefactInventory>().GetInventory();
            int playerInventoryStash = 0;
            foreach (ItemArtefact item in inventory)
            {
                playerInventoryStash += item.points;
            }
            if(playerInventoryStash > highestInventoryStash)
            {
                highestInventoryStash = playerInventoryStash;
                playerWithHighestStashOnPerson = player;
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
