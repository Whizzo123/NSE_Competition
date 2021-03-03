using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutBarUI : MonoBehaviour
{

    public GameObject loadoutContent;

    public void AddGameObjectToContent(GameObject gameObject)
    {
        gameObject.transform.SetParent(loadoutContent.transform);
    }

}
