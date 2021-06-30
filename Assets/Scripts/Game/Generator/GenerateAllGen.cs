using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GenerateAllGen : NetworkBehaviour
{

    string seed;
    public GameObject[] mapGens;

    void Start()
    {
        StartCoroutine(Gens());
    }

    IEnumerator Gens()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(5);
            RandSeed(i);
            //mapGens[i].GetComponent<MapGenerator>().GenerateEverything(seed);
        }
    }

    void RandSeed(int i)
    {
        seed = System.DateTime.Now.ToString();
        GenerateCall(i, seed);
        /*MapGenerator[] gens = FindObjectsOfType<MapGenerator>();
       foreach (MapGenerator item in gens)
       {
           item.seed = evnt.seedString;
       }*/
    }

    public void GenerateCall(int i, string seedFromServer)
    {
        mapGens[i].GetComponent<MapGenerator>().GenerateEverything(seedFromServer);
    }
}
