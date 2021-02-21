using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proto_Camera : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = player.transform.position + offset;
    }
}
