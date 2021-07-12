using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    #region Variables
    [Header("Stored Interactables")]
    //Stored interactables
    [Tooltip("The artefacts that are in range for picking up")] readonly SyncList<ArtefactBehaviour> targetedArtefacts = new SyncList<ArtefactBehaviour>();
    [Tooltip("NA")] private Stash gameStash;
    [Tooltip("The player that is currently targeted to steal artefacts from")] private PlayerController targetedPlayerToStealFrom;
    [Tooltip("In devlopment: The ability pickups that are in range for picking up")] private AbilityPickup targetedAbilityPickup;
    //Loadout and inventory
    [Tooltip("Have we exited the loadout menu")] private bool loadoutReleased;
    [Tooltip("Our abilities that we've selected")] [SyncVar]public AbilityInventory abilityInventory;
    [SyncVar]private ArtefactInventory artefactInventory;
    [SyncVar]
    public string playerName;


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

    [Tooltip("Camera attatched to the player")] public Camera playerCamera;
    [Tooltip("NA")] public Cinemachine.CinemachineVirtualCamera vCam;
    [Tooltip("NA")] public Camera cam;
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

    public void Awake()
    {
        DontDestroyOnLoad(this);
        playerCharacterController = this.gameObject.GetComponent<CharacterController>();
    }

    public override void OnStartAuthority()
    {
        vCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (vCam != null)
        {
            Invoke("SetCamera", 0);
        }
        else
        {
            Invoke("SetCamera", 5);
        }
        CmdSetupPlayer();
        SetLoadoutReleased(false);
        base.OnStartAuthority();
    }

    [Command]
    private void CmdSetupPlayer()
    {
        abilityInventory = new AbilityInventory(this);
        artefactInventory = GetComponent<ArtefactInventory>();
        immobilize = false;
        hasBeenStolenFrom = false;
    }


    [Client]
    void SetCamera()
    {
        vCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        DontDestroyOnLoad(vCam);
        vCam.LookAt = this.gameObject.transform;
        vCam.Follow = this.gameObject.transform;
        vCam.transform.rotation = Quaternion.Euler(45, 0, 0);
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
            devCam.SetActive(true);
        }
        else
        {
            devCam = Instantiate(devCam);
            devCam.SetActive(true);
        }
    }

    private void DevModeOn()
    {
        if(vCam != null)
            vCam.enabled = !devMode;
        cam.enabled = !devMode;
        playerCamera.enabled = !devMode;
        devCam.SetActive(devMode);
        FindObjectOfType<Canvas>().enabled = !devMode;
    }
    [ClientCallback]
    void Update()
    {


        if (!hasAuthority) { return; };

        #region DEVMODE
        if (Input.GetKey(KeyCode.P)) { devMode = true;}
        if (Input.GetKey(KeyCode.O)){devMode = false;}
        DevModeOn();
        if (devMode){return;}
        #endregion

        abilityInventory.Update();

        if (playerNameText == null && SceneManager.GetActiveScene().name == "GameScene")
        {
            playerNameText = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerNameText"));
            playerNameText.transform.SetParent(FindObjectOfType<CanvasUIManager>().playerTextContainer.transform);
            playerNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
            playerNameText.SetActive(true);
            playerNameText.GetComponent<Text>().text = playerName;
        }

        if (loadoutReleased)
        {


            if (immobilize == false)
            {
                #region Falling

                //Projects a sphere underneath player to check ground layer
                isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 2, 0), groundDistance, ground);

                //Player recieves a constant y velocity from gravity
                playerFallingVelocity.y += playerGravity;// * Time.deltaTime;

                //If player is fully grounded then apply some velocity down, this will change the 'floating' period before plummeting.
                if (isGrounded && playerFallingVelocity.y < 0)
                {
                    playerFallingVelocity.y = -1f;
                }
                playerFallingVelocity.y = -1f;

                #endregion

                #region Movement
                playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), playerFallingVelocity.y, Input.GetAxisRaw("Vertical")).normalized;
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                {
                    direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                    playerAnim.SetBool("moving", true);
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
                playerCharacterController.Move(playerMovement * speed * Time.deltaTime);
                PlayerRotation();
                #endregion
            }
            else
                playerAnim.SetBool("moving", false);
        }



        #region Artefact interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (targetedArtefacts.Count != 0)
            {
                if (artefactInventory.FindEmptyInventorySlot() != -1)
                {
                    Debug.Log("Picking up Artefacts");
                    // Now we are using a list, so we will pick all up, but we won't run into exiting and entering issues
                    foreach (ArtefactBehaviour item in targetedArtefacts)
                    {
                        artefactInventory.AddToInventory(item.GetArtefactName(), item.GetPoints());
                        FindObjectOfType<AudioManager>().PlaySound(item.GetRarity().ToString());
                        DestroyGameObject(item.gameObject);
                    }
                    CmdClearTargetArtefacts();
                    if(NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
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
                gameStash.CmdAddToStashScores(this);
                FindObjectOfType<AudioManager>().PlaySound("Stash");
            }
            else if(gameStash != null && !artefactInventory.InventoryNotEmpty())
            {
                FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot deposit no artefacts in inventory");
            }
        }
        #endregion

        #region Stealing
        if (Input.GetKeyDown(KeyCode.F))// && !state.HasBeenStolenFrom)
        {
            Debug.LogError(artefactInventory.GetAllArtefactNames());
            if (targetedPlayerToStealFrom != null)
            {
                if (artefactInventory.AvailableInventorySlot() && targetedPlayerToStealFrom.GrabArtefactInventory().InventoryNotEmpty())
                {
                    //We are not full, they are no longer stunned and have artefacts, we steal

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

        #region Obstacle Interaction
        if (Input.GetKey(KeyCode.Space) && toolWait== false )//&& state.Paralyzed == false)
        {
            playerAnim.SetTrigger("Cut");
            StartCoroutine(Hit());
        }
        #endregion

        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, ground))
            {
                string hitstring = hit.transform.gameObject.layer.ToString();
                int layernumber = int.Parse(hitstring);
                string lm = LayerMask.LayerToName(layernumber);
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
        if (playerFallingVelocity.y < -200)
        {
            CmdServerValidateHit();
        }
    }

    ArtefactInventory GrabArtefactInventory()
    {
        return artefactInventory;
    }


    [Command]
    private void CmdClearTargetArtefacts()
    {
        targetedArtefacts.Clear();
    }


    [Command]
    private void CmdServerValidateHit()
    {
        //Validate the logic
        //We will pass in a transform for this

        RpcHitDown();
    }

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

    /// <summary>
    /// Used to set whether we are able to move now or not
    /// </summary>
    /// <param name="value"></param>
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
    /// </summary>
    [ClientCallback]
    void PlayerRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, ground))
        {
            Quaternion finalQuat;
            Vector3 axisOfRotation = (Vector3.Cross(hit.normal, Vector3.up)).normalized;//gets the axis to rotate on a slope.
            float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(for somereason it's negative either way round) (dot product)
            Quaternion slopeQuat = Quaternion.AngleAxis(-rotationAngle, axisOfRotation);//Quaternion of the rotation necessary to angle the player on a slope

            Quaternion lookQuat = Quaternion.LookRotation(direction, Vector3.up);//Quaternion of the direction of player movement

            if (rotationAngle < 45) 
            {
                finalQuat = slopeQuat.normalized * lookQuat; //Quaternion rotation of look rotation and slope rotation
                transform.rotation = finalQuat;
            }

        }

    }

    #region Collision

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<Stash>())
        {
            gameStash = collider.gameObject.GetComponent<Stash>();
            if(FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Deposit");
        }
        else if (collider.gameObject.GetComponent<AbilityPickup>())
        {
            targetedAbilityPickup = collider.gameObject.GetComponent<AbilityPickup>();
        }
        if (collider.gameObject.GetComponent<PlayerController>())
        {
            targetedPlayerToStealFrom = collider.gameObject.GetComponent<PlayerController>();
            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press F to Steal");
        }

    }

    public List<ArtefactBehaviour> tempArtefactStorage;

    public void OnTriggerStay(Collider collider)
    {

        if (collider.gameObject.GetComponent<ArtefactBehaviour>() && tempArtefactStorage.Contains(collider.gameObject.GetComponent<ArtefactBehaviour>()) == false && collider.gameObject.GetComponent<ArtefactBehaviour>().IsAvaliableForPickup())
        {
            tempArtefactStorage.Add(collider.gameObject.GetComponent<ArtefactBehaviour>());
            CmdAddToTargetedArtefacts(collider.gameObject.GetComponent<ArtefactBehaviour>());
            if (FindObjectOfType<CanvasUIManager>() != null && NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Pickup");
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider != null)
        {
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
            else if (gameStash != null && collider.gameObject == gameStash.gameObject)
            {
                gameStash = null;
                if(NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
                    FindObjectOfType<CanvasUIManager>().CloseHintMessage();
            }
            else if (targetedAbilityPickup != null && collider.gameObject == targetedAbilityPickup.gameObject)
            {
                targetedAbilityPickup = null;
            }
            if (targetedPlayerToStealFrom != null && collider.gameObject == targetedPlayerToStealFrom.gameObject)
            {
                targetedPlayerToStealFrom = null;
                if(NetworkClient.localPlayer.GetComponent<PlayerController>() == this)
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
        toolWait= false;
    }

    [Command]
    void CmdHitForward()
    {
        HitForward();
    }

    /// <summary>
    /// Destroys obstacles directly in front of player. This relies on PlayerRotation().
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * lengthOfSphere), radiusOfSphere);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, -transform.up + transform.position);
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(transform.position - new Vector3(0, 2, 0), groundDistance);
    }

    #endregion


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

    public void ToggleMesh(bool toggle)
    {
        //Player -> _scaleTest -> FULL.002
        transform.GetChild(0).transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = toggle;
    }

    public ArtefactInventory GetArtefactInventory()
    {
        return artefactInventory;
    }

    [Command(requiresAuthority = false)]
    public void CmdModifySpeed(float newSpeed)
    {
        //Debug.Log("Modifying Speed: " + playerName);
        speed = newSpeed;
    }

    public void SetArtefactInventory(ArtefactInventory inventory)
    {
        artefactInventory = inventory;
    }

    public bool IsImmobilized()
    {
        return immobilize;
    }
    [Command (requiresAuthority = false)]
    public void CmdSetImmobilized(bool value)
    {
        immobilize = value;
    }

    public bool IsVoodooPoisoned()
    {
        return voodooPoisoned;
    }
    [Command (requiresAuthority = false)]
    public void CmdSetVoodooPoisoned(bool poisoned)
    {
        voodooPoisoned = poisoned;
    }

    public bool IsMortal()
    {
        return mortal;
    }
    [Command(requiresAuthority = false)]
    public void CmdSetMortal(bool mortal)
    {
        this.mortal = mortal;
    }
    [Command (requiresAuthority = false)]
    public void CmdSetHasBeenStolenFrom(bool value)
    {
        hasBeenStolenFrom = value;
    }
    [Command]
    public void CmdSpawnBearTrap(Vector3 spawnPos, PlayerController placingPlayer)
    {
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "BearTrap"), spawnPos, Quaternion.identity);
        go.GetComponent<BearTrapBehaviour>().SetPlacingPlayer(placingPlayer);
        NetworkServer.Spawn(go);
    }
    [Command]
    public void CmdSpawnVoodooTrap(Vector3 spawnPos, PlayerController placingPlayer)
    {
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "VoodooPoisonTrap"), spawnPos, Quaternion.identity);
        go.GetComponent<VoodooPoisonTrapBehaviour>().SetPlacingPlayer(placingPlayer);
        NetworkServer.Spawn(go);
    }


    [ClientCallback]
    public void DestroyGameObject(GameObject go)
    {
        CmdDestroyGameObject(go);
    }
    [Command(requiresAuthority = false)]
    public void CmdDestroyGameObject(GameObject go)
    {
        NetworkServer.Destroy(go);
    }
    [Command]
    public void CmdSpawnStickyBombParticles(Vector3 spawnPos, float effectDuration)
    {
        GameObject stickyBombParticles = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefab => spawnPrefab.name == "SlowBombExplosion_PA"), spawnPos, Quaternion.identity);
        stickyBombParticles.GetComponent<StickyBombBehaviour>().effectDuration = effectDuration;
        stickyBombParticles.GetComponent<StickyBombBehaviour>().tick = true;
        NetworkServer.Spawn(stickyBombParticles);
    }
    [Command]
    public void CmdSpawnCamouflageParticles(Vector3 spawnPos)
    {
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "Invisibility_PA"),
            spawnPos, Quaternion.identity);
        NetworkServer.Spawn(go);
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
            Debug.Log("RpcToggleCamouflage the ClientRpc is hitting client called: " + NetworkClient.localPlayer.GetComponent<PlayerController>().playerName);
    }
}




#region deadCode
/*
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

             #region movement

            #endregion
 */
#endregion

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