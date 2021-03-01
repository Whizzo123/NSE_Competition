using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaySpawner : MonoBehaviour
{
    public GameObject itemToSpread;
    public int numItemsToSpawn = 10;

    public float itemXSpread = 10;
    public float itemYSpread = 0;
    public float itemZSpread = 10;
    // Start is called before the first frame update
    void Start()
    {

        SpreadItem();
    }

    void SpreadItem()
    {
        for (int x = 0; x < itemXSpread; x++)
        {
            for (int y = 0; y < itemZSpread; y++)
            {
                Vector3 spawnPosition = new Vector3(x, 0, y) + transform.position;
                GameObject clone = Instantiate(itemToSpread, spawnPosition, Quaternion.identity);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
