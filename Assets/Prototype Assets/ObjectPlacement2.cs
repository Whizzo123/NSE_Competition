﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement2 : MonoBehaviour
{
    public GameObject[] planeToSpawnOn;
    public int gridX;
    public int gridZ;
    public float gridSpacingOffset = 1f;
    public Vector3 gridOrigin = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        SpawnGrid();
    }

    void SpawnGrid()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                Vector3 spawnPosition = new Vector3(x * gridSpacingOffset, 0, z * gridSpacingOffset) + gridOrigin;
                PickAndSpawn(spawnPosition, Quaternion.identity);
            }
        }
    }

    void PickAndSpawn(Vector3 positionToSpawn, Quaternion rotationToSpawn)
    {
        int randomIndex = Random.Range(0, planeToSpawnOn.Length);
        Instantiate(planeToSpawnOn[randomIndex], positionToSpawn, Quaternion.Euler(0, 90, -90));

    }
}
