using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using Steamworks;

/// <summary>
/// Handles the UI Element for each player. Handles some networking functions such as readying up, initiating the start game, disconnecting players
/// </summary>
/// TODO:Rename from MirrorRoomPlayerLobby to something more appropriate. This script gets used by steam matchmaking
public class MirrorRoomPlayerLobby : NetworkBehaviour
{
    #region Variables
    [Header("UI")]
    [SerializeField] [Tooltip("The Lobby Screen")]private GameObject lobbyUI = null;
    [SerializeField] [Tooltip("Player name for all players")] private Text[] playerNameTexts = new Text[5];
    [SerializeField] [Tooltip("Ready texts for all players")] private Image[] playerReadyImageColor = new Image[5];
    
    [SerializeField] [Tooltip("Start button")] private Button startGameButton = null;
    [SerializeField] [Tooltip("Remove buttons")] private Button[] removeButtons = new Button[5];
    [SerializeField] [Tooltip("Back button")] public Button backButton;

    [SerializeField] private GameObject playerInfoPrefab;
    [SerializeField] private GameObject playerInfoContentBox;
    //[SerializeField] private List<Image> playerReadyObject;
    //[SerializeField] private Button removePlayerButton;
    //[SerializeField] private Text playerNameText;
    [Space]

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    private ulong steamId;

    [SerializeField] [Tooltip("Is this player the host?")] private bool isLeader;//Todo: isHost?

    private void Awake()
    {
        backButton.onClick.AddListener(() => FindObjectOfType<BackTemp>().Back());
        backButton.gameObject.GetComponentInChildren<Text>().text = "LEAVE";
        backButton.gameObject.GetComponentInChildren<Text>().text = "LEAVE";
        startGameButton.gameObject.GetComponentInChildren<Text>().text = "READY";
        startGameButton.gameObject.GetComponent<Button>().onClick.AddListener(CmdReadyUp);

    }

    /// <summary>
    /// Setter. Set start game button visible if isLeader
    /// </summary>
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
            if (value == true)
            {
                IsReady = !IsReady;
                backButton.gameObject.GetComponentInChildren<Text>().text = "CLOSE LOBBY";
                startGameButton.gameObject.GetComponentInChildren<Text>().text = "START";
                startGameButton.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                startGameButton.gameObject.GetComponent<Button>().onClick.AddListener(CmdStartGame);
            }
        }
    }

    void SetHostUI()
    {
        IsReady = true;
        backButton.gameObject.GetComponentInChildren<Text>().text = "CLOSE LOBBY";
        startGameButton.gameObject.GetComponentInChildren<Text>().text = "START";
        startGameButton.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        startGameButton.gameObject.GetComponent<Button>().onClick.AddListener(CmdStartGame);
    }


    [SerializeField] [Tooltip("Reference to MyNetworkManager for")] private MyNetworkManager room;

    /// <summary>
    /// Getter. Returns MyNetworkManager
    /// </summary>
    private MyNetworkManager Room
    {
        get
        {
            if(room != null) { return room; }
            return room = MyNetworkManager.singleton as MyNetworkManager;
        }
    }

    #endregion


    /// <summary>
    /// Sets a name for a steam user 
    /// </summary>
    public void SetSteamId(ulong steamId)
    {
        this.steamId = steamId;
        if (steamId == 0)
        {
            CmdSetDisplayName(string.Format("{0} #{1}", GenerateFullName(), UnityEngine.Random.Range(1, 100)));
        }
    }

    // THIS IS VERY MUCH REQUIRED CLIENT NAME WILL NOT APPEAR WITHOUT IT
    private void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    {
        CSteamID cSteamId = new CSteamID(newSteamId);
    }

    public override void OnStartAuthority()
    {
        Debug.Log("JOE: OnStartAuthority");
        //Uses steam name or generates a temporary name
        if (!FindObjectOfType<MyNetworkManager>().useSteamMatchmaking)
        {
            string name = string.Format("{0} #{1}", GenerateFullName(), UnityEngine.Random.Range(1, 100));
            CmdSetDisplayName(name);
        }
        else
            CmdSetDisplayName(SteamFriends.GetFriendPersonaName(new CSteamID(steamId)));

        lobbyUI.SetActive(true);
        //RemoveButtons enable or disable if leader
        //if (isLeader)
        //    EnableRemoveButtons();
        //else
        //    DisableRemoveButtons();
    }

    /// <summary>
    /// Adds player to MyNetworkManager and updating the UI
    /// </summary>
    public override void OnStartClient()
    {
        Debug.Log("JOE: OnStartClient");
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }
    /// <summary>
    /// Removes player to MyNetworkManager and updating the UI
    /// </summary>
    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    /// <summary>
    /// Updates every Player UI Element
    /// </summary>
    private void UpdateDisplay()
    {
        //Updates the Element if we have authority over it. Standard loop to allow authoritive actions
        if (!hasAuthority)
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

        //Loop through playerNameTexts playerReadyTexts and set them back to default
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].transform.parent.gameObject.SetActive(true);
            playerNameTexts[i].text = "Waiting For Player...";
            //playerReadyTexts[i].text = string.Empty;
            playerReadyImageColor[i] = playerNameTexts[i].GetComponentInParent<Image>();
        }
        //Loop through all players in room and set their respective DisplayName and ReadyStatus
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            Debug.Log("Room.RoomPlayers[i].DisplayName: " + Room.RoomPlayers[i].DisplayName);

            //Sets box color to green or red depending on if player has readied up.
            Color col;
            if (Room.RoomPlayers[i].IsReady)
            {
                 col.r = 0; col.g = 255; col.b = 0; col.a = 255;
            }
            else
            {
                col.r = 255; col.g = 0; col.b = 0; col.a = 255;
            }
            playerReadyImageColor[i].color = col;
        }
    }
    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
    private void SetPlayerPrefabInfo()
    {
        int roomPlayers = Room.RoomPlayers.Count;
    }

    #region REMOVE_BUTTONS
    /// <summary>
    /// Disables all remove buttons
    /// </summary>
    public void DisableRemoveButtons()
    {
        for (int i = 0; i < removeButtons.Length; i++)
        {
            removeButtons[i].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Turns on all kick player buttons and adds a listener to each button// Need to change it to buttons that are made on the fly as each player joins, their player button will be the kick button
    /// </summary>
    public void EnableRemoveButtons()
    {
        //Sets all but host kicks on
        removeButtons[0].gameObject.SetActive(false);
        for (int i = 1; i < removeButtons.Length; i++)
        {
            removeButtons[i].gameObject.SetActive(true);
        }
        //Todo: Use For Loop? in the one above?
        removeButtons[1].onClick.AddListener(() => KickPlayer(1));
        removeButtons[2].onClick.AddListener(() => KickPlayer(2));
        removeButtons[3].onClick.AddListener(() => KickPlayer(3));
        removeButtons[4].onClick.AddListener(() => KickPlayer(4));
    }

    /// <summary>
    /// Disconnects the specified player
    /// </summary>
    public void KickPlayer(int index)
    {
        if(Room.RoomPlayers[index] != null)
            Room.RoomPlayers[index].connectionToClient.Disconnect();
    }
    #endregion

    #region READY_BUTTONS
    /// <summary>
    /// Allows the Start game button to be interactable if host and 'readyToStart' is true
    /// </summary>
    public void HandleReadyToStart(bool readyToStart)
    {
        if(!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    //Why does host not have any authority over anything??????

    /// <summary>
    /// Run on server to check whether we are ready
    /// </summary>
    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;
        if (IsReady)
        {
            startGameButton.GetComponentInChildren<Text>().text = "Unready";
        }
        else
        {
            startGameButton.GetComponentInChildren<Text>().text = "Ready";
        }

        Room.NotifyPlayersofReadyState();
    }

    #endregion


    /// <summary>
    /// When everyone is ready, and start button has been clicked. This gets called and starts game.
    /// </summary>
    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }

    /// <summary>
    /// Generates a random name.
    /// </summary>
    private string GenerateFullName()
    {
        return string.Format("{0} {1}",
                GenerateName(new System.Random(DateTime.Now.Second - 1000).Next(4, 10)),
                GenerateName(new System.Random(DateTime.Now.Second + 1000).Next(4, 10))
        );
    }

    /// <summary>
    /// Generates a random name with length of 'len'
    /// </summary>
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
