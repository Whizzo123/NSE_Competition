using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerStates : NetworkBehaviour
{
    //[Tooltip("Are we immobolised")] [SyncVar] public bool immobilize = false;
    [Tooltip("Have we been hit by the voodoo trap")] [SyncVar] private bool voodooPoisoned = false;
    [Tooltip("Are we unable to use abilities?")] [SyncVar] public bool mortal = false;
    [Tooltip("Have we recently been stolen from?")] [SyncVar] private bool hasBeenStolenFrom = false;

    [Tooltip("NA")] private GameObject nameTextPrefab;
    [Tooltip("NA")] private GameObject playerNameText;
    [SyncVar] public string playerName;

    private PlayerMovement playerMovement;
    private PlayerCamera playerCamera;
    private PlayerToArtefactInteraction playerToArtefactInteraction;
    private PlayerToObstacleInteraction playerToObstacleInteraction;
    [SyncVar] private PlayerToPlayerInteraction playerToPlayerInteraction;
    private PlayerToAbilityInteraction playerToAbilityInteraction;
    private Animator playerAnim;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);//Allows object to survive scene load(use until we know we can spawn after load or whenever we want
    }
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        playerToArtefactInteraction = GetComponent<PlayerToArtefactInteraction>();
        playerToObstacleInteraction = GetComponent<PlayerToObstacleInteraction>();
        playerToPlayerInteraction = GetComponent<PlayerToPlayerInteraction>();
        playerToAbilityInteraction = GetComponent<PlayerToAbilityInteraction>();

        playerAnim = GetComponent<NetworkAnimator>().animator;

        if (playerNameText == null && SceneManager.GetActiveScene().name == "Plains")
        {
            playerNameText = Instantiate(Resources.Load<GameObject>("PlayerAssets/PlayerNameText_UI"));
            playerNameText.transform.SetParent(FindObjectOfType<CanvasUIManager>().playerTextContainer.transform);
            playerNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
            playerNameText.SetActive(true);
            playerNameText.GetComponent<Text>().text = playerName;
        }
    }
    public override void OnStartAuthority()
    {
        SetLoadoutReleased(false);
        base.OnStartAuthority();
    }
    public void SetLoadoutReleased(bool value)
    {
        playerToAbilityInteraction.SetImmobolised(value);
        if (GameObject.Find("_wamp_water") && value == true)
        {
            GameObject.Find("_wamp_water").GetComponent<MeshCollider>().enabled = false;
        }
    }

    public Animator GetAnimator()
    {
        return playerAnim;
    }
    public PlayerMovement GetPlayerMovement()
    {
        return playerMovement;
    }
    public PlayerToAbilityInteraction GetPlayerToAbilityInteraction()
    {
        return playerToAbilityInteraction;
    }
}
