using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
public class GenerateAllGen : MonoBehaviour
{

    string seed;
    public GameObject[] mapGens;
    void Start()
    {
        if (BoltNetwork.IsServer)
        {
            StartCoroutine(Gens());
        }
    }

    IEnumerator Gens()
    {
            for (int i = 0; i < 5; i++)
            {
                RandSeed(i);
                mapGens[i].GetComponent<MapGenerator>().GenerateEverything(seed);
                yield return new WaitForSeconds(1);
            }
    }

    void RandSeed(int i)
    {
                seed = System.DateTime.Now.ToString();
                var request = SpawnObstacle.Create();
                request.seedString = seed;
                request.gen = i;
                request.Send();
    }
    
    public void GenerateCall(int i, string seedFromServer)
    {
        mapGens[i].GetComponent<MapGenerator>().GenerateEverything(seedFromServer);
    }
}
