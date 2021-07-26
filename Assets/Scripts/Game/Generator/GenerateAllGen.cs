using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Generates a random seed for each map generator and starts the generation process for each of them.
/// </summary>
public class GenerateAllGen : NetworkBehaviour
{
    [Tooltip("All map generators inside the map")]public GameObject[] mapGens;

    //Todo: Get rid of magic numbers
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            CmdChangeSeed(i);
        }
        StartCoroutine(Gens());
        
    }
    /// <summary>
    /// Changes the i'th mapgenerator seed
    /// </summary>
    /// <param name="i"></param>
    [Command(requiresAuthority = false)]
    void CmdChangeSeed(int i)
    {
        string seed = System.DateTime.Now.Millisecond.ToString();
        //Debug.LogWarning(System.DateTime.Now.Millisecond.ToString());
        mapGens[i].GetComponent<MapGenerator>().seed = seed;
    }

    /// <summary>
    /// Call GenerateCall every 3 seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator Gens()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(3);
            GenerateCall(i);
        }
    }
    public void GenerateCall(int i)
    {
        mapGens[i].GetComponent<MapGenerator>().GenerateEverything();
    }
}
