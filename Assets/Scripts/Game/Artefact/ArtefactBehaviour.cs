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
    private bool avaliableForPickup;

    public override void OnStartAuthority()
    {
        avaliableForPickup = false;
    }

    public void EnableForPickup()
    {
        avaliableForPickup = true;
        CmdEnableRenderer();
    }

    [Command]
    private void CmdEnableRenderer()
    {
        RpcEnableRenderer();
    }

    [ClientRpc]
    private void RpcEnableRenderer()
    {
        GetComponent<MeshRenderer>().enabled = true;
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

}
