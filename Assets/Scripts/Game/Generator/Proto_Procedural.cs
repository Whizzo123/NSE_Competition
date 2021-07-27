using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Proto_Procedural 
{

    /// <summary>
    /// This procedural generation algorithm is called Poisson Disc Sampling, by Bridson. It has since had many alterations to make it more efficient and produce more tightly packed points.
    /// </summary>
    public static List<Vector2> GenerateGrids(System.Random pseudoRandom, int radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
    {
        float cellSize = radius / Mathf.Sqrt(2);
        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];

        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoint = new List<Vector2>();

        spawnPoint.Add(sampleRegionSize / 2);
        while(spawnPoint.Count > 0)
        {
            int spawnIndex = pseudoRandom.Next(0, spawnPoint.Count);//Random.Range(0, spawnPoint.Count);
            Vector2 spawncenter = spawnPoint[spawnIndex];
            bool candidateAccepted = false;
            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = pseudoRandom.Next() * Mathf.PI * 2;//Random.value * Mathf.PI * 2; <---------------- This was the culprit, psuedoRandom(1)
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawncenter + dir * pseudoRandom.Next(radius, 2 * radius);//Random.Range(radius, 2 * radius);
                if (isValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoint.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;

                }
            }
            if (!candidateAccepted)
            {
                spawnPoint.RemoveAt(spawnIndex);
            }
        }
        return points;
    }

    static bool isValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >=  0 && candidate.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y  = searchStartY; y  <= searchEndY; y ++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if(sqrDst < radius*radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}
