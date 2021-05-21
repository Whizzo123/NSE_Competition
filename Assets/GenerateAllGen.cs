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
                yield return new WaitForSeconds(2);
                RandSeed(i);
                //mapGens[i].GetComponent<MapGenerator>().GenerateEverything(seed);

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
