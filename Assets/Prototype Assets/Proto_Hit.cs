using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proto_Hit : MonoBehaviour
{
    public List<GameObject> hitObjects;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit with machete");
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("Hit obstacle");
            //Destroy(other.gameObject);
            hitObjects.Add(other.gameObject);
        }
    }
}
