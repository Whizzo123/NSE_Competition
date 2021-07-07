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

    [SerializeField]readonly SyncList<ItemArtefact> inventory = new SyncList<ItemArtefact>();

    void Start()
    {

    }

    /// <summary>
    /// Called in order to add artefact to player inventory
    /// </summary>
    /// <param name="artefactName"></param>
    /// <param name="artefactPoints"></param>
    [ClientCallback]
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
    [Command(requiresAuthority = false)]
    private void CmdAddToInventory(ItemArtefact item)
    {
        inventory.Add(item);
    }

    


    public List<ItemArtefact> GetInventory()
    {
        return inventory.ToList<ItemArtefact>();
    }



    /// <summary>
    /// Called in order to remove from artefact from inventory
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="points"></param>
    [ClientCallback]
    public void RemoveFromInventory(int index, string name, int points)
    {

        ItemArtefact screenItemArtefact;
        screenItemArtefact.name = name;
        screenItemArtefact.points = points;

        CmdInventoryRemoveAt(index, "", 0, screenItemArtefact);
    }
    [Command(requiresAuthority = false)]
    public void CmdInventoryRemoveAt(int index, string name , int points, ItemArtefact ia)
    {
        Debug.Log("Removing artefact:" + inventory[index].name + " at " + index);
        inventory.RemoveAt(index);

        RFD(ia);
    }
    [TargetRpc]
    public void RFD(ItemArtefact screenItemArtefact)
    {
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
    private void CmdResetInventory()
    {
        inventory.Clear();
    }

    /// <summary>
    /// Checks inventory count is above or equal to 8
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
    /// Checks if inventory count is less than 8
    /// </summary>
    public bool InventoryNotEmpty()
    {
        if (inventory.Count < 8)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// TO DEPRECIATE?: Find empty inventory slot from player inventory
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
    /// Just grab the first item in the player inventory
    /// </summary>
    /// <returns></returns>
    public ItemArtefact GrabRandomItem()
    {
        //Safe break
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


    public string GetAllArtefactNames()
    {
        string artefactList = "The artefacts are: ";
        for (int i = 0; i < inventory.Count; i++)
        {
            artefactList += inventory[i].name + ", ";
        }
        return artefactList;
    }


}
