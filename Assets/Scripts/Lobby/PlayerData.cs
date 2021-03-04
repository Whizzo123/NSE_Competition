using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Bolt;

[BoltGlobalBehaviour("LobbyScene", "GameScene")]
public class PlayerData : GlobalEventListener
{
    #region Variables
    private Dictionary<BoltConnection, string> connectionToUsername;
    private string serverUsername;
    #endregion

    /// <summary>
    /// Run after bolt instance is started for (client or server)
    /// </summary>
    public override void BoltStartDone()
    {
        connectionToUsername = new Dictionary<BoltConnection, string>();
    }


    public void AddUsername(BoltConnection connection, string username)
    {
        if(connection != null)
        {
            BoltLog.Info("AddingUsername: (connection:-" + connection.ConnectionId + " / username:-" + username);
            connectionToUsername[connection] = username;
        }
        else
        {
            BoltLog.Info("ServerAddingUsername: (connection:-null / username:-" + username);
            serverUsername = username;
        }
    }

    public void RegisterConnection(BoltConnection connection)
    {
        BoltLog.Info("Registering connection: " + connection.ConnectionId);
        if (connection != null)
            connectionToUsername.Add(connection, "");
        else
            BoltLog.Info("RegisterConnection:- connection was null");

        BoltLog.Info("Print connections stored");

        foreach(BoltConnection c in connectionToUsername.Keys)
        {
            BoltLog.Info("Connection in list iD of: " + c.ConnectionId);
        }
    }

    public string GetUsername(BoltConnection connection)
    {
        if (connection != null)
        {
            return connectionToUsername[connection];
        }
        else
            return serverUsername;
    }

}
