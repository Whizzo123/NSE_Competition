using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
public class GenerateAllGen : MonoBehaviour
{
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
        mapGens[0].GetComponent<MapGenerator>().GenerateEverything();
        yield return new WaitForSeconds(5);
        mapGens[1].GetComponent<MapGenerator>().GenerateEverything();
        yield return new WaitForSeconds(5);
        mapGens[2].GetComponent<MapGenerator>().GenerateEverything();
        yield return new WaitForSeconds(5);
        mapGens[3].GetComponent<MapGenerator>().GenerateEverything();
        yield return new WaitForSeconds(5);
        mapGens[4].GetComponent<MapGenerator>().GenerateEverything();
    }
}
