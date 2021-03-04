using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Globals
    [Header("Map")]
    public int width;
    public int height;
    [HideInInspector]
    private int[,] map;
    [Space]
    [SerializeField] [Tooltip("Should be a spawner, currently 'Spawner.cs'")] private GameObject spawner;
    [Space]

    [Header("Map Settings")]
    [SerializeField] [Tooltip("Type in seed to generate specific seed pattern")] private string seed;
    [SerializeField] private bool useRandomSeed;
    [Range(0, 100)]
    [SerializeField] [Tooltip("Percentace of the block to be filled")] private int randomFillPercent;
    [SerializeField] [Tooltip("The more mapSmoothinging operations that occur the less fragmented the pattern is, however it is also taxing. Diminishing Returns")] private int mapSmoothing = 5;

    [Header("StuffToSpawn")]
    [SerializeField] [Tooltip("Obstacle game objects can have infinite amount")] private GameObject[] obstacles;
    [SerializeField] [Tooltip("1st slot is an exotic artefact, 2nd slot is a rare artefact, 3rd is a common artefact.")] private GameObject[] artefactSpawner;
    [SerializeField] private int commonArtefacts = 10;
    [SerializeField] private int rareArtefacts = 3;
    [SerializeField] private int exoticArtefacts = 1;

    #endregion

    #region Get
    public int[,] GetMap()
    {
        return map;
    }
    #endregion

    private void Start()
    {
        //Find a way to generate things one at a time to look cool?
        GenerateEverything();
    }

    #region GenerationOfObstacles
    /// <summary>
    /// Generates All the obstacles in the map
    /// </summary>
    void GenerateEverything()
    {
        //DestroyAllGrass();//Remove after Development
        GenerateMap();
        SpreadItem();
        ArtefactSpawner();
    }

    /// <summary>
    /// Generates an 2d array of width and height that has on and off cells .Calls RandomFillMap().ref.MapGenerator and SmoothingMap().ref.MapGenerator. 
    /// </summary>
    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < mapSmoothing; i++)
        {
            SmoothingMap();
        }
    }

    /// <summary>
    /// Fills the array with 1's and 0's. If seed is specified, it will not be random however if 'useRandomSeed' is checked, it will be random everytime.
    /// </summary>
    void RandomFillMap()
    {
        //Can't use Random.Range if we want to specify seed.
        if (useRandomSeed)
        {
            seed = System.DateTime.Now.ToString();
        }
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Perimeter is 1
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
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

    /// <summary>
    /// Smooths out the map and makes it less chaotic. Based on cave generation/Cellular Automata smoothing. This uses the neighbouring cell states to determine what the cell state should be. Allows paths of 1's and 0's. Calls 'GetSurroundingWallCount'.ref.MapGenerator
    /// </summary>
    void SmoothingMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                //These values work quite well however they can be changed to produce vastly different results.
                if (neighbourWallTiles > 4)
                {
                    map[x, y] = 1;
                }
                else if (neighbourWallTiles < 4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Gets surrounding tiles of 'gridX' and 'gridY' in map and checks how many neighbours of the tiles are switched on.
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns>wallCount</returns>
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        //Checks a 3 by 3 grid around the specified cell location
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                //If we're on perimeter of map wallcount++ otherwise:
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    //Being honest, no idea how this works
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

    /// <summary>
    /// Spreads 'obstacles'.ref.MapGenerator on map if map cell is 0
    /// </summary>
    void SpreadItem()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (map[x, z] == 0)
                {
                    Vector3 spawnPosition = new Vector3(x, 0, z) + transform.position;
                    Instantiate(spawner, spawnPosition, Quaternion.identity).GetComponent<Spawner>().ObstacleSpawner(obstacles);
                }

            }
        }

    }
    #endregion

    /// <summary>
    /// Spreads artefacts randomly around map grid. Quite taxing to system.
    /// </summary>
    void ArtefactSpawner()
    {
        int itemsSpawned = 0;//total artefacts spawned
        int cSpawned = 0, rSpawned = 0, eSpawned = 0;//artefact type spawned
        int iterations = 0;//Used to make random less taxing in case of small artefact number and it keeps looping.

        int totalArtefacts = commonArtefacts + rareArtefacts + exoticArtefacts;
        int[,] map2 = map;

        while (itemsSpawned < totalArtefacts)
        {
            iterations++;
            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {

                    if (map2[x, y] == 0 && (UnityEngine.Random.Range(0, width * height) <= totalArtefacts * iterations) && itemsSpawned < totalArtefacts)
                    {
                        
                        Vector3 spawnPosition = new Vector3(x, 0, y) + transform.position;

                        float ran = UnityEngine.Random.Range(0, totalArtefacts - itemsSpawned);
                        if (ran < exoticArtefacts && eSpawned < exoticArtefacts)
                        {
                            Instantiate(spawner, spawnPosition, Quaternion.identity).GetComponent<Spawner>().ArtefactSpawner(artefactSpawner[0]);
                            eSpawned++;
                        }
                        else if(ran < rareArtefacts && rSpawned < rareArtefacts)
                        {
                            Instantiate(spawner, spawnPosition, Quaternion.identity).GetComponent<Spawner>().ArtefactSpawner(artefactSpawner[1]);
                            rSpawned++;
                        }
                        else if(ran < commonArtefacts && cSpawned < commonArtefacts)
                        {
                            Instantiate(spawner, spawnPosition, Quaternion.identity).GetComponent<Spawner>().ArtefactSpawner(artefactSpawner[2]);
                            cSpawned++;
                        }
                        else
                        {
                            Debug.Log("DIDN'T SPAWN ARTEFACT");
                            break;
                        }

                        itemsSpawned++;
                        map2[x, y] = 1;
                    }
                    else if (itemsSpawned >= totalArtefacts)
                    {
                        y = height + 1;
                        x = height + 1;
                        break;
                    }
                }
            }

        }
    }



    //Get rid of after development
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //GenerateEverything();
        }
    }

    /// <summary>
    /// Destroys all objects with tag 'Obstacle' and 'Spawner'. Doesn't always work, just used in development.
    /// </summary>
    public void DestroyAllGrass()
    {
        Debug.Log("DESTROY");
        GameObject[] grasses = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] artefacts = GameObject.FindGameObjectsWithTag("Artefact");

        if (grasses.Length > 0)
        {
            for (int i = 0; i < grasses.Length; i++)
            {
                Destroy(grasses[i]);
                Destroy(artefacts[i]);
            }
        }
    }

    #region DeadCode
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
    #endregion
}
