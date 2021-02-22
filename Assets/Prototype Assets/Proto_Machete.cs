using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proto_Machete : MonoBehaviour
{

    public void DestroySelf()
    {
        Debug.Log("Destroying Self");
        List<GameObject> destroyObject;

        destroyObject = FindObjectOfType<Proto_Hit >().hitObjects;
        foreach (GameObject item in destroyObject)
        {
            Destroy(item);
        }
        FindObjectOfType<T_CharacterMovementScript>().pressed = false;
        Destroy(this.gameObject);

    }
}
