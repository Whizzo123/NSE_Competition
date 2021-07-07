using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StickyBombBehaviour : NetworkBehaviour
{
    public float effectDuration;
    public bool tick;

    // Update is called once per frame
    void Update()
    {
        if (tick)
        {
            effectDuration -= Time.deltaTime;
            if (effectDuration < 0)
                Destroy(this.gameObject);
        }
    }
}
