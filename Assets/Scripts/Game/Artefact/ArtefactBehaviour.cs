using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ArtefactRarity { Common, Rare, Exotic}

/// <summary>
/// Attatched to the artefact object. Contains information for the artefact and contains function for the gameobject behaviour
/// </summary>
public class ArtefactBehaviour : NetworkBehaviour
{
    [SyncVar] [SerializeField] private string artefactName;
    [SyncVar] [SerializeField] private int points;
    [SyncVar] [SerializeField] private ArtefactRarity rarity;
    [SyncVar] [SerializeField] public bool avaliableForPickup = false;
    //Todo: No need for public variable if we have a getter right?
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    /// <summary>
    /// Sets up information for this artefact
    /// </summary>
    public void PopulateData(string dataName, ArtefactRarity rarity)
    {
        artefactName = dataName;
        this.rarity = rarity;

        switch (rarity)
        {
            case ArtefactRarity.Common:
                points = 200;
                break;
            case ArtefactRarity.Rare:
                points = 1000;
                break;
            case ArtefactRarity.Exotic:
                points = 5000;
                break;
        }
    }

    #region VISIBILITY
    /// <summary>
    /// Makes the artefact visible and availbleforpickup
    /// </summary>
    public void EnableForPickup()
    {
        CmdSetAvaliableForPickup(true);
        CmdToggleRenderer(true);
    }
    [Command(requiresAuthority = false)]
    private void CmdSetAvaliableForPickup(bool value)
    {
        avaliableForPickup = value;
    }
    [Command]
    private void CmdToggleRenderer(bool toggle)
    {
        RpcToggleRenderer(toggle);
        GetComponent<MeshRenderer>().enabled = toggle;
    }
    //Todo: RpcToggleRenderer could get confusing between ClientRpc and TargetRpc
    [ClientRpc]
    private void RpcToggleRenderer(bool toggle)
    {
        GetComponent<MeshRenderer>().enabled = toggle;
        Debug.Log("RpcToggleRenderer: " + toggle);
    }
    #endregion

    #region GETTERS
    public ArtefactRarity GetRarity()
    {
        return rarity;
    }

    public string GetArtefactName()
    {
        return artefactName;
    }

    public int GetPoints()
    {
        return points;
    }

    public bool IsAvaliableForPickup()
    {
        return avaliableForPickup;
    }
    #endregion
}
