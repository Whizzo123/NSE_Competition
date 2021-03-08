using System.Collections;
using UnityEngine;


public class ObstacleTrap : Trap
{

    public ObstacleTrap() :base ("Obstacle Trap", "Spawns obstacles to delay enemy players", 3, AbilityUseTypes.ONE_TIME)
    {

    }

    public override Ability Clone()
    {
        return new ObstacleTrap();
    }
}
