using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2D Representation of the <see cref="Proto_Procedural"/> script, draws in editor
/// </summary>
public class Proto_Test : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Radius for generating objects")]public int radius = 7;
    [Tooltip("Region size for aree. NOTE: Be careful going over 200,200, and be careful changing it in increments")] public Vector2 regionSize = new Vector2(200, 200);

    [Tooltip("Points generated")] private List<Vector2> points;


    public void OnValidate()
    {
        System.Random f = new System.Random(Random.Range(0, 10));
        points = Proto_Procedural.GenerateGrids(f, radius, regionSize);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(regionSize / 2, regionSize);
        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                Gizmos.DrawSphere(point, radius);
            }
        }
    }
    //private void OnDrawGizmos()
    //{
    //    if (map != null)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            for (int y = 0; y < height; y++)
    //            {
    //                Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
    //                //Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
    //                Vector3 pos = new Vector3(x, 0, y);
    //                Gizmos.DrawCube(pos, Vector3.one);
    //            }
    //        }
    //    }
    //}
}
