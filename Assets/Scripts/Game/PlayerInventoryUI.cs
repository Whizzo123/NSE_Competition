using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventoryUI : MonoBehaviour
{
    private Dictionary<string, GameObject> inventoryTabs;
    public GameObject inventoryTabPrefab;

    void Start()
    {
        inventoryTabs = new Dictionary<string, GameObject>();
    }

    
    public void AddInventoryItem(ItemArtefact item)
    {
        if(inventoryTabs.ContainsKey(item.name))
        {
            UpdateInventoryItem(item, true);
        }
        else
        {
            GameObject go = Instantiate(inventoryTabPrefab, this.transform);
            go.GetComponent<InventoryTabUI>().AddText(item.name, 1);
            inventoryTabs.Add(item.name, go);
        }
    }

    public void SubtractInventoryItem(ItemArtefact item)
    {
        if(inventoryTabs.ContainsKey(item.name))
        {
            UpdateInventoryItem(item, false);
        }
        else
        {
            BoltLog.Error("Attempting to subtract when no inventory tab exists under that name");
        }
    }

    private void UpdateInventoryItem(ItemArtefact item, bool adding)
    {
        if (adding)
        {
            inventoryTabs[item.name].GetComponent<InventoryTabUI>().EditCount(1);
        }
        else
        {
            inventoryTabs[item.name].GetComponent<InventoryTabUI>().EditCount(-1);
            if(inventoryTabs[item.name].GetComponent<InventoryTabUI>().GrabPersonalCount() <= 0)
            {
                RemoveInventoryItem(item.name);
            }
        }
    }

    public void ClearInventoryScreen()
    {
        string[] inventoryKeys = inventoryTabs.Keys.ToArray<string>();
        for (int i = 0; i < inventoryKeys.Length; i++)
        {
            RemoveInventoryItem(inventoryKeys[i]);
        }
    }

    public void RemoveInventoryItem(string name)
    {
        Destroy(inventoryTabs[name]);
        inventoryTabs.Remove(name);
    }
}
