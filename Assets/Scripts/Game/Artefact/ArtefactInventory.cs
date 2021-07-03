using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;


public struct ItemArtefact
{
    public string name;
    public int points;
}

public class ArtefactInventory : NetworkBehaviour
{

    readonly SyncList<ItemArtefact> inventory = new SyncList<ItemArtefact>();

    void Start()
    {

    }

    /// <summary>
    /// Called in order to add artefact to player inventory
    /// </summary>
    /// <param name="artefactName"></param>
    /// <param name="artefactPoints"></param>
    public void AddToInventory(string artefactName, int artefactPoints)
    {
        ItemArtefact item = new ItemArtefact();
        item.name = artefactName;
        item.points = artefactPoints;
        int emptySlot = FindEmptyInventorySlot();
        if (emptySlot > -1)
        {
            inventory[emptySlot] = item;
            FindObjectOfType<CanvasUIManager>().PopupArtefactPickupDisplay(item);
            FindObjectOfType<CanvasUIManager>().AddToInventoryScreen(item);
        }
        else
        {
           Debug.LogError("Inventory is full");
        }
    }

    public List<ItemArtefact> GetInventory()
    {
        return inventory.ToList<ItemArtefact>();
    }

    /// <summary>
    /// Find empty inventory slot from player inventory
    /// </summary>
    /// <returns></returns>
    public int FindEmptyInventorySlot()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].name == "")
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Called in order to remove from artefact from inventory
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="points"></param>
    public void RemoveFromInventory(int index, string name, int points)
    {
        ItemArtefact itemArtefact = inventory[index];
        itemArtefact.name = "";
        itemArtefact.points = 0;
        inventory[index] = itemArtefact;
        ItemArtefact screenItemArtefact;
        screenItemArtefact.name = name;
        screenItemArtefact.points = points;
        FindObjectOfType<CanvasUIManager>().RemoveFromInventoryScreen(screenItemArtefact);
    }
    /// <summary>
    /// Remove all items from inventory
    /// </summary>
    public void ClearInventory()
    {
        FindObjectOfType<CanvasUIManager>().inventoryUI.ClearInventoryScreen();
        for (int i = 0; i < inventory.Count; i++)
        {
            ItemArtefact itemArtefact = inventory[i];
            itemArtefact.name = "";
            itemArtefact.points = 0;
            inventory[i] = itemArtefact;
        }
    }

    /// <summary>
    /// Check to see whether inventory has any empty slots
    /// </summary>
    /// <returns></returns>
    public bool IsInventoryEmpty()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].name == "")
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Just grab the first item in the player inventory
    /// </summary>
    /// <returns></returns>
    public ItemArtefact GrabRandomItem()
    {
        return inventory[0];
    }

    public bool InventoryNotEmpty()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].points > 0) return true;
        }
        return false;
    }

}
