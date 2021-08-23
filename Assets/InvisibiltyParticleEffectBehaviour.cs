using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InvisibiltyParticleEffectBehaviour : NetworkBehaviour
{

    public GameObject particlefx;
    // Start is called before the first frame update
    void Start()
    {
        //particlefx.SetActive(true);
        Invoke("PlayFX", 3);
        Destroy(this.gameObject, 4);

    }

    [ClientCallback]
    public void PlayFX()
    {
        Debug.Log("BIHTT");
        particlefx.SetActive(true);
        particlefx.GetComponentInChildren<ParticleSystem>().Play();
    }
}
