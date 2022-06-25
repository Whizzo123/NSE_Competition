using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{

    [SyncVar] public int connectionID;
    [SyncVar] public int playerIDNumber;
    [SyncVar] public ulong playerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;

    private MyNetworkManager manager;

    private MyNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = MyNetworkManager.singleton as MyNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        gameObject.name = "LocalGamePlayer";
        
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }
    public override void OnStartClient()
    {
        Manager.matchmakingPlayers.Add(this);
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();
        CmdSetPlayerName(SteamFriends.GetPersonaName());

    }
    public override void OnStopClient()
    {
        Manager.matchmakingPlayers.Remove(this);
        LobbyController.instance.UpdatePlayerList();
    }
    [Command]
    private void CmdSetPlayerName(string playerNameToSet)
    {
        this.PlayerNameUpdate(this.playerName, playerNameToSet);
    }
    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.playerName = newValue;
        }
        if (isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }
}
