using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ArtefactRarity { Common, Rare, Exotic}

public class ArtefactBehaviour : NetworkBehaviour
{
    [SyncVar]
    private string artefactName;
    [SyncVar]
    private int points;
    [SyncVar]
    private ArtefactRarity rarity;
    [SyncVar]
    public bool avaliableForPickup = false;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public override void OnStartAuthority()
    {

    }

    public void EnableForPickup()
    {
        CmdSetAvaliableForPickup(true);
        CmdToggleRenderer(true);
    }

    [Command]
    private void CmdToggleRenderer(bool toggle)
    {
        RpcToggleRenderer(toggle);
        GetComponent<MeshRenderer>().enabled = toggle;
    }

    [ClientRpc]
    private void RpcToggleRenderer(bool toggle)
    {
        GetComponent<MeshRenderer>().enabled = toggle;
        Debug.Log("RpcToggleRenderer: " + toggle);
    }

    public void PopulateData(string dataName, ArtefactRarity rarity)
    {
        artefactName = dataName;
        this.rarity = rarity;
        switch(rarity)
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

    [Command(requiresAuthority = false)]
    private void CmdSetAvaliableForPickup(bool value)
    {
        avaliableForPickup = value;
    }

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
}
