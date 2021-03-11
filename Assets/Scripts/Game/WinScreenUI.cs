using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenUI : MonoBehaviour
{
    public GameObject winElementPrefab;
    public GameObject content;

    public void AddToContent(string name, int points)
    {
        GameObject go = Instantiate(winElementPrefab);
        go.transform.SetParent(transform);
        go.GetComponent<WinElementUI>().PopulateFields(name, transform.childCount - 1, points);
    }

}
