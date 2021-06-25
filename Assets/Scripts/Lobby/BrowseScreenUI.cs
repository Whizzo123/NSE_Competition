using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;
using System;
using Steamworks;
using Mirror.Discovery;

public class BrowseScreenUI : MonoBehaviour
{
    #region Variables
    public GameObject sessionListObjectPrefab;
    public GameObject noServerFoundText;
    public GameObject serverList;

    public event Action<LobbyInfo> OnClickJoinSession;

    private LobbyUIManager lobbyUIManager;

    private float waitTime = 5f;
    private float currentWaitTime = 0f;
    #endregion 

    private void Start()
    {
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();
    }


    /// <summary>
    /// Resets UI by destroying the old outdated server list uielements
    /// </summary>
    private void ResetUI()
    {
        noServerFoundText.SetActive(true);

        foreach(ServerListRoomUI child in serverList.transform.GetComponentsInChildren<ServerListRoomUI>())
        {
            Destroy(child.gameObject);
        }
    }


    /// <summary>
    /// Updated frequently with all servers hosted currently on the bolt network
    /// </summary>
    /// <param name="sessionList"></param>
    public void SessionListUpdated(List<LobbyInfo> lobbies)
    {
        Debug.Log("Recieved session list update");

        ResetUI();

        if (lobbies.Count == 0)
        {
            noServerFoundText.SetActive(true);
            return;
        }

        noServerFoundText.SetActive(false);

            foreach (LobbyInfo info in lobbies)
            {
                //Searches for any lobby that is in the current version.
                GameObject serverEntryGO = Instantiate(sessionListObjectPrefab, serverList.transform, false);

                ServerListRoomUI serverEntryUI = serverEntryGO.GetComponent<ServerListRoomUI>();
                serverEntryUI.Populate(info, UnityEngine.Random.ColorHSV());
            }
    }

    private void Update()
    {
        if (!LobbyUIManager.useSteamMatchmaking)
        {
            if (currentWaitTime <= 0)
            {
                Dictionary<long, ServerResponse> servers = lobbyUIManager.discoveredServers;
                ResetUI();
                if(servers.Count == 0)
                {
                    noServerFoundText.SetActive(true);
                    return;
                }

                noServerFoundText.SetActive(false);
                foreach (long serverID in servers.Keys)
                {
                    GameObject serverEntryGO = Instantiate(sessionListObjectPrefab, serverList.transform, false);
                    ServerListRoomUI serverEntryUI = serverEntryGO.GetComponent<ServerListRoomUI>();
                    serverEntryUI.Populate(serverID, UnityEngine.Random.ColorHSV());
                }
                currentWaitTime = waitTime;
            }
            else
            {
                currentWaitTime -= Time.deltaTime;
            }
        }
    }


    #region dedcode
    /*
    // [0] == standard. Always there. 
    // [1] == true, false for privateSession. It is double the length of the string and + 0 for false, and + 1 for true.
    // [2] == Start of string
    // [3 to x] == String password
    // [x + 1] == Terminator.
    //Examples:
    //{2,0,0,0} Only has false priv session, no token for password
    //{2,1,0,0} Only has true priv session, no token for password 
    //Notice they still have beginning 0 and terminator 0
    //{2,11,0,140,194,196,198,200, 0} True, password == "Fabcd". Notice [1] is double the lenght and +1 for true. Also notice that the characters are 2 digits apart.
    //{2,3,0,140,0} true, password "F". 
    //public byte[] publicServerByteInfo = { 2, 8, 0, 220, 234, 216, 216, 0 };
    //public byte[] privateServerByteInfo = { 2, 9, 0, 220, 234, 216, 216, 0 };

//Photon Session
//    UdpKit.Platform.Photon.PhotonSession photonSession = udpSession as UdpKit.Platform.Photon.PhotonSession;
//if (photonSession != null)
//{


//if (photonSession.Properties.ContainsKey("UserToken"))
//{
//    value_t = photonSession.Properties["UserToken"];
//}
//if (photonSession.Properties.ContainsKey("password"))
//{
//    value_t = photonSession.Properties["password"];
//}
//if (!value_t)
//{
//    var session = pair.Value;
//    GameObject serverEntryGO = Instantiate(sessionListObjectPrefab, serverList.transform, false);

//    ServerListRoomUI serverEntryUI = serverEntryGO.GetComponent<ServerListRoomUI>();
//    serverEntryUI.Populate(session, UnityEngine.Random.ColorHSV(), () =>
//    {
//        if (OnClickJoinSession != null) OnClickJoinSession.Invoke(session);
//    });
//}
//BoltLog.Info("Joining Session with custom data {0}/{1}", value_t, value_m);
*/
    //if (BoltNetwork.SessionList.Count > 0)
    //{
    //    GUILayout.BeginVertical();
    //    GUILayout.Space(30);

    //    foreach (var session in BoltNetwork.SessionList)
    //    {
    //        var photonSession = session.Value as UdpKit.Platform.Photon.PhotonSession;

    //        if (photonSession.Source == UdpSessionSource.Photon)
    //        {
    //            var matchName = photonSession.HostName;
    //            var label = string.Format("Join: {0} | {1}/{2}", matchName, photonSession.ConnectionsCurrent, photonSession.ConnectionsMax);

    //            if (ExpandButton(label))
    //            {
    //                Using the UserToken to send our credential information
    //                UserToken credentials = new UserToken();
    //                credentials.Username = "Bob";
    //                credentials.Password = "654321";

    //                BoltNetwork.Connect(photonSession, credentials);
    //                state = State.Started;
    //            }
    //        }
    //    }
    //}
    #endregion
}

public struct LobbyInfo
{
    public CSteamID lobbyID;
    public string lobbyName;
    public int playerCount;
}