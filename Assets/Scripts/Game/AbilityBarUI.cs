using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarUI : MonoBehaviour
{

    public GameObject barContent;


    public void AddGameObjectToContent(GameObject gameObject)
    {
        gameObject.transform.SetParent(barContent.transform);
    }
}
