using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using Steamworks;

public class MirrorRoomPlayerLobby : NetworkBehaviour
{

    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Text[] playerNameTexts = new Text[5];
    [SerializeField] private Text[] playerReadyTexts = new Text[5];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button[] removeButtons = new Button[5];

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    private ulong steamId;

    private bool isLeader;

    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private MyNetworkManager room;

    private MyNetworkManager Room
    {
        get
        {
            if(room != null) { return room; }
            return room = MyNetworkManager.singleton as MyNetworkManager;
        }
    }

    #region Server

    public void SetSteamId(ulong steamId)
    {
        this.steamId = steamId;
        if (steamId == 0)
        {
            CmdSetDisplayName(string.Format("{0} #{1}", GenerateFullName(), UnityEngine.Random.Range(1, 100)));
            PlayerPrefs.SetString("username", DisplayName);
        }
    }

    #endregion

    #region Client
    private void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    {
        CSteamID cSteamId = new CSteamID(newSteamId);

        CmdSetDisplayName(SteamFriends.GetFriendPersonaName(cSteamId));
        PlayerPrefs.SetString("username", DisplayName);
    }

    #endregion

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(string.Format("{0} #{1}", GenerateFullName(), UnityEngine.Random.Range(1, 100)));
        lobbyUI.SetActive(true);
        if (isLeader)
            EnableRemoveButtons();
        else
            DisableRemoveButtons();

    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if(!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if(player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }
        Debug.Log("Room Roomplayer count: " + Room.RoomPlayers.Count);
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
        }
    }

    public void EnableRemoveButtons()
    {
        removeButtons[0].gameObject.SetActive(false);
        for (int i = 1; i < removeButtons.Length; i++)
        {
            removeButtons[i].gameObject.SetActive(true);
        }
        removeButtons[1].onClick.AddListener(() => KickPlayer(1));
        removeButtons[2].onClick.AddListener(() => KickPlayer(2));
        removeButtons[3].onClick.AddListener(() => KickPlayer(3));
        removeButtons[4].onClick.AddListener(() => KickPlayer(4));
    }

    public void KickPlayer(int index)
    {
        if(Room.RoomPlayers[index] != null)
            Room.RoomPlayers[index].connectionToClient.Disconnect();
    }

    public void DisableRemoveButtons()
    {
        for (int i = 0; i < removeButtons.Length; i++)
        {
            removeButtons[i].gameObject.SetActive(false);
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if(!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    //Why does host not have any authority over anything??????

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersofReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if(Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }

    private string GenerateFullName()
    {
        return string.Format("{0} {1}",
                GenerateName(new System.Random(DateTime.Now.Second - 1000).Next(4, 10)),
                GenerateName(new System.Random(DateTime.Now.Second + 1000).Next(4, 10))
        );
    }

    private string GenerateName(int len)
    {
        var rand = new System.Random(DateTime.Now.Second);

        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        string name = "";

        name += consonants[rand.Next(consonants.Length)].ToUpper();
        name += vowels[rand.Next(vowels.Length)];

        var b = 2;
        while (b < len)
        {
            name += consonants[rand.Next(consonants.Length)];
            b++;
            name += vowels[rand.Next(vowels.Length)];
            b++;
        }

        return name;
    }
}
