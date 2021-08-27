using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStates : NetworkBehaviour
{
    [Tooltip("Are we immobolised")] [SyncVar] public bool immobilize = false;
    [Tooltip("Have we been hit by the voodoo trap")] [SyncVar] public bool voodooPoisoned = false;
    [Tooltip("Are we unable to use abilities?")] [SyncVar] public bool mortality = false;
    [Tooltip("Have we recently been stolen from?")] [SyncVar] public bool hasBeenStolenFrom = false;

    [Tooltip("NA")] public GameObject nameTextPrefab;
    [Tooltip("NA")] public GameObject playerNameText;

    public PlayerMovement playerMovement;
    public PlayerCamera playerCamera;
    public PlayerToArtefactInteraction playerToArtefactInteraction;
    public PlayerToObstacleInteraction playerToObstacleInteraction;
    public PlayerToPlayerInteraction playerToPlayerInteraction;
    public PlayerToAbilityInteraction playerToAbilityInteraction;
    public Animator playerAnim;

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
            playerNameText.GetComponent<Text>().text = playerToArtefactInteraction.playerName;
        }
    }
    public override void OnStartAuthority()
    {
        SetLoadoutReleased(false);
        base.OnStartAuthority();
    }
    public void SetLoadoutReleased(bool value)
    {
        if (value == true)
        {
            Un_ImmobolizePlayer();
        }
        else
        {
            ImmobolizePlayer();
        }
        if (GameObject.Find("_wamp_water") && value == true)
        {
            GameObject.Find("_wamp_water").GetComponent<MeshCollider>().enabled = false;
        }
    }
    //Possibly need to make these functions commands

    public void ImmobolizePlayer()
    {
        playerMovement.enabled = false;
    }
    public void Un_ImmobolizePlayer()
    {
        playerMovement.enabled = true;
    }
    public void SetImmobolised(bool immobolise)
    {
        playerMovement.enabled = immobolise;
    }
    public void SetStolenFrom(bool stolen)
    {
        playerToPlayerInteraction.enabled = !stolen;
        SetImmobolised(stolen);
        if (stolen == true)
        {
            //Todo: Start timer for wear off period

        }
    }

    public void VoodooPlayer()
    {
        voodooPoisoned = true;
        playerMovement.voodooSpeed = -1;
    }
    public void Un_VoodooPlayer()
    {
        voodooPoisoned = false;
        playerMovement.speed = 1;
    }
}
