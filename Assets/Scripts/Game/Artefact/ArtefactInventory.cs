using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;


/// <summary>
/// Way to identify what artefact it is
/// </summary>
public struct ItemArtefact
{
    public string name;
    public int points;
}

/// <summary>
/// Manages the artefacts in our inventory
/// </summary>
public class ArtefactInventory : NetworkBehaviour
{

    [SerializeField]readonly SyncList<ItemArtefact> inventory = new SyncList<ItemArtefact>();

    #region ADDITION_AND_SUBTRACTION_OF_INVENTORY
    /// <summary>
    /// Called in order to add artefact to player inventory, calls update ui
    /// </summary>
    [ClientCallback]
    public void AddToInventory(string artefactName, int artefactPoints)
    {
        ItemArtefact item = new ItemArtefact();
        item.name = artefactName;
        item.points = artefactPoints;

        //Find an empty slot and add it, update the ui
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
    [Command(requiresAuthority = false)]
    private void CmdAddToInventory(ItemArtefact item)
    {
        inventory.Add(item);
    }

    /// <summary>
    /// Called in order to remove from artefact from inventory, calls <see cref="CmdInventoryRemoveAt(int, string, int, ItemArtefact)"/>
    /// </summary>
    [ClientCallback]
    public void RemoveFromInventory(int index, string name, int points)
    {
        ItemArtefact screenItemArtefact;
        screenItemArtefact.name = name;
        screenItemArtefact.points = points;

        CmdInventoryRemoveAt(index, "", 0, screenItemArtefact);
    }
    //Todo: Remove unnecessary parameters
    /// <summary>
    /// Removes artefact from synclist inventory, and calls <see cref="RFD(ItemArtefact)"/>
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdInventoryRemoveAt(int index, string name, int points, ItemArtefact ia)
    {
        Debug.Log("Removing artefact:" + inventory[index].name + " at " + index);
        inventory.RemoveAt(index);

        RFD(ia);
    }
    //Todo: Rename function
    /// <summary>
    /// Removes item from inventory of player called
    /// </summary>
    [TargetRpc]
    public void RFD(ItemArtefact screenItemArtefact)
    {
        FindObjectOfType<CanvasUIManager>().RemoveFromInventoryScreen(screenItemArtefact);
    }
    //Todo: Rename for consistant normal function to CmdFunction
    /// <summary>
    /// Remove all items from inventory
    /// </summary>
    public void ClearInventory(string name)
    {
        if (NetworkClient.localPlayer.GetComponent<PlayerController>().playerName == name)
        {
            FindObjectOfType<CanvasUIManager>().inventoryUI.ClearInventoryScreen();
        }
        CmdResetInventory();
    }
    /// <summary>
    /// Clears artefact from synclist inventory
    /// </summary>
    [Command]
    private void CmdResetInventory()
    {
        inventory.Clear();
    }
    #endregion

    #region GET_INVENTORY_OR_ARTEFACTS
    /// <summary>
    /// Get a list of the inventory
    /// </summary>
    /// <returns></returns>
    public List<ItemArtefact> GetInventory()
    {
        return inventory.ToList<ItemArtefact>();
    }
    /// <summary>
    /// Grabs a random artefact in player inventory
    /// </summary>
    public ItemArtefact GrabRandomItem()
    {
        //Safe break away in case of lots of iterations
        int tries = 0;

        ItemArtefact artefactToReturn;
        artefactToReturn.name = ""; artefactToReturn.points = 0;

        //Keep trying to find one while we haven't got one or until we reach the limit of tries
        while (artefactToReturn.name == "" || tries > 500)
        {
            artefactToReturn = inventory.ElementAt<ItemArtefact>(Random.Range(0, inventory.Count()));
            tries++;
        }

        return artefactToReturn;
    }
    /// <summary>
    /// Used for debugging, returns all artefact names
    /// </summary>
    public string GetAllArtefactNames()
    {
        string artefactList = "The artefacts are: ";
        for (int i = 0; i < inventory.Count; i++)
        {
            artefactList += inventory[i].name + ", ";
        }
        return artefactList;
    }
    #endregion


    /// <summary>
    /// Checks inventory count is above or equal to 8 - is it full
    /// </summary>
    public bool AvailableInventorySlot()
    {
        if (inventory.Count >= 8)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// Checks if inventory count is more than 0 - is there an artefact
    /// </summary>
    public bool InventoryNotEmpty()
    {
        if (inventory.Count > 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Todo: JoeComment, just making sure, can we depreciate this now. I'm pretty sure we can use count TO DEPRECIATE?: Find empty inventory slot from player inventory
    /// </summary>
    /// <returns></returns>
    public int FindEmptyInventorySlot()
    {
        if (inventory.Count >= 8)
            return -1;
        else
            return 0;
    }

}
