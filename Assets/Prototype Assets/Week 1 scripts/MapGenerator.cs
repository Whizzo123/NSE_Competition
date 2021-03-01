using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;
    public int smooth = 5;

    [Range(0,100)]
    public int randomFillPercent;

    public int[,] map;


    public void DestroyAllGrass()
    {
        GameObject[] grasses = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        if (spawners.Length > 0 && grasses.Length > 0)
        {
            for (int i = 0; i < grasses.Length; i++)
            {
                Destroy(spawners[i]);
                Destroy(grasses[i]);
            }
        }

    }
    private void Start()
    {
        GenerateEverything();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateEverything();
        }
    }
    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smooth; i++)
        {
          SmoothMap();
        }
    }

    void GenerateEverything()
    {
        DestroyAllGrass();
        GenerateMap();
        SpreadItem();
    }

    public GameObject itemToSpread;

    void SpreadItem()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (map[x,z] == 0)
                {
                    Vector3 spawnPosition = new Vector3(x, 0, z) + transform.position;
                    GameObject clone = Instantiate(itemToSpread, spawnPosition, Quaternion.identity);
                }

            }
        }

    }


    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height-1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
                
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                {
                    map[x, y] = 1;
                }else if(neighbourWallTiles < 4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX+1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY+1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }

            }
        }
        return wallCount;
    }

    /*
    private void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    //Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
                    Vector3 pos = new Vector3(x, 0, y);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }*/
}
