using System.Collections;
using UnityEngine;


public class VisionCloudTrap : Trap
{

    public VisionCloudTrap() :base ("Vision Cloud Trap", "Hinders your opponents by shrinking their visual field", 3, AbilityUseTypes.ONE_TIME)
    {

    }

    public override Ability Clone()
    {
        return new VisionCloudTrap();
    }

}
