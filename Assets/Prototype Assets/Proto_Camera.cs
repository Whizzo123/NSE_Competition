using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proto_Camera : MonoBehaviour
{
    /*public float speed = 3.5f;
    private float X;
    private float Y;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }
    }
}*/
    public GameObject player;
    public Vector3 offset = new Vector3(0,10,-10);
    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.transform.position;
        //offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = player.transform.position + offset;
    }
}
