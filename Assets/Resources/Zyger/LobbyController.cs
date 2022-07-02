using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Linq;
using Steamworks;

public class LobbyController : MonoBehaviour
{
    [SerializeField] public static LobbyController instance;

    public Text lobbyNameText;

    public GameObject playerListViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObject;

    public ulong currentLobbyID;
    public bool playerItemCreated = false;
    [SerializeField] private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    public PlayerObjectController localPlayerController;

    public GameObject startGameButton;
    public GameObject readyButton;
    public GameObject lobbyVisibility;
    public Text readyButtonText;

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ReadyPlayer()
    {
        localPlayerController.ChangeReady();
    }
    public void UpdateButton()
    {
        if (localPlayerController.ready)
        {
            readyButtonText.text = "Unready";

        }
        else
        {
            readyButtonText.text = "Ready";

        }
    }
    public void StartGame()
    {
        manager.StartGame();
    }
    public void LeaveLobby()
    {
        manager.StopClient();
    }
    public void CheckIfAllReady()
    {
        bool allReady = false;
        foreach (PlayerObjectController player in Manager.matchmakingPlayers)
        {
            if (player.ready)
            {
                allReady = true;
            }
            else
            {
                allReady = false;
                break;
            }
        }
        if (allReady)
        {
            if (localPlayerController.playerIDNumber == 1)
            {
                startGameButton.SetActive(true);
            }
            else
            {
                startGameButton.SetActive(false);
            }
        }
        else
        {
            startGameButton.SetActive(false);

            if (localPlayerController.playerIDNumber == 1)
            {
                lobbyVisibility.GetComponent<Dropdown>().interactable = true;
            }
            else
            {
                lobbyVisibility.GetComponent<Dropdown>().interactable = false;
            }
        }
    }



    public void UpdateLobbyName()
    {
        currentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyId;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "LobbyName");
    }

    public void UpdatePlayerList()
    {
        if (!playerItemCreated){ 
            CreateHostPlayerItem(); 
        }

        if (playerListItems.Count < Manager.matchmakingPlayers.Count) {CreateClientPlayerItem();}
        if (playerListItems.Count > Manager.matchmakingPlayers.Count) {RemovePlayerItem(); }
        if (playerListItems.Count == Manager.matchmakingPlayers.Count) { UpdatePlayerItem();}
    }
    public void FindLocalPlayer()
    {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();
    }
    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.matchmakingPlayers)
        {
            GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
            PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerItemScript.playerName = player.playerName;
            newPlayerItemScript.connectionID = player.connectionID;
            newPlayerItemScript.playerSteamId = player.playerSteamID;
            newPlayerItemScript.ready = player.ready;
            newPlayerItemScript.SetPlayerValues();


            newPlayerItem.transform.SetParent(playerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;

            playerListItems.Add(newPlayerItemScript);

        }
        playerItemCreated = true;
    }
    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.matchmakingPlayers)
        {
            //Are we already in the list? If we're not we are calling
            if (!playerListItems.Any(b => b.connectionID == player.connectionID))
            {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.connectionID = player.connectionID;
                newPlayerItemScript.playerSteamId = player.playerSteamID;
                newPlayerItemScript.ready = player.ready;
                newPlayerItemScript.SetPlayerValues();


                newPlayerItem.transform.SetParent(playerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerItemScript);
            }

        }

       // playerItemCreated = true;
    }
    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.matchmakingPlayers)
        {
            foreach (PlayerListItem playerListItemScript in playerListItems)
            {
                if (playerListItemScript.connectionID == player.connectionID)
                {
                    playerListItemScript.playerName = player.playerName;
                    playerListItemScript.ready = player.ready;
                    playerListItemScript.SetPlayerValues();

                    if (player == localPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }

        }
        CheckIfAllReady();
    }
    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerListItem in playerListItems)
        {
            //We only want to remove ourselves if we are already originally in the list
            if (!Manager.matchmakingPlayers.Any(b => b.connectionID == playerListItem.connectionID))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }
        if (playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItemRemoval in playerListItemsToRemove)
            {
                GameObject objectToRemove = playerListItemRemoval.gameObject;
                playerListItems.Remove(playerListItemRemoval);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

    public void SetLobbyType(int lobbyType)
    {
        switch (lobbyType)
        {
            case 0:
                SteamMatchmaking.SetLobbyType(new CSteamID(SteamLobby.instance.currentLobbyId), ELobbyType.k_ELobbyTypePublic);
                break;
            case 1:
                SteamMatchmaking.SetLobbyType(new CSteamID(SteamLobby.instance.currentLobbyId), ELobbyType.k_ELobbyTypePrivate);
                break;
            case 2:
                SteamMatchmaking.SetLobbyType(new CSteamID(SteamLobby.instance.currentLobbyId), ELobbyType.k_ELobbyTypeFriendsOnly);
                break;
            default:
                break;
        }

    }
}
