using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// The visuals for the artefacts in the inventory.
/// </summary>
public class PlayerInventoryUI : MonoBehaviour
{
    [Tooltip("The indivual elements for artefacts")]private Dictionary<string, GameObject> inventoryTabs;
    [Tooltip("The bounding box for the artefact elements")]public GameObject inventoryTabPrefab;

    void Start()
    {
        inventoryTabs = new Dictionary<string, GameObject>();
    }

    
    /// <summary>
    /// Called in order to add item to inventory screen
    /// </summary>
    public void AddInventoryItem(ItemArtefact item)
    {
        //Updates count or adds it to screen
        if(inventoryTabs.ContainsKey(item.name))
        {
            UpdateInventoryItem(item, true);
        }
        else
        {
            GameObject go = Instantiate(inventoryTabPrefab, this.transform);
            go.GetComponent<InventoryTabUI>().Populate(item.name, 1);
            inventoryTabs.Add(item.name, go);
        }
    }
    /// <summary>
    /// Called in order to update the count for artefacts and visually the inventory item count
    /// </summary>
    private void UpdateInventoryItem(ItemArtefact item, bool adding)
    {

        if (adding)
        {
            inventoryTabs[item.name].GetComponent<InventoryTabUI>().EditCount(1);
        }
        else
        {
            inventoryTabs[item.name].GetComponent<InventoryTabUI>().EditCount(-1);
           
            if (inventoryTabs[item.name].GetComponent<InventoryTabUI>().GrabPersonalCount() <= 0)
            {
                RemoveInventoryItem(item.name);
            }
        }
    }
//Todo: JoeComment why do we need two ways to do subtraction with the inventoryTabs.
    /// <summary>
    /// Called in order to remove item from inventory by one count from screen
    /// </summary>
    /// <param name="item"></param>
    public void SubtractInventoryItem(ItemArtefact item)
    {
        if(inventoryTabs.ContainsKey(item.name))
        {
            UpdateInventoryItem(item, false);
        }
        else
        {
            Debug.LogError("Attempting to subtract when no inventory tab exists under that name");
        }
    }



    /// <summary>
    /// Clears entire inventory screen
    /// </summary>
    public void ClearInventoryScreen()
    {
        string[] inventoryKeys = inventoryTabs.Keys.ToArray<string>();
        for (int i = 0; i < inventoryKeys.Length; i++)
        {
            RemoveInventoryItem(inventoryKeys[i]);
        }
    }
    /// <summary>
    /// Destroys inventory item uielement and removes from inventory screen list
    /// </summary>
    public void RemoveInventoryItem(string name)
    {
        Destroy(inventoryTabs[name]);
        inventoryTabs.Remove(name);
    }
}
