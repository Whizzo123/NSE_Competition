using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class grassTempScript : EntityBehaviour<IObstacles>
{
    public override void Attached()
    {
        if (entity.IsOwner)
        {
            state.SetTransforms(state.Transform, transform);
        }
    }
}
