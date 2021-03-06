using System.Collections;
using UnityEngine;


public class PlayerTracker : Powerup
{

    private PlayerTrackIconUI playerTrackIcon;

    public PlayerTracker() : base("PlayerTracker", "Track other players on the map", 5, AbilityUseTypes.RECHARGE, 30.0f, 50.0f)
    {
        playerTrackIcon = GameObject.FindObjectOfType<PlayerTrackIconUI>();
    }

    public override void Use()
    {
        //Display player track icon
        playerTrackIcon.SetIconTarget(FindHighestPlayerTarget());
    }

    protected override void EndEffect()
    {
        //Get rid of player track icon
        playerTrackIcon.SetIconTarget(null);
        base.EndEffect();
    }

    private PlayerController FindHighestPlayerTarget()
    {
        throw new System.NotImplementedException();
    }

}
