﻿using System.Collections;
using UnityEngine;
using Bolt;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;


public class LobbyPlayer : NetworkBehaviour
{
	#region Variables
	[SyncVar(hook = nameof(HandleSteamIdUpdated))]
	private ulong steamId;

	public Text name;
    public Button readyButton;
    public Button removePlayerButton;

    public Color NotReadyColor;
    public Color ReadyColor;

	public static LobbyPlayer localPlayer;

	public string playerName;
	public bool isReady;
	#endregion

	public LobbyPlayer()
    {

    }

	//Can sync the UI by using commands but only finds that player object if connection is assigned so we need to assign that connection in the creating of the LobbyPlayer which in that 
	//case will need to be shifted to the NetworkManager script

	#region Server

	public void SetSteamId(ulong steamId)
    {
		this.steamId = steamId;
		if (steamId == 0)
		{
			name.text = string.Format("{0} #{1}", GenerateFullName(), UnityEngine.Random.Range(1, 100));
			PlayerPrefs.SetString("username", name.text);
		}
    }

	#endregion

	#region Client
	private void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId)
	{
		CSteamID cSteamId = new CSteamID(newSteamId);

		name.text = SteamFriends.GetFriendPersonaName(cSteamId);
		PlayerPrefs.SetString("username", name.text);
	}

    #endregion

    public override void OnStartAuthority()
    {
		CmdSetTransform();
	}

	[Command]
	public void CmdSetTransform()
    {
		this.transform.SetParent(GameObject.Find("PlayersList").transform);
		GameObject.Find("RoomCanvas").AddComponent<NetworkTransformChild>().target = FindObjectOfType<RoomScreenUI>().playersList.transform;
	}

	/// <summary>
	/// Called after entity is attached in the network like the unity Start function
	/// </summary>
	/*public override void Attached()
    {
        state.AddCallback("Name", () => nameInput.text = state.Name);
        state.AddCallback("Ready", callback: () => OnClientReady(state.Ready));

		//Is owner asks whether this machine created this player
        if(entity.IsOwner)
        {
            state.Name = string.Format("{0} #{1}", GenerateFullName(), UnityEngine.Random.Range(1, 100));
            state.Ready = isReady = false;
        }
    }*/

	/// <summary>
	/// Like update function only run on the controller computer
	/// </summary>
	/*public override void SimulateController()
	{
		// Update every 5 frames
		if (BoltNetwork.Frame % 5 != 0) return;

		var input = LobbyCommand.Create();
		input.Name = playerName;
		input.Ready = isReady;

		entity.QueueInput(input);
	}*/

	/// <summary>
	/// Called when either TakeControl() or AssignControl() is used on an entity
	/// </summary>
	/*public override void ControlGained()
    {
        BoltLog.Info("ControlGained");

        //readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        SetupPlayer();
    }
    public override void OnEvent(LobbyPlayerKick evnt)
    {
		BoltLog.Info("Kick");
		foreach (var entity in BoltNetwork.Entities)
		{
			if (entity.IsOwner)
			{
				Debug.LogError("OWNER OF: " + entity.gameObject.name + " | HAS CONTROL: " + entity.HasControl + " | NETWORK ID : " + entity.NetworkId);//entity.TakeControl);
			}
			else
			{
				Debug.LogError("NOT OWNER OF: " + entity.gameObject.name + " | HAS CONTROL: " + entity.HasControl + " | NETWORK ID : " + entity.NetworkId);
			}
		}
		BoltNetwork.Destroy(this.gameObject);
		BoltNetwork.Shutdown();

	}*/

	public void OnRemovePlayerClick()
    {
        /*if (BoltNetwork.IsServer && !FindObjectOfType<LobbyUIManager>().isCountdown)
        {
            LobbyPlayerKick.Create(entity, EntityTargets.OnlyController).Send();
        }*/
    }

	/*public override void ExecuteCommand(Command command, bool resetState)
	{
		if (!entity.IsOwner) { return; }

		if (!resetState && command.IsFirstExecution)
		{
			LobbyCommand lobbyCommand = command as LobbyCommand;

			state.Name = lobbyCommand.Input.Name;
			state.Ready = lobbyCommand.Input.Ready;
		}
	}*/

	[Command]
	private void CmdSetPlayerDisplayName(string displayName)
    {

    }

	/// <summary>
	/// Sets up player on the client computer
	/// </summary>
	public void SetupPlayer()
    {
        BoltLog.Info("SetupPlayer");

        localPlayer = this;

        //nameInput.interactable = true;

        removePlayerButton.gameObject.SetActive(false);
        removePlayerButton.interactable = false;

        //readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
        readyButton.interactable = true;

        //we switch from simple name display to name input
        //nameInput.interactable = true;

        //nameInput.onEndEdit.RemoveAllListeners();
        //nameInput.onEndEdit.AddListener((text => { playerName = text; PlayerPrefs.SetString("username", text); }));

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClicked);

        //OnClientReady(state.Ready);
    }
	/// <summary>
	/// Sets up player on the server computer
	/// </summary>
    public void SetupOtherPlayer()
    {
        //BoltLog.Info("SetupOtherPlayer");
		//nameInput.interactable = false;

        removePlayerButton.gameObject.SetActive(BoltNetwork.IsServer);
        removePlayerButton.interactable = BoltNetwork.IsServer;

        ChangeReadyButtonColor(NotReadyColor);

        readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
        readyButton.interactable = false;

        //OnClientReady(state.Ready);
    }

	/// <summary>
	/// Called when player is removed from lobby
	/// </summary>
	public void RemovePlayer()
	{
		/*if (entity && entity.IsAttached)
		{
			BoltNetwork.Destroy(gameObject);
		}*/
	}

	// Utils

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

	// UI

	void ChangeReadyButtonColor(Color c)
	{
		ColorBlock b = readyButton.colors;
		b.normalColor = c;
		b.pressedColor = c;
		b.highlightedColor = c;
		b.disabledColor = c;
		readyButton.colors = b;
	}

	public void OnReadyClicked()
	{
		isReady = !isReady;
	}

	/// <summary>
	/// Check to see if player is ready and change lobbyplayer uiElement accordingly
	/// </summary>
	/// <param name="readyState"></param>
	public void OnClientReady(bool readyState)
	{
		if (readyState)
		{

			Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
			textComponent.text = "READY";
			//textComponent.color = ReadyColor;
			readyButton.interactable = false;
			//nameInput.interactable = false;
		}
		else
		{
			ChangeReadyButtonColor(NotReadyColor);

			Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
			//textComponent.text = entity.IsControlled ? "JOIN" : "...";
			//textComponent.color = Color.white;
			//readyButton.interactable = entity.IsControlled;
			//nameInput.interactable = entity.IsControlled;
		}
	}

}
