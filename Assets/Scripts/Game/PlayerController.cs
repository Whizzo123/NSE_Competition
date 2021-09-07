using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

/// <summary>
/// The player controller class provides input to the game player object. It interacts with many,
///  if not all systems, such as:
/// <list type="bullet">
///<listheader>
///    <item>Artefact</item><description> - Calls many pickup functions</description>
///    <item>Ability</item><description> - Know what abilities are available for the networked player.</description>
///    <item>SceneManager</item><description> - Interacts with the stash</description>
///</listheader>
///</list>
///<para>It is most likely the heaviest class. We may want to think about splitting up core features in the future in seperate scripts e.g. split the movement, the interactions, the spawning of camera.</para>
/// </summary>
public class PlayerController : NetworkBehaviour
{
    #region Variables
    [Header("Stored Interactables")]
    //Stored interactables
    [Tooltip("This is used for adding artefacts to the inventory temporarily while a Command is being sent to add artefacts to the real inventory. The reason for this was to allow us to check that we are not picking up the same artefact twice.")] public List<ArtefactBehaviour> tempArtefactStorage;
    [Tooltip("The artefacts that are in range for picking up")] public readonly SyncList<ArtefactBehaviour> targetedArtefacts = new SyncList<ArtefactBehaviour>();
    [Tooltip("Artefact netId's that have been marked for destruction, don't add back anywhere")]public List<uint> artefactsForDestruction = new List<uint>();
    [Tooltip("NA")] private Stash gameStash;
    [Tooltip("In devlopment: The ability pickups that are in range for picking up")] private AbilityPickup targetedAbilityPickup;
    //Loadout and inventory
    [Tooltip("Have we exited the loadout menu")] private bool loadoutReleased;
    [Tooltip("Our abilities that we've selected")] [SyncVar] public AbilityInventory abilityInventory;
    [SyncVar] private ArtefactInventory artefactInventory;
    [SyncVar] public string playerName;


    [Space]

    [Header("Player options")]
    //Tools
    [Tooltip("If the tools are currently on cooldown")] private bool toolWait = false;

    //Movement
    [Tooltip("The current speed of the player")] [SyncVar] public float speed = 10f;
    [Tooltip("The original speed of the player")] public float normalSpeed = 10f;


    //Sphere
    [Tooltip("Distance forward from the player for the destruction sphere")] public float lengthOfSphere = 2f;
    [Tooltip("Radius of the obstacle destruction sphere cast")] public float radiusOfSphere = 1f;
    [Space]

    [Header("LayerMasks and Components")]
    //Layermasks
    public LayerMask obstacles;
    [Space]
    //Player
    [Tooltip("Character controller reference")] public CharacterController playerCharacterController;//See attached()
    public Animator playerAnim;

    [Tooltip("NA")] public GameObject nameTextPrefab;
    [Tooltip("NA")] public GameObject playerNameText;

    [Tooltip("Camera attatched to the player, already in the world space")] public Camera playerCamera;
    [Tooltip("Virtual camera controlling the playerCamera, already in the world space")] public Cinemachine.CinemachineFreeLook vCam;
    [Space]


    [Tooltip("NA")] public Vector3 offset = new Vector3(0, 10, 10);

    [Header("States")]
    //Stuns
    [Tooltip("Are we immobolised")] [SyncVar] private bool immobilize;
    [Tooltip("Have we been hit by the voodoo trap")] [SyncVar] private bool voodooPoisoned;
    [Tooltip("Can we use abilities?")] [SyncVar] private bool mortal;
    [Tooltip("Can we use our tools?")][SyncVar] private bool paralyzed;

    //Other Variables
    [Tooltip("Have we recently been stolen from?")] [SyncVar] private bool hasBeenStolenFrom = false;

    [Space]
    [Header("DevMode")]
    [Tooltip("Devmode allows us to disconnect ourselves from the player")] public bool devMode;
    [Tooltip("Free look camera")] public GameObject devCam;
    #endregion

    #region SETUP_PLAYER
    public void Awake()
    {
        DontDestroyOnLoad(this);//Allows object to survive scene load(use until we know we can spawn after load or whenever we want
        playerCharacterController = this.gameObject.GetComponent<CharacterController>();
    }

    public override void OnStartAuthority()
    {
        // Player Setup start authority method
        GetComponent<PlayerSetup>().StartAuthority(this);
        // End Player Setup class start authority method
        base.OnStartAuthority();
        
    }
    // Player Setup - move whole method CmdSetupPlayer
    // End Player Setup - whole method
    // Player Camera class - just sitting in setup for now
    /// <summary>
    /// Attatches the normal camera and devcam. Todo: It is very messy right now, will need to clean up later
    /// </summary>
    
    //End player camera class - whole method
    #endregion

    /// <summary>
    /// Turns devcam on and off
    /// </summary>
    private void DevModeOn()
    {
        if (vCam != null)
            vCam.enabled = !devMode;
        playerCamera.enabled = !devMode;
        devCam.SetActive(devMode);

        FindObjectOfType<Canvas>().enabled = !devMode;
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; };

        #region DEVMODE
        //if (Input.GetKey(KeyCode.P)) { devMode = true;}
        //if (Input.GetKey(KeyCode.O)){devMode = false;}
        //DevModeOn();
        //if (devMode){return;}
        #endregion

        abilityInventory.Update();

        //PlayerSetup class - Update
        GetComponent<PlayerSetup>().UpdateSetup(this);
        //End PlayerSetup
        //Todo: Cleanup, maybe instead use loadoutReleased to return instead, this should help boost speed as
        //we can get rid of the pre-emptive code loading and reduce the amount of code that is predicted.
        if (loadoutReleased)
        {
            if (immobilize == false)
            {
                //Player Movement class
                #region MOVEMENT_AND_ANIMATION
                GetComponent<PlayerMovement>().UpdateMovement(this);
                #endregion
                //End PlayerMovement
            }
            else
                playerAnim.SetBool("moving", false);
        }

        //Todo:Remove ability pickups from here? Different Stash button as well? Thoughts for discussion
        #region ARTEFACT_INTERACTION
        if (Input.GetKeyDown(KeyCode.E))
        {
            // PlayerToArtefactInteraction class
            GetComponent<PlayerToArtefactInteraction>().InteractWithArtefact(this);
            //End PlayerToArtefactInteraction class
            // Player Ability Interaction class
            GetComponent<PlayerToAbilityInteraction>().InteractWithAbility(this);
            // End Player Ability Interaction class
            //Player Artefact Interaction
            //Game stash logic
            //End Player Artefact Interaction
        }
        #endregion

        #region STEALING
        if (Input.GetKeyDown(KeyCode.F) && !hasBeenStolenFrom)
        {
            //PlayerToPlayer class
            GetComponent<PlayerToPlayerInteraction>().Steal(this);
            //End PlayerToPlayer class
        }

        //PlayerToPlayer Class
        GetComponent<PlayerToPlayerInteraction>().UpdateSteal(this);
        //End PlayerToPlayer class
        #endregion

        #region OBSTACLE_INTERACTION
        if (Input.GetKey(KeyCode.Space) && toolWait == false && paralyzed == false)
        {
            //PlayerObstacleInteraction class
            GetComponent<PlayerToObstacleInteraction>().Cut(this);
            //End PlayerObstacleInteraction class
        }
        #endregion

        #region FOOTSTEP_SOUNDS
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && GetComponent<PlayerMovement>().isGrounded)
        {
            //Raycasts to our feet, grabs the layer below us and uses the string from the layer to play the sound
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, GetComponent<PlayerMovement>().ground))
            {
                string hitstring = hit.transform.gameObject.layer.ToString();//I believe this is unnecessary
                int layernumber = int.Parse(hitstring);//I believe this is unnescessary
                string lm = LayerMask.LayerToName(layernumber);
                //Todo: Re do this as it causes weird behaviour
                if (lm == "SwampGround")
                {
                    FindObjectOfType<AudioManager>().PlaySoundOnly(lm);
                }
                else
                {
                    FindObjectOfType<AudioManager>().StopSound("SwampGround");
                }
                FindObjectOfType<AudioManager>().PlaySoundOnly(lm);
            }
        }
        #endregion

        //PlayerMovement class
        //Call to CmdServerValidateHit inside if statement determined by falling velocity
        //End PlayerMovement class
    }

    /// <summary>
    /// Used to set whether we are able to move now or not, also disables the water collider so we can go through the water
    /// </summary>
    public void SetLoadoutReleased(bool value)
    {
        loadoutReleased = value;
        if (GameObject.Find("_wamp_water") && value == true)
        {
            GameObject.Find("_wamp_water").GetComponent<MeshCollider>().enabled = false;
        }
    }

    //PlayerMovement class
    //PlayerRotation
    // End PlayerMovement class

    // PlayerToPlayerInteraction class
    //Cmdhasbeenstolenfrom
    // End PlayerToPlayerINteraction class

    //PlayerToArtefactInteraction class DestroyGameObject & CmdDestroyGameObject
    //End PlayerToArtefactInteraction class

    // PlayerMovement class
    //Cmd and Rpc for moving player
    //End PlayerMovement class

    #region Collision
    /// <summary>
    /// Used for entering the stash and other players to allow for interaction and ui pop ups
    /// </summary>
    public void OnTriggerEnter(Collider collider)
    {
        //PlayerToArtefactInteraction class OnTriggerEnter method
        GetComponent<PlayerToArtefactInteraction>().TriggerEnterInteraction(this, collider);
        //End PlayerToArtefactInteraction
        //PlayerToAbilityInteraction class OnTriggerEnter method
        GetComponent<PlayerToAbilityInteraction>().TriggerEnterInteraction(this, collider);
        //End PlayerToAbilityInteraction class
        //PlayerToPlayer Interaction class OnTriggerEnter method
        GetComponent<PlayerToPlayerInteraction>().TriggerEnterInteraction(this, collider);
        //End PlayerToPlayerInteraction
    }

    /// <summary>
    /// Used for the targeted artefact as it doesn't work in OnTriggerEnter, as when the artefacts re-appear
    /// , they don't trigger the function.
    /// </summary>
    public void OnTriggerStay(Collider collider)
    {
        //PlayerToArtefactInteraction class OnTriggerStay
        GetComponent<PlayerToArtefactInteraction>().TriggerStayInteraction(this, collider);
        //End PlayerToArtefactInteraction class OnTriggerStay
    }

    /// <summary>
    /// Used to disable all interactions and disbable text interactions.
    /// </summary>
    public void OnTriggerExit(Collider collider)
    {
        
        if (collider != null)
        {
            //PlayerToArtefactInteraction class OnTriggerExit method
            GetComponent<PlayerToArtefactInteraction>().TriggerExitInteraction(this, collider);
            //End PlayerToArtefactInteraction class
            //PlayerToAbilityInteraction class OnTriggerExit method
            //Ability Pickup
            GetComponent<PlayerToAbilityInteraction>().TriggerExitInteraction(this, collider);
            //End PlayerToAbilityInteraction class OnTriggerExit method
            //PlayerToPlayerInteraction class OnTriggerExit method
            GetComponent<PlayerToPlayerInteraction>().TriggerExitInteraction(this, collider);
            //End PlayerToPlayerInteraction class OnTriggerExit method
        }
    }

    #endregion

    #region Obstacle Destruction
    //PlayerToObstacleInteraction class
    //Hit logic
    //End PlayerToObstacleInteraction class



    #endregion

    #region ARTEFACT_FUNCTIONS
    public ArtefactInventory GetArtefactInventory()
    {
        return artefactInventory;
    }
    //PlayerToArtefactInteraction class
    //TargetArtefacts commands
    //End PlayerToArtefactInteraction method

    [Command]
    public void CmdClearTargetArtefacts()
    {
        Debug.Log("Command is hit");
        GetComponent<PlayerController>().targetedArtefacts.Clear();
        Debug.Log("TargetedArtefact is causing issues");
    }

    public void SetArtefactInventory(ArtefactInventory inventory)
    {
        artefactInventory = inventory;
    }
    #endregion

    #region ABILITY_FUNCTIONS
    //Speed
    [Command(requiresAuthority = false)]
    public void CmdModifySpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    //Immobolise
    public bool IsImmobilized()
    {
        return immobilize;
    }
    [Command(requiresAuthority = false)]
    public void CmdSetImmobilized(bool value)
    {
        immobilize = value;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetParalyzed(bool value)
    {
        paralyzed = value;
    }

    //Poison
    public bool IsVoodooPoisoned()
    {
        return voodooPoisoned;
    }
    [Command(requiresAuthority = false)]
    public void CmdSetVoodooPoisoned(bool poisoned)
    {
        voodooPoisoned = poisoned;
    }

    //Mortal
    public bool IsMortal()
    {
        return mortal;
    }
    [Command(requiresAuthority = false)]
    public void CmdSetMortal(bool mortal)
    {
        this.mortal = mortal;
    }
    [Command]
    public void CmdToggleCamouflage(bool toggle, PlayerController player)
    {
        Debug.Log("CmdToggleCamouflage: local player: " + NetworkClient.localPlayer.GetComponent<PlayerController>().playerName);
        //GetPlayerToEmpower().ToggleMesh(toggle);
        RpcToggleCamouflage(toggle, player);
    }
    [ClientRpc]
    private void RpcToggleCamouflage(bool toggle, PlayerController player)
    {
        Debug.Log("ClientRpc call toggling camouflage for: " + player.playerName);
        if (NetworkClient.localPlayer.GetComponent<PlayerController>() != player)
        {
            Debug.Log("RpcToggleCamouflage the ClientRpc is hitting another player: " + NetworkClient.localPlayer.GetComponent<PlayerController>().playerName);
            player.ToggleMesh(toggle);
        }
        else
        {
            Debug.Log("RpcToggleCamouflage the ClientRpc is hitting client called: " + NetworkClient.localPlayer.GetComponent<PlayerController>().playerName);
            
        }
    }

    /// <summary>
    /// Toggles the mesh on and off for invisibility effect
    /// </summary>
    public void ToggleMesh(bool toggle)
    {
        //Player -> _scaleTest -> FULL.002
        transform.GetChild(0).transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = toggle;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * lengthOfSphere), radiusOfSphere);//Death sphere
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down);//Down 
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, -transform.up + transform.position);//Up
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(transform.position - new Vector3(0, 2, 0), groundDistance);
    }

    public void SetGameStash(Stash stash)
    {
        gameStash = stash;
    }

    public Stash GetGameStash()
    {
        return gameStash;
    }

    public void SetToolWait(bool toolWait)
    {
        this.toolWait = toolWait;
    }

    public bool HasPlayerBeenStolenFrom()
    {
        return hasBeenStolenFrom;
    }

    public void SetHasBeenStolenFrom(bool value)
    {
        hasBeenStolenFrom = value;
    }

    public AbilityPickup GetTargetedAbilityPickup()
    {
        return targetedAbilityPickup;
    }

    public void SetTargetedAbilityPickup(AbilityPickup pickup)
    {
        targetedAbilityPickup = pickup;
    }
}