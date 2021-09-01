using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;
using Mirror.Discovery;

/// <summary>
/// Controls the Browsing screen UI - displays the list of all available sessions and joining of them
/// //Todo: allows filtering of all sessions
/// </summary>
public class BrowseScreenUI : MonoBehaviour
{
    #region Variables
    [Header("Session lists")]
    [SerializeField] [Tooltip("Prefab used for instantiation for sessions, has ServerListRoomUI.cs")] public GameObject sessionListObjectPrefab;
    [SerializeField] [Tooltip("Prefab used for no lobbies")] public GameObject noServerFoundText;
    [SerializeField] [Tooltip("GameObject containing list of servers")] public GameObject serverList;
    

    public event Action<LobbyInfo> OnClickJoinSession;

    private LobbyUIManager lobbyUIManager;

    [Header("Refresh rate")]
    [SerializeField][Tooltip("Time to wait for refreshing")] private float waitTime = 5f;
    [Tooltip("Time elapsed since last refresh")]private float currentWaitTime = 0f;
    //Create a countdown action
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
    /// Updates the UI for any lobbies
    /// </summary>
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
        //Todo: Version Control, filtering
        //Instantiates all available lobbies
        foreach (LobbyInfo info in lobbies)
        {
            GameObject serverEntryGO = Instantiate(sessionListObjectPrefab, serverList.transform, false);

            ServerListRoomUI serverEntryUI = serverEntryGO.GetComponent<ServerListRoomUI>();
            serverEntryUI.Populate(info, UnityEngine.Random.ColorHSV());
        }
    }


    //Todo: Refresh the browse screen periodically or add a refresh button to show new lobbies
    void Update()
    { 
        //if (!FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
        //{
            //Periodically updates the details of the lobby
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
                //Loop through all servers and spawn them in the server list and populate the element with the server name and stuff
                foreach (long serverID in servers.Keys)
                {
                    GameObject serverEntryGO = Instantiate(sessionListObjectPrefab, serverList.transform, false);
                    ServerListRoomUI serverEntryUI = serverEntryGO.GetComponent<ServerListRoomUI>();
                    serverEntryUI.Populate(servers[serverID], UnityEngine.Random.ColorHSV());
                }
                currentWaitTime = waitTime;
            }
            else
            {
                currentWaitTime -= Time.deltaTime;
            }
        //}
        /*else
        {
            if(currentWaitTime <= 0)
            {
                SteamMatchmaking.RequestLobbyList();
            }
        }*/
    }


}

/// <summary>
/// Struct container for storing Steam Lobby data
/// </summary>
public struct LobbyInfo
{
    public CSteamID lobbyID;
    public string lobbyName;
    public int playerCount;
}