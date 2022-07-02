using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;

    public GameObject lobbiesMenu, matchmakingMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;
    public GameObject noServerText;


    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// Destroys all lobby prefabs in content
    /// </summary>
    public void DestroyLobbies()
    {
        foreach (GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }

    /// <summary>
    /// Displays either all lobbies found or a 'No Servers Found' tezt
    /// </summary>
    /// <param name="lobbyIDs"></param>
    /// <param name="result"></param>
    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        //No servers text
        if (lobbyIDs.Count == 0){noServerText.SetActive(true); return; }
        else { noServerText.SetActive(false); }

        //Display all servers and set information
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            //Sometimes we might get the wrong lobby id
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);

                createdItem.GetComponent<LobbyRoomEntry_Data>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;

                createdItem.GetComponent<LobbyRoomEntry_Data>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "LobbyName");

                createdItem.GetComponent<LobbyRoomEntry_Data>().SetLobbyData();

                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;

                listOfLobbies.Add(createdItem);
            }
        }

    }

    /// <summary>
    /// Called by button to switch screen to the lobby list display
    /// </summary>
    public void SwitchToLobbyListScreen()
    {
        matchmakingMenu.SetActive(false);
        lobbiesMenu.SetActive(true);

        RefreshLobbyList();
    }
    /// <summary>
    /// Called by button to switch screen to the Host/Browse screen
    /// </summary>
    public void SwitchToMatchmakingScreen()
    {
        matchmakingMenu.SetActive(true);
        lobbiesMenu.SetActive(false);
    }
    /// <summary>
    /// Called by button to refresh the lobby lists(This can be done by GetListOfLobbies() but the naming and function
    /// </summary>
    public void RefreshLobbyList()
    {
        SteamLobby.instance.GetLobbiesList();
    }
}
