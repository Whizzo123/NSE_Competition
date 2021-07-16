using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proto_Test : MonoBehaviour
{
    public int radius = 7;
    public Vector2 regionSize = new Vector2(200, 200);
    List<Vector2> points;


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
