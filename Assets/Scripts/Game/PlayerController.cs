using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class PlayerController : NetworkBehaviour
{
    #region Variables
    [Header("Stored Interactables")]
    //Stored interactables
    private List<ArtefactBehaviour> targetedArtefacts;
    private Stash gameStash;
    private PlayerController targetedPlayerToStealFrom;
    private AbilityPickup targetedAbilityPickup;
    public float speed = 20f;
    private bool loadoutReleased;
    public AbilityInventory abilityInventory;
    public bool immobilize;
    private float currentStunAfterTimer;
    private float timeForStunAfterSteal;
    [Space]

    [Header("Player options")]
    [SerializeField] [Tooltip("Time delay before destroying another obstacle")] [Range(0, 1)] private float waitTime = 0.05f;
    private bool wait = false;
    [SerializeField] private float playerGravity = -65;
    [SerializeField] private float groundDistance = 2.5f;
    public float lengthOfSphere = 2f;
    public float radiusOfSphere = 1f;
    [Space]

    [Header("LayerMasks and Components")]
    public LayerMask obstacles;
    public LayerMask ground;
    [Space]
    public PlayerController localPlayer;
    public CharacterController playerCharacterController;//See attached()
    private Vector3 direction;
    public GameObject playerNameText;
    public Camera playerCamera;
    public GameObject nameTextPrefab;
    [Space]

    [Header("States")]
    [SerializeField] private Vector3 playerFallingVelocity;
    private Vector3 playerMovement = Vector3.zero;
    private bool isGrounded = true;
    public Cinemachine.CinemachineVirtualCamera vCam;
    public Camera cam;
    public Vector3 offset = new Vector3(0, 10, 10);




    #endregion

    public void Awake()
    {
        DontDestroyOnLoad(this);
        playerCharacterController = this.gameObject.GetComponent<CharacterController>();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }
    //public override void OnStartAuthority()
    //{
    //    targetedArtefacts = new List<ArtefactBehaviour>();
    //    //state.SetTransforms(state.PlayerTransform, transform);
    //    SetLoadoutReleased(false);
    //    abilityInventory = new AbilityInventory(this);
    //    immobilize = false;
    //    timeForStunAfterSteal = 10.0f;
    //    ////Set state transform to be equal to current transform
    //    //    state.Speed = speed;
    //    //    state.RayLength = lengthOfSphere;
    //    //    state.LoadoutReady = false;
    //    //    state.HasBeenStolenFrom = false;
    //    //    state.Paralyzed = false;
    //    //    for (int i = 0; i < state.Inventory.Length; i++)
    //    //    {
    //    //        state.Inventory[i].ItemName = "";
    //    //        state.Inventory[i].ItemPoints = 0;
    //    //    }
    //        //playerCharacterController = this.gameObject.GetComponent<CharacterController>();
    //        vCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
    //        vCam.LookAt = this.gameObject.transform;
    //        vCam.Follow = this.gameObject.transform;
    //        vCam.transform.rotation = Quaternion.Euler(45, 0, 0);
    //    //}
    //    //if (!entity.IsOwner)
    //    //{
    //    //    //Disable other players cameras so that we don't accidentally get assigned to another players camera
    //    //    if (playerCamera != null)
    //    //        playerCamera.gameObject.SetActive(false);
    //    //}
    //    localPlayer = this;
    //    playerCamera = FindObjectOfType<Camera>();

    //    this.gameObject.transform.position = new Vector3(0, 0, 0);
    //}

    #region ArtefactInventory
    /// <summary>
    /// Called in order to add artefact to player inventory
    /// </summary>
    /// <param name="artefactName"></param>
    /// <param name="artefactPoints"></param>
    //public void AddToInventory(string artefactName, int artefactPoints)
    //{
    //    ItemArtefact item = new ItemArtefact();
    //    item.name = artefactName;
    //    item.points = artefactPoints;
    //    int emptySlot = FindEmptyInventorySlot();
    //    if (emptySlot > -1)
    //    {
    //        state.Inventory[emptySlot].ItemName = artefactName;
    //        state.Inventory[emptySlot].ItemPoints = artefactPoints;
    //        FindObjectOfType<CanvasUIManager>().PopupArtefactPickupDisplay(item);
    //        FindObjectOfType<CanvasUIManager>().AddToInventoryScreen(item);
    //    }
    //    else
    //    {
    //        BoltLog.Error("Inventory is full");
    //    }
    //}

    /// <summary>
    /// Called in order to remove from artefact from inventory
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="points"></param>
    //public void RemoveFromInventory(int index, string name, int points)
    //{
    //    state.Inventory[index].ItemName = "";
    //    state.Inventory[index].ItemPoints = 0;
    //    ItemArtefact itemArtefact;
    //    itemArtefact.name = name;
    //    itemArtefact.points = points;
    //    FindObjectOfType<CanvasUIManager>().RemoveFromInventoryScreen(itemArtefact);
    //}

    /// <summary>
    /// Find empty inventory slot from player inventory
    /// </summary>
    /// <returns></returns>
    //private int FindEmptyInventorySlot()
    //{
    //    for (int i = 0; i < state.Inventory.Length; i++)
    //    {
    //        if (state.Inventory[i].ItemName == "")
    //        {
    //            return i;
    //        }
    //    }
    //    return -1;
    //}

    //Remove all items from inventory
    //public void ClearInventory()
    //{
    //    FindObjectOfType<CanvasUIManager>().inventoryUI.ClearInventoryScreen();
    //    for (int i = 0; i < state.Inventory.Length; i++)
    //    {
    //        state.Inventory[i].ItemName = "";
    //        state.Inventory[i].ItemPoints = 0;
    //    }
    //}

    /// <summary>
    /// Check to see whether inventory has any empty slots
    /// </summary>
    /// <returns></returns>
    //public bool IsInventoryEmpty()
    //{
    //    for (int i = 0; i < state.Inventory.Length; i++)
    //    {
    //        if (state.Inventory[i].ItemName == "")
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    /// <summary>
    /// Just grab the first item in the player inventory
    /// </summary>
    /// <returns></returns>
    //public InventoryItem GrabRandomItem()
    //{
    //    return state.Inventory[0];
    //}
    #endregion 

    //public bool InventoryNotEmpty()
    //{
    //    for (int i = 0; i < state.Inventory.Length; i++)
    //    {
    //        if (state.Inventory[i].ItemPoints > 0) return true;
    //    }
    //    return false;
    //}
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; };
        //abilityInventory.Update();

        //if (playerNameText == null)
        //{
        //    playerNameText = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerNameText"));
        //    playerNameText.transform.SetParent(FindObjectOfType<CanvasUIManager>().playerTextContainer.transform);
        //    playerNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
        //    playerNameText.SetActive(true);
        //    playerNameText.GetComponent<Text>().text = "temp";
        //}

       // if (loadoutReleased)
        //{
           //if (immobilize == false)
           //{
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
                playerMovement = new Vector3
                (Input.GetAxisRaw("Horizontal"),
                 playerFallingVelocity.y,
                 Input.GetAxisRaw("Vertical")).normalized;



        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            ///////////////////////////////////////////////////////////////////Poison effect, place somewhere else?
            if (false)//poisoned?
            {
                playerMovement = new Vector3(playerMovement.x * -1, playerMovement.y, playerMovement.z * -1);
                direction *= -1;
            }

            //if (transform.GetChild(0).GetComponent<Animator>().GetBool("moving") == false)
            //{
            //    var request = ChangeAnimatorMovementParameter.Create();
            //    request.Target = entity;
            //    request.Value = true;
            //    request.Send();
            //}
        }
        else
        {
            //if (transform.GetChild(0).GetComponent<Animator>().GetBool("moving") == true)
            //{
            //    var request = ChangeAnimatorMovementParameter.Create();
            //    request.Target = entity;
            //    request.Value = false;
            //    request.Send();
            //}
        }
        //this.GetComponent<Rigidbody>().velocity = playerFallingVelocity;
        playerCharacterController.Move(playerMovement * speed * Time.deltaTime);
        PlayerRotation();

        //  }
        #endregion


        #region Artefact interaction
        if (Input.GetKeyDown(KeyCode.E))
            {
                //Debug.LogError("Artefact count " + targetedArtefacts.Count);
                //if (targetedArtefacts.Count != 0)
                //{
                //    if (FindEmptyInventorySlot() != -1)
                //    {
                //        Debug.Log("Picking up Artefacts");
                //        // targetedArtefact.Pickup(this);

                //        // Now we are using a list, so we will pick all up, but we won't run into exiting and entering issues
                //        foreach (ArtefactBehaviour item in targetedArtefacts)
                //        {
                //            item.Pickup(this);
                //            FindObjectOfType<AudioManager>().PlaySound(item.GetRarity().ToString());
                //        }

                //        targetedArtefacts.Clear();
                //    }
                //    else
                //    {
                //        FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot pickup artefact inventory is full (Max: 8 artefacts)");
                //    }
                //}
                //else if (targetedAbilityPickup != null)
                //{
                //    targetedAbilityPickup.PickupAbility(this);
                //    targetedAbilityPickup = null;
                //}
                //else if (gameStash != null && InventoryNotEmpty())
                //{
                //    gameStash.AddToStashScores(this);
                //    FindObjectOfType<AudioManager>().PlaySound("Stash");
                //}
                //else if(gameStash != null && !InventoryNotEmpty())
                //{
                //    FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot deposit no artefacts in inventory");
                //}
            }
            #endregion

            #region Stealing
            //if (Input.GetKeyDown(KeyCode.F) && !state.HasBeenStolenFrom)
            //{
            //    BoltLog.Info("F pressed");
            //    if (targetedPlayerToStealFrom != null)
            //    {
            //        BoltLog.Info("Has a target");
            //        if (IsInventoryEmpty() && targetedPlayerToStealFrom.state.HasBeenStolenFrom == false && targetedPlayerToStealFrom.InventoryNotEmpty())
            //        {
            //            BoltLog.Info("Attempting steal");
            //            InventoryItem randomArtefact = targetedPlayerToStealFrom.GrabRandomItem();
            //            AddToInventory(randomArtefact.ItemName, randomArtefact.ItemPoints);
            //            int indexToRemove = -1;
            //            for (int i = 0; i < targetedPlayerToStealFrom.state.Inventory.Length; i++)
            //            {
            //                if (targetedPlayerToStealFrom.state.Inventory[i].ItemName != string.Empty && targetedPlayerToStealFrom.state.Inventory[i].ItemName == randomArtefact.ItemName)
            //                {
            //                    indexToRemove = i;
            //                }
            //            }
            //            var request = InventoryRemove.Create();
            //            request.ItemIndex = indexToRemove;
            //            request.InventoryEntity = targetedPlayerToStealFrom.entity;
            //            request.ItemName = randomArtefact.ItemName;
            //            request.ItemPoints = randomArtefact.ItemPoints;
            //            request.Send();
            //        }
            //        else
            //        {
            //            FindObjectOfType<CanvasUIManager>().PopupMessage("Cannot steal from player has no artefacts or stolen from recently");
            //        }
            //    }
            //}

            //if (state.HasBeenStolenFrom)
            //{
            //    if (currentStunAfterTimer >= timeForStunAfterSteal)
            //    {
            //        currentStunAfterTimer = 0;
            //        state.HasBeenStolenFrom = false;
            //        var request = StunEnemyPlayer.Create();
            //        request.Target = entity;
            //        request.End = true;
            //        request.Send();
            //    }
            //    else
            //    {
            //        currentStunAfterTimer += Time.deltaTime;
            //    }
            //}
            #endregion

            #region Obstacle Interaction
            if (Input.GetKey(KeyCode.Space) && wait == false )//&& state.Paralyzed == false)
            {
                StartCoroutine(Hit());
            }
            #endregion


            //if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && isGrounded)
            //{
            //    RaycastHit hit;
            //    if (Physics.Raycast(transform.position, Vector3.down, out hit, ground))
            //    {
            //        string hitstring = hit.transform.gameObject.layer.ToString();
            //        int layernumber = int.Parse(hitstring);
            //        string lm = LayerMask.LayerToName(layernumber);
            //        if (lm == "SwampGround")
            //        {
            //            FindObjectOfType<AudioManager>().PlaySoundOnly(lm);
            //        }
            //        else
            //        {
            //            FindObjectOfType<AudioManager>().StopSound("SwampGround");
            //        }
            //        FindObjectOfType<AudioManager>().PlaySoundOnly(lm);
            //    }
            //}
            //if (playerFallingVelocity.y < -200)
            //{
            //    CmdServerValidateHit();
            //}

        //}
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
                /*ab = item.transform.gameObject.GetComponent<ArtefactBehaviour>();
                var req = ArtefactEnable.Create();
                req.artefact = ab.entity;
                req.Send();*/
                //item.transform.gameObject.GetComponentInChildren<ArtefactBehaviour>().transform.SetParent(null);
            }
            else if (item.transform.GetComponent<AbilityPickup>())
            {
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                item.transform.GetComponent<AbilityPickup>().enabledForPickup = true;
            }
            else
            {
                Destroy(item.transform.gameObject);
            }
            //var request = ObstacleDisable.Create();
            //request.position = transform.position;
            //request.forward = transform.forward;
            //request.Send();
        }
    }

    /// <summary>
    /// Sets player name to state
    /// </summary>
    /// <param name="playerName"></param>
    private void Setup(string playerName)
    {
        //if(entity.IsOwner)
        //{
        //    BoltLog.Info("Setup: " + playerName);
        //    state.Name = playerName;
        //}
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
    /// Used to spawn in gameobject for player
    /// </summary>
    public static void Spawn()
    {
        Vector3 pos = new Vector3(Random.Range(2.26f, 3.86f), 0.6f, Random.Range(-26.13f, -11.94f));

        //BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, pos, Quaternion.identity);
        //playerEntity.TakeControl();
        //string playerUsername = FindObjectOfType<PlayerData>().GetUsername(playerEntity.Controller);


        BoltLog.Info("Spawning player");

        //PlayerController playerController = playerEntity.GetComponent<PlayerController>();

       // CanvasUIManager.SpawnPlayerNameTextPrefab(playerController);


        if (PlayerPrefs.GetString("username") != null)
        {
            //playerController.Setup(PlayerPrefs.GetString("username"));
        }
        else
        {
            //playerController.Setup("Player #" + Random.Range(1, 100));
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
        //if (entity.IsOwner)
        //{
        //    if (collider.gameObject.GetComponent<ArtefactBehaviour>())
        //    {
        //        targetedArtefacts.Add(collider.gameObject.GetComponent<ArtefactBehaviour>());
        //    }
        //    else if (collider.gameObject.GetComponent<Stash>())
        //    {
        //        gameStash = collider.gameObject.GetComponent<Stash>();
        //        FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press E to Deposit");
        //    }
        //    else if (collider.gameObject.GetComponent<PlayerController>())
        //    {
        //        targetedPlayerToStealFrom = collider.gameObject.GetComponent<PlayerController>();
        //        FindObjectOfType<CanvasUIManager>().ShowHintMessage("Press F to Steal");
        //    }
        //    else if (collider.gameObject.GetComponent<AbilityPickup>())
        //    {
        //        targetedAbilityPickup = collider.gameObject.GetComponent<AbilityPickup>();
        //    }
        //}
    }

    public void OnTriggerExit(Collider collider)
    {
        //if (entity.IsOwner)
        //{
        //    if (collider != null)
        //    {
        //        if (targetedArtefacts.Count != 0 && collider.gameObject.GetComponent<ArtefactBehaviour>())
        //        {
        //            //Removes specific artefact that we exited.
        //            int i = 0;
        //            foreach (ArtefactBehaviour item in targetedArtefacts)
        //            {
        //                if (item.GetInstanceID() == collider.gameObject.GetComponent<ArtefactBehaviour>().GetInstanceID())
        //                {
        //                    targetedArtefacts.RemoveAt(i);
        //                }
        //                i++;
        //            }
        //        }
        //        else if (gameStash != null && collider.gameObject == gameStash.gameObject)
        //        {
        //            gameStash = null;
        //            FindObjectOfType<CanvasUIManager>().CloseHintMessage();
        //        }
        //        else if (targetedPlayerToStealFrom != null && collider.gameObject == targetedPlayerToStealFrom.gameObject)
        //        {
        //            targetedPlayerToStealFrom = null;
        //            FindObjectOfType<CanvasUIManager>().CloseHintMessage();
        //        }
        //        else if (targetedAbilityPickup != null && collider.gameObject == targetedAbilityPickup.gameObject)
        //        {
        //            targetedAbilityPickup = null;
        //        }
        //    }
        //}
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
        CmdHitForward();
        yield return new WaitForSeconds(waitTime);
        wait = false;
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
                item.transform.GetComponent<AbilityPickup>().enabledForPickup = true;
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

    public void ToggleMesh(bool toggle)
    {
        //Player -> _scaleTest -> FULL.002
        transform.GetChild(0).transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = toggle;
    }
}

        

public struct ItemArtefact
{
    public string name;
    public int points;
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