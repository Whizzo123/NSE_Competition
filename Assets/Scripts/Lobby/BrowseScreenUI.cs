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

        foreach (ServerListRoomUI child in serverList.transform.GetComponentsInChildren<ServerListRoomUI>())
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
                if (servers.Count == 0)
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


}

public struct LobbyInfo
{
    public CSteamID lobbyID;
    public string lobbyName;
    public int playerCount;
}