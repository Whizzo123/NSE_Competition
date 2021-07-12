using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GenerateAllGen : NetworkBehaviour
{

    //string seed;
    public GameObject[] mapGens;


    void Start()
    {
        //Debug.LogWarning("Auth" + hasAuthority);
        for (int i = 0; i < 5; i++)
        {
            CmdChangeSeed(i);
        }
        StartCoroutine(Gens());
        
    }

    IEnumerator Gens()
    {
        //Debug.LogWarning("Coroutine running");
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(3);
            GenerateCall(i);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdChangeSeed(int i)
    {
        string seed = System.DateTime.Now.Millisecond.ToString();
        //Debug.LogWarning(System.DateTime.Now.Millisecond.ToString());
        mapGens[i].GetComponent<MapGenerator>().seed = seed;
    }

    //void RandSeed(int i)
    //{
    //    seed = System.DateTime.Now.ToString();
    //    GenerateCall(i, seed);
    //    MapGenerator[] gens = FindObjectsOfType<MapGenerator>();
    //   //foreach (MapGenerator item in gens)
    //   //{
    //   //    item.seed = evnt.seedString;
    //   //}
    //}
    public void GenerateCall(int i)
    {
        mapGens[i].GetComponent<MapGenerator>().GenerateEverything();
    }
}
