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
    [Tooltip("The artefacts that are in range for picking up")] readonly SyncList<ArtefactBehaviour> targetedArtefacts = new SyncList<ArtefactBehaviour>();
    [Tooltip("Artefact netId's that have been marked for destruction, don't add back anywhere")]private List<uint> artefactsForDestruction = new List<uint>();
    [Tooltip("NA")] private Stash gameStash;
    [Tooltip("The player that is currently targeted to steal artefacts from")] private PlayerController targetedPlayerToStealFrom;
    [Tooltip("In devlopment: The ability pickups that are in range for picking up")] private AbilityPickup targetedAbilityPickup;
    //Loadout and inventory
    [Tooltip("Have we exited the loadout menu")] private bool loadoutReleased;
    [Tooltip("Our abilities that we've selected")] [SyncVar] public AbilityInventory abilityInventory;
    [SyncVar] private ArtefactInventory artefactInventory;
    [SyncVar] public string playerName;


    [Space]

    [Header("Player options")]
    //Tools
    [SerializeField] [Tooltip("Time delay before destroying another obstacle")] [Range(0, 1)] private float waitTime = 0.05f;
    [Tooltip("If the tools are currently on cooldown")] private bool toolWait = false;
    //Gravity
    [SerializeField] private float playerGravity = -65;
    [SerializeField] [Tooltip("The distance from the player to the ground to check if they're grounded")] private float groundDistance = 2.5f;
    [Tooltip("Is the player touching the ground")] private bool isGrounded = true;
    [SerializeField] [Tooltip("How fast the player is currently falling by y axis")] private Vector3 playerFallingVelocity;
    //Movement
    [Tooltip("The current speed of the player")] [SyncVar] public float speed = 10f;
    [Tooltip("The original speed of the player")] public float normalSpeed = 10f;
    [Tooltip("Direction player is moving in by input, not physics")] private Vector3 direction;
    [Tooltip("Direction of player movement, by input and physics")] private Vector3 playerMovement = Vector3.zero;

    //Sphere
    [Tooltip("Distance forward from the player for the destruction sphere")] public float lengthOfSphere = 2f;
    [Tooltip("Radius of the obstacle destruction sphere cast")] public float radiusOfSphere = 1f;
    [Space]

    [Header("LayerMasks and Components")]
    //Layermasks
    public LayerMask obstacles;
    public LayerMask ground;
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
    [Tooltip("NA")] private float currentStunAfterTimer;
    [Tooltip("Time player is stunned after being stolen from")] public float timeForStunAfterSteal = 10.0f;

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
        //Attatches Camera
        vCam = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        if (vCam != null)
        {
            Invoke("SetCamera", 1);
        }
        else
        {
            Invoke("SetCamera", 5);
        }

        //Setup player components and immobolise player
        CmdSetupPlayer();
        SetLoadoutReleased(false);
        base.OnStartAuthority();
    }
    /// <summary>
    /// Resets some variables and sets up some components
    /// </summary>
    [Command]
    private void CmdSetupPlayer()
    {
        //Components
        abilityInventory = new AbilityInventory(this);
        artefactInventory = GetComponent<ArtefactInventory>();

        //Variables
        immobilize = false;
        hasBeenStolenFrom = false;
    }
    /// <summary>
    /// Attatches the normal camera and devcam. Todo: It is very messy right now, will need to clean up later
    /// </summary>
    [Client]
    void SetCamera()
    {
        //Currently what it's trying to do is find cameras, if it can't find cameras, it will instantiate cameras
        //It also modifies so many things. It is simply a mess that needs to be fixed, it's not worth commenting here
        vCam = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        //DontDestroyOnLoad(vCam);
        vCam.LookAt = this.gameObject.transform;
        vCam.Follow = this.gameObject.transform;
        //vCam.transform.rotation = Quaternion.Euler(45, 0, 0);
        playerCamera = Camera.main;
        if (!hasAuthority)
        {
            //Disable other players cameras so that we don't accidentally get assigned to another players camera
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
        }
        if (devCam == null)
        {
            //REMINDER, you can't find the object if they are not in the same section ie dontdestroysection
            devCam = GameObject.Find("DevCam");
            Debug.LogError(devCam);
            devCam.SetActive(false);
        }
        else
        {
            devCam = Instantiate(devCam);
            devCam.SetActive(false);
        }
    }
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

        //Sets up player name for scoreboard use and floating name use
        if (playerNameText == null && SceneManager.GetActiveScene().name == "GameScene")
        {
            playerNameText = Instantiate(Resources.Load<GameObject>("PlayerAssets/PlayerNameText_UI"));
            playerNameText.transform.SetParent(FindObjectOfType<CanvasUIManager>().playerTextContainer.transform);
            playerNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
            playerNameText.SetActive(true);
            playerNameText.GetComponent<Text>().text = playerName;
        }
        //Todo: Cleanup, maybe instead use loadoutReleased to return instead, this should help boost speed as
        //we can get rid of the pre-emptive code loading and reduce the amount of code that is predicted.
        if (loadoutReleased)
        {


            if (immobilize == false)
            {


                #region MOVEMENT_AND_ANIMATION
                if (isGrounded)
                {
                    playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
                    playerMovement = playerCamera.transform.TransformDirection(playerMovement);//Allows player to move along camera rotation axis
                    if (Input.GetAxisRaw("Vertical") < -0.5)//To stop backwards going up
                    {
                        playerMovement.y = 0;
                    }
                }



                //Animations, with movement checks
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                {
                    direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

                    playerAnim.SetBool("moving", true);

                    //Voodoo poison
                    if (voodooPoisoned)
                    {
                        playerMovement = new Vector3(playerMovement.x * -1, playerMovement.y, playerMovement.z * -1);
                        direction *= -1;
                    }
                }
                else
                {
                    playerAnim.SetBool("moving", false);
                }

                #region FALLING

                //Projects a sphere underneath player to check ground layer
                isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 2, 0), groundDistance, ground);

                //Player recieves a constant y velocity from gravity
                playerFallingVelocity.y += playerGravity;// * Time.deltaTime;

                //If player is fully grounded then apply some velocity down, this will change the 'floating' period before plummeting.
                if (isGrounded && playerFallingVelocity.y < 0)
                {
                    playerFallingVelocity.y = -5f;
                }

                #endregion

                playerCharacterController.Move(playerMovement * speed * Time.deltaTime);
                playerCharacterController.Move(playerFallingVelocity * Time.deltaTime);
                PlayerRotation();
                
                #endregion
            }
            else
                playerAnim.SetBool("moving", false);
        }

        //Todo:Remove ability pickups from here? Different Stash button as well? Thoughts for discussion
        #region ARTEFACT_INTERACTION
        if (Input.GetKeyDown(KeyCode.E))
        {
            //If we have artefacts in range
            if (targetedArtefacts.Count != 0 && tempArtefactStorage.Count != 0)
            {
                //If we have an empty slot
                if (artefactInventory.GetInventoryCount() <= 7)
                {
                    Debug.Log("Picking up Artefacts");
                    // All artefacts that are in our range get added to our inventory and gameobject destroyed
                    foreach (ArtefactBehaviour item in targetedArtefacts)
                    {
                        Debug.Log("Looping now ");
                        artefactInventory.AddToInventory(item.GetArtefactName(), item.GetPoints());
                        FindObjectOfType<AudioManager>().PlaySound(item.GetRarity().ToString());
                        DestroyGameObject(item.gameObject);
                        artefactsForDestruction.Add(item.GetComponent<NetworkIdentity>().netId);
                        
                    }
                    CmdClearTargetArtefacts();
                    tempArtefactStorage.Clear();
                    
                    if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                        FindObjectOfType<CanvasUIManager>().CloseHintMessage();
                }
                else
                {
                    FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot pickup artefact inventory is full (Max: 8 artefacts)");
                }
            }
            else if (targetedAbilityPickup != null)
            {
                targetedAbilityPickup.PickupAbility(this);
                targetedAbilityPickup = null;
            }
            else if (gameStash != null && artefactInventory.InventoryNotEmpty())
            {
                //Todo: For consistancy, instead of clearing the artefact inventory elsewhere, let's clear it here
                gameStash.CmdAddToStashScores(this);
                tempArtefactStorage.Clear();
                artefactsForDestruction.Clear();
                CmdClearTargetArtefacts();
                FindObjectOfType<AudioManager>().PlaySound("Stash");
            }
            else if (gameStash != null && !artefactInventory.InventoryNotEmpty())
            {
                FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot deposit no artefacts in inventory");
            }
        }
        #endregion

        #region STEALING
        if (Input.GetKeyDown(KeyCode.F) && !hasBeenStolenFrom)
        {
            Debug.LogError(artefactInventory.GetAllArtefactNames());

            //If we are not full, they are no longer stunned and have artefacts, we steal
            if (targetedPlayerToStealFrom != null)
            {
                if (artefactInventory.AvailableInventorySlot() && targetedPlayerToStealFrom.GrabArtefactInventory().InventoryNotEmpty() && targetedPlayerToStealFrom.hasBeenStolenFrom == false)
                {

                    //Add to our inventory
                    ItemArtefact randomArtefact = targetedPlayerToStealFrom.GetArtefactInventory().GrabRandomItem();
                    artefactInventory.AddToInventory(randomArtefact.name, randomArtefact.points);

                    //remove from enemy inventory
                    for (int indexToRemove = 0; indexToRemove < targetedPlayerToStealFrom.GetArtefactInventory().GetInventory().Count; indexToRemove++)
                    {
                        if (targetedPlayerToStealFrom.GetArtefactInventory().GetInventory()[indexToRemove].name != string.Empty && targetedPlayerToStealFrom.GetArtefactInventory().GetInventory()[indexToRemove].name == randomArtefact.name)
                        {
                            targetedPlayerToStealFrom.GetArtefactInventory().RemoveFromInventory(indexToRemove, randomArtefact.name, randomArtefact.points);
                            targetedPlayerToStealFrom.CmdSetImmobilized(true);
                            targetedPlayerToStealFrom.CmdSetHasBeenStolenFrom(true);
                            break;
                        }
                    }
                }
                else
                {
                    FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot steal from player has no artefacts or stolen from recently");

                }
            }
        }

        //Stunning timer from being stolen from
        if (hasBeenStolenFrom)
        {
            if (currentStunAfterTimer >= timeForStunAfterSteal)
            {
                currentStunAfterTimer = 0;
                CmdSetHasBeenStolenFrom(false);
                CmdSetImmobilized(false);
            }
            else
            {
                currentStunAfterTimer += Time.deltaTime;
            }
        }
        #endregion

        #region OBSTACLE_INTERACTION
        if (Input.GetKey(KeyCode.Space) && toolWait == false)//&& state.Paralyzed == false)
        {
            playerAnim.SetTrigger("Cut");
            StartCoroutine(Hit());
        }
        #endregion

        #region FOOTSTEP_SOUNDS
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && isGrounded)
        {
            //Raycasts to our feet, grabs the layer below us and uses the string from the layer to play the sound
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, ground))
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

        //Todo: Find a different way to stop players from climbing obstacles as this is unreliable
        if (playerFallingVelocity.y < -200)
        {
            CmdServerValidateHit();
        }
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

    /// <summary>
    /// Rotates player according to slope and movement direction.
    /// If we wanted the model to remain upright, we can attach this to a child that isn't visible, but has the same parameters as the player(height etc)
    /// <para>Primarily used for the purpose of aiming the forward ray when on slopes</para>
    /// </summary>
    [ClientCallback]
    void PlayerRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, ground))
        {


            //Quaternion finalQuat;
            //Vector3 axisOfRotation = (Vector3.Cross(hit.normal, Vector3.up)).normalized;//gets the axis to rotate on a slope.
            //float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(for somereason it's negative either way round) (dot product)
            //Quaternion slopeQuat = Quaternion.identity;// = Quaternion.AngleAxis(-rotationAngle, axisOfRotation);//Quaternion of the rotation necessary to angle the player on a slope

            //Quaternion lookQuat = Quaternion.LookRotation(direction, Vector3.up);//Quaternion of the direction of player movement

            //if (rotationAngle < 45)
            //{
            //    slopeQuat = Quaternion.AngleAxis(-rotationAngle, axisOfRotation);//Quaternion of the rotation necessary to angle the player on a slope

            //    //finalQuat = slopeQuat.normalized;// * lookQuat; //Quaternion rotation of look rotation and slope rotation
            //    //transform.rotation = finalQuat;
            //}
            if (playerMovement.x != 0 && playerMovement.z != 0)
            {
                float ta = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg;// + cam.transform.eulerAngles.y;
                Quaternion rot = Quaternion.Euler(0f, ta, 0f);// * slopeQuat.normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);

            }
        }

        if (Input.GetMouseButton(1))
        {
            vCam.m_XAxis.m_MaxSpeed = 200;

        }
        else
        {
            vCam.m_XAxis.m_MaxSpeed = 0;
        }
        if (Input.GetMouseButton(2))
        {
            vCam.m_YAxis.m_MaxSpeed = 20;//Adjustable sensitivity
        }else
        {
            vCam.m_YAxis.m_MaxSpeed = 0;
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdSetHasBeenStolenFrom(bool value)
    {
        hasBeenStolenFrom = value;
    }

    /// <summary>
    /// Calls CmdDestroyGameObject.
    /// </summary>
    [ClientCallback]
    public void DestroyGameObject(GameObject go)
    {
        CmdDestroyGameObject(go);
    }
    /// <summary>
    /// Destroys networked GameObjects.
    /// <para>Call DestroyGameObject(GameObject go) instead to destroy on all instances.</para>
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdDestroyGameObject(GameObject go)
    {
        NetworkServer.Destroy(go);
    }
    [Command(requiresAuthority = false)]
    public void CmdMovePlayer(Vector3 position, string playerName)
    {
        transform.position = position;
        RpcMovePlayer(position, playerName);
    }

    [ClientRpc]
    public void RpcMovePlayer(Vector3 position, string playerName)
    {
        if(this.playerName == playerName)
        {
            transform.position = position;
        }
    }

    #region Collision
    /// <summary>
    /// Used for entering the stash and other players to allow for interaction and ui pop ups
    /// </summary>
    public void OnTriggerEnter(Collider collider)
    {
        //Allows us to interact with the gamestash and shows hint message
        if (collider.gameObject.GetComponent<Stash>())
        {
            gameStash = collider.gameObject.GetComponent<Stash>();
            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Deposit");
        }
        else if (collider.gameObject.GetComponent<AbilityPickup>())
        {
            targetedAbilityPickup = collider.gameObject.GetComponent<AbilityPickup>();
        }
        //Allows us to interact with A player and shows hint message
        if (collider.gameObject.GetComponent<PlayerController>())
        {
            //if (collider.GetComponentInChildren<SkinnedMeshRenderer>().material.name.Contains("Fade"))
            //{
                //return;
            //}
            targetedPlayerToStealFrom = collider.gameObject.GetComponent<PlayerController>();
            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
            {
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press F to Steal");

            }

        }

    }

    /// <summary>
    /// Used for the targeted artefact as it doesn't work in OnTriggerEnter, as when the artefacts re-appear
    /// , they don't trigger the function.
    /// </summary>
    public void OnTriggerStay(Collider collider)
    {

        if (!collider.gameObject.GetComponent<ArtefactBehaviour>())
        {
            return;
        }
        ArtefactBehaviour artefactBehaviour = collider.gameObject.GetComponent<ArtefactBehaviour>();
        if (artefactsForDestruction.Contains(artefactBehaviour.netId))
        {
            return;
        }
        //If it is available for pickup and it currently isn't in tempartefactstorage
        if (artefactBehaviour &&
            tempArtefactStorage.Contains(artefactBehaviour) == false && targetedArtefacts.Contains(artefactBehaviour) == false &&
            artefactBehaviour.IsAvaliableForPickup() && 
            targetedArtefacts.Count <= 4)
        {
            //Adds it temporarily
            tempArtefactStorage.Add(artefactBehaviour);
            //Sends command to add it to targeted artefact
            CmdAddToTargetedArtefacts(artefactBehaviour);

            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Pickup");
        }
    }

    /// <summary>
    /// Used to disable all interactions and disbable text interactions.
    /// </summary>
    public void OnTriggerExit(Collider collider)
    {
        if (collider != null)
        {
            //Artefacts
            if (targetedArtefacts.Count != 0 && collider.gameObject.GetComponent<ArtefactBehaviour>())
            {
                //Removes specific artefact that we exited.
                int i = 0;
                foreach (ArtefactBehaviour item in targetedArtefacts)
                {
                    if (item.GetInstanceID() == collider.gameObject.GetComponent<ArtefactBehaviour>().GetInstanceID())
                    {
                        tempArtefactStorage.Remove(item);
                        CmdTargetArtefactsRemoveAt(item);

                    }
                    i++;
                }
                if (targetedArtefacts.Count == 0)
                {
                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
                }
            }
            //Game Stash
            else if (gameStash != null && collider.gameObject == gameStash.gameObject)
            {
                gameStash = null;
                if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
            }
            //Ability Pickup
            else if (targetedAbilityPickup != null && collider.gameObject == targetedAbilityPickup.gameObject)
            {
                targetedAbilityPickup = null;
            }
            //Players
            if (targetedPlayerToStealFrom != null && collider.gameObject == targetedPlayerToStealFrom.gameObject)
            {
                targetedPlayerToStealFrom = null;
                if (NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
            }
        }
    }

    #endregion

    #region Obstacle Destruction

    /// <summary>
    /// Gives a delay to the destroying obstacles function 'HitForward()'.
    /// </summary>
    /// <returns></returns>
    [ClientCallback]
    System.Collections.IEnumerator Hit()
    {
        toolWait = true;
        CmdHitForward();
        yield return new WaitForSeconds(waitTime);
        toolWait = false;
    }

    [Command]
    void CmdHitForward()
    {
        HitForward();
    }
    /// <summary>
    /// Destroys obstacles directly in front of player.
    /// </summary>
    [ClientRpc]
    void HitForward()
    {
        FindObjectOfType<AudioManager>().PlaySound("Cut");

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hit;

        //Sends a sphere in front to hit objects
        hit = Physics.SphereCastAll(ray, radiusOfSphere, lengthOfSphere, obstacles);

        foreach (RaycastHit item in hit)
        {
            if (item.transform.GetComponent<ArtefactBehaviour>())
            {
                item.transform.gameObject.GetComponent<ArtefactBehaviour>().EnableForPickup();
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else if (item.transform.GetComponent<AbilityPickup>())
            {
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                item.transform.GetComponent<AbilityPickup>().CmdSetEnabledForPickup(true);
            }
            else
            {
                Destroy(item.transform.gameObject);
            }
        }

    }

    /// <summary>
    /// Used to destroy the obstacles underneath player if player has accidentaly went on top of obstacles
    /// </summary>
    [Command]
    private void CmdServerValidateHit()
    {
        //Validate the logic//Just a reminder that everything right now is pretty much client authorative. Cheating is easily possible.

        RpcHitDown();
    }
    /// <summary>
    /// Destroy obstacles underneath player, still does normal hit behaviour for interactables
    /// </summary>
    [ClientRpc]
    private void RpcHitDown()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hit;
        hit = Physics.SphereCastAll(ray, radiusOfSphere, lengthOfSphere, obstacles);
        foreach (RaycastHit item in hit)
        {
            if (item.transform.GetComponent<ArtefactBehaviour>())
            {
                item.transform.gameObject.GetComponent<ArtefactBehaviour>().EnableForPickup();
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else if (item.transform.GetComponent<AbilityPickup>())
            {
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                item.transform.GetComponent<AbilityPickup>().CmdSetEnabledForPickup(true);
            }
            else
            {
                Destroy(item.transform.gameObject);
            }
        }
    }



    #endregion

    #region ARTEFACT_FUNCTIONS
    public ArtefactInventory GetArtefactInventory()
    {
        return artefactInventory;
    }
    //Todo:Remove this, we already have GetArtefactInventory, line 318 uses this
    ArtefactInventory GrabArtefactInventory()
    {
        return artefactInventory;
    }

    [Command]
    private void CmdClearTargetArtefacts()
    {
        Debug.Log("Command is hit");
        targetedArtefacts.Clear();
        Debug.Log("TargetedArtefact is causing issues");
    }
    [Command]
    private void CmdAddToTargetedArtefacts(ArtefactBehaviour artefact)
    {
        targetedArtefacts.Add(artefact);
    }
    [Command]
    private void CmdTargetArtefactsRemoveAt(ArtefactBehaviour artefact)
    {
        targetedArtefacts.Remove(artefact);
    }
    [Command]
    private void CmdTargetArtefactsRemoveAtI(int i)
    {
        targetedArtefacts.RemoveAt(i);
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
}




#region deadCode

          //Old camera code
            /* if (Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
             {
                 if (lastMousePos != Input.mousePosition)
                 {
                     float mouseMoveDistance = lastMousePos.x - Input.mousePosition.x;
                     if (inverted)
                         mouseMoveDistance *= -1;
                     this.transform.Rotate(new Vector3(0, mouseMoveDistance * rotationSpeed * Time.deltaTime, 0));
                 }

                 lastMousePos = Input.mousePosition;
             }
*/
//////Remember, this was all called from Update() A [ClientCallback], also remember we testing player position. This is updated from the transform, not necessarily the function
//No tags, if we do something, everyone else sees that
//[Client] If we do somethingg, everyone else sees that
//[ClientRpc] Host can call, everyone sees. Host called RocketMan(), host and client saw host do RocketMan. Client tried RocketMan, didn't do anything for host and client.
//[Command] Anyone can call, only host sees. Client call RocketMan(), client doesn't do RocketMan, but host sees RocketMan on client.

//////Remember, this was all called from Update() A [ClientCallback], we are destroying an object
//No tags, Host can call, only host sees. Client can call, only client sees
//[Client] Host can call, only host sees. Client can call, only client sees
//[ClientRpc] Host can call, everyone sees. 
//[Command] Anyone can call, only host sees
//Error message for 2 below, called when server not active
//[Server] ???KEEP AN EYE, not completely sure. Host can call, only host sees
//[TargetRpc] ???KEEP AN EYE, not completely sure. Host can call, only host sees




//[Server]
//Only a server can call the method(throws a warning or an error when called on a client).
//[ServerCallback]
//Same as Server but does not throw warning when called on client.

//[Client]
//Only a Client can call the method(throws a warning or an error when called on the server).
//[ClientCallback]
//Same as Client but does not throw warning when called on server.

//[ClientRpc]
//The server uses a Remote Procedure Call(RPC) to run that function on clients.See also: Remote Actions
//[TargetRpc]
// This is an attribute that can be put on methods of NetworkBehaviour classes to allow them to be invoked on clients from a server. Unlike the ClientRpc attribute, 
//these functions are invoked on one individual target client, not all of the ready clients. See also: Remote Actions
//[Command]
//Call this from a client to run this function on the server. Make sure to validate input etc. 
//It's not possible to call this from a server. Use this as a wrapper around another function, if you want to call it from the server too. 
#endregion