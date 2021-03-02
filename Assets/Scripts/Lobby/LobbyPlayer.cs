using System.Collections;
using UnityEngine;
using Bolt;
using UnityEngine.UI;
using System;

public class LobbyPlayer : EntityEventListener<ILobbyPlayerInfoState>
{

    public BoltConnection connection;

    public InputField nameInput;
    public Button readyButton;
    public Button removePlayerButton;

    public Color NotReadyColor;
    public Color ReadyColor;

	public static LobbyPlayer localPlayer;


	public LobbyPlayer()
    {

    }

    public string playerName;
    public bool isReady;

    public override void Attached()
    {
        state.AddCallback("Name", () => nameInput.text = state.Name);
        state.AddCallback("Ready", callback: () => OnClientReady(state.Ready));

		//Is owner asks whether this machine created this player
        if(entity.IsOwner)
        {
            state.Name = string.Format("{0} #{1}", GenerateFullName(), UnityEngine.Random.Range(1, 100));
            state.Ready = isReady = false;
        }
    }

	public override void SimulateController()
	{
		// Update every 5 frames
		if (BoltNetwork.Frame % 5 != 0) return;

		var input = LobbyCommand.Create();
		input.Name = playerName;
		input.Ready = isReady;

		entity.QueueInput(input);
	}

	public override void ControlGained()
    {
        BoltLog.Info("ControlGained");

        //readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        SetupPlayer();
    }
    public override void OnEvent(LobbyPlayerKick evnt)
    {
		if (entity.IsOwner)
		{
			BoltNetwork.Destroy(this.gameObject);
		}
		BoltNetwork.Shutdown();
    }

    public void OnRemovePlayerClick()
    {
        if (BoltNetwork.IsServer)
        {
            LobbyPlayerKick.Create(entity, EntityTargets.OnlyController).Send();
        }
    }

	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (!entity.IsOwner) { return; }

		if (!resetState && command.IsFirstExecution)
		{
			LobbyCommand lobbyCommand = command as LobbyCommand;

			state.Name = lobbyCommand.Input.Name;
			state.Ready = lobbyCommand.Input.Ready;
		}
	}

	public void SetupPlayer()
    {
        BoltLog.Info("SetupPlayer");

        localPlayer = this;

        nameInput.interactable = true;

        removePlayerButton.gameObject.SetActive(false);
        removePlayerButton.interactable = false;

        //readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
        readyButton.interactable = true;

        //we switch from simple name display to name input
        nameInput.interactable = true;

        nameInput.onEndEdit.RemoveAllListeners();
        nameInput.onEndEdit.AddListener((text => { playerName = text; PlayerPrefs.SetString("username", text); }));

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClicked);

        OnClientReady(state.Ready);
    }

    public void SetupOtherPlayer()
    {
        BoltLog.Info("SetupOtherPlayer");
		nameInput.interactable = false;

        removePlayerButton.gameObject.SetActive(BoltNetwork.IsServer);
        removePlayerButton.interactable = BoltNetwork.IsServer;

        ChangeReadyButtonColor(NotReadyColor);

        readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
        readyButton.interactable = false;

        OnClientReady(state.Ready);
    }

	public void RemovePlayer()
	{
		if (entity && entity.IsAttached)
		{
			BoltNetwork.Destroy(gameObject);
		}
	}

	public override void Detached()
	{
		//            if (OnDetach != null) OnDetach.Invoke(this);
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

	public void OnClientReady(bool readyState)
	{
		if (readyState)
		{

			Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
			textComponent.text = "READY";
			//textComponent.color = ReadyColor;
			readyButton.interactable = false;
			nameInput.interactable = false;
		}
		else
		{
			ChangeReadyButtonColor(NotReadyColor);

			Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
			textComponent.text = entity.IsControlled ? "JOIN" : "...";
			//textComponent.color = Color.white;
			readyButton.interactable = entity.IsControlled;
			nameInput.interactable = entity.IsControlled;
		}
	}
}
