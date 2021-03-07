using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{

    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSamples = 30;
    public float displayRadius = 1;
    public GameObject prefab;

    List<Vector2> points;

    void OnValidate()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        //points = Proto_Procedural.GenerateGrids(radius, regionSize, rejectionSamples);
        foreach (Vector2 vector2 in points)
        {
            Instantiate(prefab, vector2, Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
