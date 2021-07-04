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
            CmdAddToInventory(item);
            FindObjectOfType<CanvasUIManager>().PopupArtefactPickupDisplay(item);
            FindObjectOfType<CanvasUIManager>().AddToInventoryScreen(item);
        }
        else
        {
           Debug.LogError("Inventory is full");
        }
    }

    /// <summary>
    /// As synclists can only be modified on the server this command has to be called to add items to inventory
    /// </summary>
    [Command]
    private void CmdAddToInventory(ItemArtefact item)
    {
        inventory.Add(item);
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
        if (inventory.Count >= 8)
            return -1;
        else
            return 0;
    }

    /// <summary>
    /// Called in order to remove from artefact from inventory
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="points"></param>
    public void RemoveFromInventory(int index, string name, int points)
    {
        CmdInventoryRemoveAt(index);
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
        CmdResetInventory();
    }

    [Command]
    private void CmdInventoryRemoveAt(int index)
    {
        inventory.RemoveAt(index);
    }

    [Command]
    private void CmdResetInventory()
    {
        inventory.Clear();
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
