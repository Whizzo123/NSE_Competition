using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

/// <summary>
/// Main idea behind the map generator is cellular automata and a sprinkle of poisson disc sampling.ref.'Proto_Procedural.cs'. 
/// </summary>
public class MapGenerator : NetworkBehaviour
{
    #region Globals
    [Header("Map")]
    public int width = 100;
    public int height = 100;
    private int[,] map;
    [Space]

    [Header("Spawner Attributes")]
    [SerializeField] [Tooltip("Distance spawner is from the ground")] private float raycastDistance = 100f;
    [SerializeField] [Tooltip("The angle of the slope where objects won't spawn")] [Range(1, 90)] private float slopeAngle = 35f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask indestructables;
    [SerializeField] private LayerMask obstaclesLayer;
    [SerializeField] private LayerMask obsground;
    [Space]

    [Header(" --------- Obstacles")]

    [SerializeField] [Tooltip("Obstacle game objects can have infinite amount")] private GameObject[] obstacles;
    [SerializeField] [Tooltip("Type in seed to generate specific seed pattern")][SyncVar] public string seed = "";
    [SerializeField] private bool useRandomSeed;
    [Range(0, 100)]
    [SerializeField] [Tooltip("Percentace of the block to be filled")] private int randomFillPercent = 45;
    [SerializeField] [Tooltip("The more mapSmoothinging operations that occur the less fragmented the pattern is, however it is also taxing. Diminishing Returns")] private int mapSmoothing = 3;
    [SerializeField] [Tooltip("The spacing between each spawn of any objects that use map, the scale of spawned objects have to be manually tweaked. This is equivalent to multiplying the world position.")] private int spawnPosScale = 1;
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
    [SerializeField] [Tooltip("This dictates the rarity of spawned objects and how spread apart they are. A larger radius means it is more spread.")] private int radius = 17;
    [SerializeField] [Tooltip("This is the area that the indestructables spawn in.")] private Vector2 regionSize = Vector2.one;//It can be the same as the map width and heigh but I thought it better to leave here just in case.
    [SerializeField] [Tooltip("Tries to make spawn location before giving up.")] private int rejectionSamples = 30;
    private List<Vector2> points;
    [Space]

    [Header(" ---------- Abilities")]
    [SerializeField] [Tooltip("Amount of ability charges to spawn per biome")] private int abilityAmountToSpawn;
    [SerializeField] [Tooltip("Ability charges to spawn")] private GameObject[] abilityChargesToSpawn;



    #endregion

    //[ClientRpc]
    //public void ChangeSeed(string newSeed)
    //{
    //    seed = newSeed;
    //}



    System.Random f;
    /// <summary>
    /// Generates All the obstacles in the map, 
    /// </summary> 
    [ClientRpc]
    public void GenerateEverything()
    {

        //Debug.LogWarning("The seed is: " + seed);
        //Debug.LogWarning("pseudoRandom : " + seed.GetStableHashCode());
        //Can't use Random.Range if we want to specify seed, and UnityEngine.Random will affect repeated uses of random that are connected
        System.Random pseudoRandom = new System.Random(seed.GetStableHashCode());
        f = pseudoRandom;

        GenerateIndestructables(pseudoRandom);
        StartCoroutine(rest());

    }

    IEnumerator rest()
    {

       yield return new WaitForSeconds(3);
        System.Random pseudoRandom = f;
        GenerateMap(pseudoRandom);
        ObstacleGeneration(pseudoRandom);
        ArtefactSpawner();
    }


    #region MapGeneration
    /// <summary>
    /// Generates an 2d array of width and height that has on and off cells .Calls RandomFillMap().ref.MapGenerator and SmoothingMap().ref.MapGenerator. 
    /// </summary>
    void GenerateMap(System.Random pseudoRandom)
    {
        map = new int[width, height];
        RandomFillMap(pseudoRandom);

        for (int i = 0; i < mapSmoothing; i++)
        {
            SmoothingMap();
        }
    }

    /// <summary>
    /// Fills the array with 1's and 0's. If seed is specified, it will not be random however if 'useRandomSeed' is checked, it will be random everytime.
    /// </summary>
    void RandomFillMap(System.Random pseudoRandom)
    {

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
    /// Spreads 'obstacles'.ref.MapGenerator on map if map cell is 0.
    /// </summary>
    void ObstacleGeneration(System.Random pseudoRandom)
    {
       // Debug.LogWarning("Map.length obst :" + map.Length);
        for (int x = 0; x < width; x++)
        {
                for (int z = 0; z < height; z++)
                {
                    if (map[x, z] == 0)
                    {
                        //Spawn random Obstacle via' ObstacleSpawner()'
                        Vector3 spawnPosition = new Vector3(x * spawnPosScale, 0, z *spawnPosScale) + transform.position;
                        //If anything is obstructing the spawning, it will make the map = 1
                        if (ObstacleSpawner(obstacles[pseudoRandom.Next(obstacles.Length)], spawnPosition))
                        {
                            map[x, z] = 1;
                       // Debug.LogWarning("Map has been changed to 1 : " + map[x, z] + " is on " + x + " : " + z);
                        }

                    }

                }

        }
       // Debug.LogWarning("Map.length after :" + map.Length);

    }

    /// <summary>
    /// Spreads artefacts randomly around map grid.
    /// </summary>
    [Server]
    void ArtefactSpawner()
    {
        int itemsSpawned = 0;//total artefacts spawned
        int cSpawned = 0, rSpawned = 0, eSpawned = 0;//artefact type spawned
        int iterations = 0;//Used to make random less taxing in case of small artefact number and it keeps looping.

        int totalArtefacts = commonArtefacts + rareArtefacts + exoticArtefacts;
        

        //If all artefacts haven't been spawned, go through loop again, iteration increases which then increases the chance to spawn artefacts.
        while (itemsSpawned < totalArtefacts)
        {
            iterations++;
            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    //Artefact spawning chance  &&  map[] = 0  &&  all artefacts have spawned
                    if (map[x, y] == 0 && (UnityEngine.Random.Range(0, width * height) <= totalArtefacts * iterations) && itemsSpawned < totalArtefacts)
                    {
                        
                        Vector3 spawnPosition = new Vector3(x * spawnPosScale, 0, y * spawnPosScale) + transform.position;
                        //Hones in on last artefact types to spawn
                        float ran = UnityEngine.Random.Range(0, totalArtefacts - itemsSpawned);
                        //There should never be more exotics than rares, more rares than commons.
                        if (ran < exoticArtefacts && eSpawned < exoticArtefacts)
                        {
                            
                            eSpawned = eSpawned + ArtefactSpawner(artefactSpawner[0], spawnPosition, ArtefactRarity.Exotic);
                        }
                        else if(ran < rareArtefacts && rSpawned < rareArtefacts)
                        {
                            
                            rSpawned += ArtefactSpawner(artefactSpawner[1], spawnPosition, ArtefactRarity.Rare);
                        }
                        else if(ran < commonArtefacts && cSpawned < commonArtefacts)
                        {
                         
                            cSpawned += ArtefactSpawner(artefactSpawner[2], spawnPosition, ArtefactRarity.Common);
                        }
                        else
                        {
                            break;
                        }
                        //Artefact can't spawn in same place twice.
                        itemsSpawned = eSpawned + rSpawned + cSpawned;
                        map[x, y] = 1;
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
    void GenerateIndestructables(System.Random pseudoRandom)
    {
        //Call static procedural generation method and return spawnPoints
        points = Proto_Procedural.GenerateGrids(pseudoRandom, radius, regionSize, rejectionSamples);
        foreach (Vector2 vector2 in points)
        {
            //Spawn the spawners and either rare or common indestructables
            Vector3 spawnPosition = new Vector3(vector2.x, 0, vector2.y) + transform.position;
            if (pseudoRandom.Next(10) > commonCommonality)
            {
                IndestructableSpawner(rareIndestructables[pseudoRandom.Next(rareIndestructables.Length)], spawnPosition, pseudoRandom);
            }
            else
            {
                IndestructableSpawner(commonIndestructables[pseudoRandom.Next(commonIndestructables.Length)], spawnPosition, pseudoRandom);
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
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, raycastDistance, indestructables))
        {
            //Instantiate(Resources.Load<GameObject>("Sphere") as GameObject, hit.point, Quaternion.identity);
            return true;
        }
        //RaycastHit[] hit2;
        //hit2 = Physics.RaycastAll(spawnPos, Vector3.down, raycastDistance);
        //foreach (RaycastHit item in hit2)
        //{
        //    if (item.collider.gameObject.tag.StartsWith("indestr"))
        //    {

        //    }
        //}
        LayerMask lm = ground;
        if (lm.value == LayerMask.GetMask("SwampGround"))
        {
            lm = LayerMask.GetMask("SwampWater");
            
        }
        //If ground is found
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, raycastDistance, lm))
        {
            //If angle hit is bigger than 'slopeAngle' then return true
            float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(dot product)
            if (rotationAngle > slopeAngle)
            {
                //Instantiate(Resources.Load<GameObject>("Sphere2") as GameObject, hit.point, Quaternion.identity);
                return true;
            }
            else //Spawn obstacles and return false;
            {
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);//Quaternion for orientating the GO to be perpendicular to the ground
                spawnRotation *= Quaternion.Euler(-90, 0, 0);
                GameObject go = Instantiate(ob, hit.point, spawnRotation);
                //NetworkServer.Spawn(go);
                return false;
            }
        }
        

        return false;

    }

    /// <summary>
    /// Shoots ray down from spawn location. 'objectToSpawn'.ref.Spawner is then instantiated at the hit point if the point is ground. It will also align the object to the rotation of the land.
    /// </summary>
    [Server]
    public int ArtefactSpawner(GameObject ob, Vector3 spawnPos, ArtefactRarity rarity)
    {
        //If ground is hit, spawn artefact

        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, raycastDistance, ground))
        {
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            GameObject go = Instantiate(ob, hit.point + (Vector3.up * 2), spawnRotation);
            go.GetComponent<ArtefactBehaviour>().PopulateData(go.name, rarity);
            NetworkServer.Spawn(go);
            return 1;
        }
        else
        {
            
            return 0;
        }
    }

    /// <summary>
    /// Shoots down ray from spawn location. If angle of slope is higher than 30, will not spawn object. Otherwise spawn object.
    /// </summary>
    /// <param name="ob"></param>
    public void IndestructableSpawner(GameObject ob, Vector3 spawnPos, System.Random ran)
    {

        //If ground is hit
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, raycastDistance, ground))
        {
            //If angle hit is bigger than 'slopeAngle' then return true
            float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(dot product)
            if (rotationAngle > slopeAngle) {return;
            }
            else//Spawn indestructable
            {
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                if (ob.gameObject.name.Contains("Rock"))
                {
                    Quaternion randAllRotation = Quaternion.Euler(ran.Next(0,360), ran.Next(0, 360), ran.Next(0, 360));
                    spawnRotation *= randAllRotation;

                }
                else
                {
                    Quaternion randYRotation = Quaternion.Euler(0, ran.Next(0, 360), 0);
                    spawnRotation *= randYRotation;
                }
                int randScale = ran.Next(1, 5);
                spawnRotation *= Quaternion.Euler(-90, 0, 0);
                GameObject go = Instantiate(ob, hit.point, spawnRotation);
                go.transform.localScale = new Vector3(randScale, randScale, randScale);
                //NetworkServer.Spawn(go);
            }

        }

    }
    #endregion

}