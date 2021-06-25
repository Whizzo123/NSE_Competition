using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;

public class NetworkConnections : GlobalEventListener
{

    /////////// Lobby info
    public static bool joinableLobby = true;
    public static string roomName;
    public static bool privateLobby = false;
    public static string password = "null";
    public static double versionNo = 1.1;

    public static int instanceNumber = 1;

    private void Awake()
    {
        if (instanceNumber == 1)
        {
            DontDestroyOnLoad(this.gameObject);
            instanceNumber++;
        }

    }

    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<ErrorMessages>();
        BoltNetwork.RegisterTokenClass<ServerInfo>();
    }

    #region BOLTNETWORK_START_AUTHORISATION_JOIN_REFUSAL

    /// <summary>
    /// If we're on the same version and we're searching for a public lobby, return true, or if we're searching for a private lobby and have entered the correct password, return true.
    /// </summary>
    private bool AuthUser(bool priv, string pass, double vers)
    {
        if (vers == versionNo)
        {
            if (priv == privateLobby)
            {
                if (pass == password)
                {
                    return true;
                }
            }
        }

        return false;
    }

    //Called when a client wants to connect, by the host
    public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
    {
        //This is called in by the host
        BoltLog.Warn("Connect Request");

        //If the client info sent is the same as the host server info, accept the client else send an error message
        var userToken = token as ServerInfo;
        if (userToken != null)
        {
            if (AuthUser(userToken.privateSession, userToken.password, userToken.versionNumber) && joinableLobby)
            {
                BoltNetwork.Accept(endpoint, userToken);
                return;
            }
        }

        ErrorMessages error = new ErrorMessages();
        error.Error = "Unknown";

        if (BoltMatchmaking.CurrentSession.ConnectionsCurrent == BoltMatchmaking.CurrentSession.ConnectionsMax)
        {
            error.Error = "The lobby is full";
        }
        else if (password != userToken.password)
        {
            error.Error = "The password was wrong";
        }
        if (!joinableLobby)
        {
            error.Error = "The lobby has now entered the game";
        }
        if (versionNo != userToken.versionNumber)
        {
            error.Error = "Tried joining a game that was on a different version, sorry for the situation.";
        }

        BoltNetwork.Refuse(endpoint, error);

    }

    //Called when the client is refused, by the client
    public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
    {
        //This is called by the refused client
        BoltLog.Warn("Connection refused!");

        var authToken = token as ErrorMessages;
        if (authToken != null)
        {
            //Display to user as text box
            Debug.LogError(authToken.Error);
        }
        BoltNetwork.Shutdown();
    }


    #endregion

    //These functions are for in case situations. For the lobby, they must remove the gameobjects that have been made. 
    //I am assuming for the game, they will have to remove the player, trap objects and such when a player disconnets

    /// <summary>
    /// <para>Called once entity is detached from bolt gameobject most likely destroyed</para>
    /// </summary>
    public override void EntityDetached(BoltEntity entity)
    {
    }
    //Called on disconnect
    public override void Disconnected(BoltConnection connection)
    {
    }
    //Called once new gameobject has been instantiated with the bolt entity component added to it
    public override void EntityAttached(BoltEntity entity)
    {
    }
}
