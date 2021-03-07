using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Main idea behind the map generator is cellular automata and a sprinkle of poisson disc sampling.ref.'Proto_Procedural.cs'. 
/// WARNING: Currently it is way to taxing on the system. Just a 100x100 grid has a 2 second delay on a high end copmputer. Look at 'ProfilerDataSimSmall.data' profiler data to see issues
/// regarding computer performance.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    #region Globals
    [Header("Map")]
    public int width = 100;
    public int height = 100;
    private int[,] map;
    [Space]

    [Header("Spawner Attributes")]
    [SerializeField] [Tooltip("Distance spawner is from the ground")] private float raycastDistance = 100f;
    [SerializeField] [Tooltip("The angle of the slope where objects won't spawn")] [Range(1, 90)] private float slopeAngle = 30f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask indestructables;
    [Space]

    [Header(" --------- Obstacles")]
    [SerializeField] [Tooltip("Obstacle game objects can have infinite amount")] private GameObject[] obstacles;
    [SerializeField] [Tooltip("Type in seed to generate specific seed pattern")] private string seed;
    [SerializeField] private bool useRandomSeed;
    [Range(0, 100)]
    [SerializeField] [Tooltip("Percentace of the block to be filled")] private int randomFillPercent = 45;
    [SerializeField] [Tooltip("The more mapSmoothinging operations that occur the less fragmented the pattern is, however it is also taxing. Diminishing Returns")] private int mapSmoothing = 3;
    [Space]

    [Header(" --------- Artefacts")]
    [SerializeField] [Tooltip("1st slot is an exotic artefact, 2nd slot is a rare artefact, 3rd is a common artefact.")] private GameObject[] artefactSpawner;
    [SerializeField] private int commonArtefacts = 50;
    [SerializeField] private int rareArtefacts = 10;
    [SerializeField] private int exoticArtefacts = 1;
    [Space]

    [Header(" ---------- Indestructables")]
    [SerializeField] [Tooltip("These indestructables are more commonly spawned")] private GameObject[] commonIndestructables;
    [SerializeField] [Tooltip("These indestructables are more rare in the world")] private GameObject[] rareIndestructables;
    [SerializeField] [Tooltip("How common commons are, a higher number means more commons than rares")][Range(1,10)] private int commonCommonality = 7;
    [SerializeField] [Tooltip("This dictates the rarity of spawned objects and how spread apart they are. A larger radius means it is more spread.")] private float radius = 17;
    [SerializeField] [Tooltip("This is the area that the indestructables spawn in.")] private Vector2 regionSize = Vector2.one;//It can be the same as the map width and heigh but I thought it better to leave here just in case.
    [SerializeField] [Tooltip("Tries to make spawn location before giving up.")] private int rejectionSamples = 30;
    private List<Vector2> points;

    #endregion


    private void Start()
    {
        //Find a way to generate things one at a time to look cool?
        //Couroutines? Don't really work. I've tried putting couroutines inside the generation themselves however couroutines do not pause the generation of everything
        //Only that particular branch of code, the rest continue running.
        StartCoroutine(GenerateAll());
    }

    /// <summary>
    /// Generates All the obstacles in the map, 
    /// </summary> 
    void GenerateEverything()
    {
        GenerateIndestructables();
        GenerateMap();
        ObstacleGeneration();
        ArtefactSpawner();
    }
    /// <summary>
    /// Generates All the obstacles in the map IN A COUROUTINE!
    /// </summary>
    IEnumerator GenerateAll()
    {
        GenerateIndestructables();
        yield return new WaitForSeconds(2);
        GenerateMap();
        yield return new WaitForSeconds(2);
        ObstacleGeneration();//This function causes the most performance issues.
        yield return new WaitForSeconds(2);
        ArtefactSpawner();
        yield return new WaitForSeconds(2);
    }


    #region MapGeneration
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

        //Randomly allocates map grid 1's and 0's
        for (int x = 0; x < width; x++)
        {
                for (int y = 0; y < height; y++)
                {
                    //Perimeter of map is 1
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
        //Smooths map
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
    #endregion

    #region Prefab Generation
    /// <summary>
    /// Spreads 'obstacles'.ref.MapGenerator on map if map cell is 0. WARNING: Due to the high volume of obstacle generation this function does, it has a massive impact on performance.
    /// </summary>
    void ObstacleGeneration()
    {

        for (int x = 0; x < width; x++)
        {
                for (int z = 0; z < height; z++)
                {
                    if (map[x, z] == 0)
                    {
                        //Spawn random Obstacle via' ObstacleSpawner()'
                        Vector3 spawnPosition = new Vector3(x, 0, z) + transform.position;
                        if (ObstacleSpawner((obstacles[UnityEngine.Random.Range(0, obstacles.Length)]), spawnPosition))
                        {
                            map[x, z] = 1;
                        }

                    }

                }

        }

    }

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

        //If all artefacts haven't been spawned, go through loop again, iteration increases which then increases the chance to spawn artefacts.
        while (itemsSpawned < totalArtefacts)
        {
            iterations++;
            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    //Artefact spawning chance  &&  map[] = 0  &&  all artefacts have spawned
                    if (map2[x, y] == 0 && (UnityEngine.Random.Range(0, width * height) <= totalArtefacts * iterations) && itemsSpawned < totalArtefacts)
                    {
                        
                        Vector3 spawnPosition = new Vector3(x, 0, y) + transform.position;
                        //Hones in on last artefact types to spawn
                        float ran = UnityEngine.Random.Range(0, totalArtefacts - itemsSpawned);
                        //There should never be more exotics than rares, more rares than commons.
                        if (ran < exoticArtefacts && eSpawned < exoticArtefacts)
                        {
                            ArtefactSpawner(artefactSpawner[0], spawnPosition);
                            eSpawned++;
                        }
                        else if(ran < rareArtefacts && rSpawned < rareArtefacts)
                        {
                            ArtefactSpawner(artefactSpawner[1], spawnPosition);
                            rSpawned++;
                        }
                        else if(ran < commonArtefacts && cSpawned < commonArtefacts)
                        {
                            ArtefactSpawner(artefactSpawner[2], spawnPosition);
                            cSpawned++;
                        }
                        else
                        {
                            break;
                        }
                        //Artefact can't spawn in same place twice.
                        itemsSpawned++;
                        map2[x, y] = 1;
                    }
                    else if (itemsSpawned >= totalArtefacts)
                    {
                        //If all artefacts spawned exit loop
                        y = height + 1;
                        x = height + 1;
                        break;
                    }
                }
            }

        }
    }

    /// <summary>
    /// Generates Indestructable objects randomly around the world. The ration between rare and common indestructables is decided by 'commonCommonality'. 
    /// </summary>
    void GenerateIndestructables()
    {
        //Call static procedural generation method and return spawnPoints
        points = Proto_Procedural.GenerateGrids(radius, regionSize, rejectionSamples);
        foreach (Vector3 vector3 in points)
        {
            //Spawn the spawners and either rare or common indestructables
            Vector3 spawnPosition = new Vector3(vector3.x, 0, vector3.y) + transform.position;
            if (UnityEngine.Random.Range(0, 10) > commonCommonality)
            {
                IndestructableSpawner(rareIndestructables[UnityEngine.Random.Range(0, rareIndestructables.Length)], spawnPosition);
            }
            else
            {
                IndestructableSpawner(commonIndestructables[UnityEngine.Random.Range(0, commonIndestructables.Length)], spawnPosition);
            }

        }
    }
    #endregion

    #region Spawner

    /// <summary>
    /// Shoots ray down from spawn location. 'objectToSpawn'.ref.Spawner is then instantiated at the hit point if the point is ground. It will also align the object to the rotation of the land.
    /// If it finds a indestructable object or the slope is too steep it will return true and not instantiate gameobject.
    /// </summary>
    public bool ObstacleSpawner(GameObject ob, Vector3 spawnPos)
    {
        //If indestructable object is found, return true.
        RaycastHit hit;
        if ((Physics.Raycast(spawnPos, Vector3.down, raycastDistance, indestructables)))
        {
            return true;
        }
        //If ground is found
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, raycastDistance, ground))
        {
            //If angle hit is bigger than 'slopeAngle' then return true
            float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(dot product)
            if (rotationAngle > slopeAngle)
            {
                return true;
            }
            else//Spawn obstacles and return false;
            {
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);//Quaternion for orientating the GO to be perpendicular to the ground
                Instantiate(ob, hit.point, spawnRotation * Quaternion.Euler(0, 90, -90));//The extra quaternion is for the temp grass model which did not come with an upright rotation immediately.
                Debug.Log("Grass is has a rotated model. The code reflects this and will apply some extra rotation. Return to 'Spawner.cs' to remove when models have upright rotation");
                return false;
            }


        }
        return false;

    }

    /// <summary>
    /// Shoots ray down from spawn location. 'objectToSpawn'.ref.Spawner is then instantiated at the hit point if the point is ground. It will also align the object to the rotation of the land.
    /// </summary>
    public void ArtefactSpawner(GameObject ob, Vector3 spawnPos)
    {
        //If ground is hit, spawn artefact
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, raycastDistance, ground))
        {
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Instantiate(ob, hit.point, spawnRotation);
        }
        ///////////////Destroy(this.gameobject);
    }

    /// <summary>
    /// Shoots down ray from spawn location. If angle of slope is higher than 30, will not spawn object. Otherwise spawn object.
    /// </summary>
    /// <param name="ob"></param>
    public void IndestructableSpawner(GameObject ob, Vector3 spawnPos)
    {
        //If ground is hit
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, raycastDistance, ground))
        {
            //If angle hit is bigger than 'slopeAngle' then return true
            float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(dot product)
            if (rotationAngle > slopeAngle)
            {
                //Destroy(this.gameObject);
            }
            else //Spawn indestructable
            {
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Instantiate(ob, hit.point, spawnRotation);
                //Destroy(this.gameObject);
            }

        }

    }
    #endregion

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