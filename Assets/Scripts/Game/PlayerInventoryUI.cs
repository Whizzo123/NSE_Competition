using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                RemoveInventoryItem(item);
            }
        }
    }

    public void RemoveInventoryItem(ItemArtefact item)
    {
        Destroy(inventoryTabs[item.name]);
        inventoryTabs.Remove(item.name);
    }
}
