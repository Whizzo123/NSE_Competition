using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bolt;

public class PlayerController : EntityBehaviour<IGamePlayerState>
{
    #region Variables
    public GameObject playerNameText;
    public Camera playerCamera;
    public GameObject nameTextPrefab;
    private Vector3 lastMousePos;
    public bool inverted;
    public float rotationSpeed;
    //Stored interactables
    private ArtefactBehaviour targetedArtefact;
    private Stash gameStash;
    private PlayerController targetedPlayerToStealFrom;
    private AbilityPickup targetedAbilityPickup;
    public float speed = 4f;
    private bool loadoutReleased;
    public AbilityInventory abilityInventory;

    public static PlayerController localPlayer;

    #endregion

    /// <summary>
    /// Called when entity attached to network 
    /// </summary>
    public override void Attached()
    { 

        state.SetTransforms(state.PlayerTransform, transform);
        SetLoadoutReleased(false);
        abilityInventory = new AbilityInventory(this);
        //Set state transform to be equal to current transform
        if (entity.IsOwner)
        {
            state.Speed = 4f;
            state.LoadoutReady = false;
            for (int i = 0; i < state.Inventory.Length; i++)
            {
                state.Inventory[i].ItemName = "";
                state.Inventory[i].ItemPoints = 0;
            }
        }
        if(!entity.IsOwner)
        {
            //Disable other players cameras so that we don't accidentally get assigned to another players camera
            playerCamera.gameObject.SetActive(false);
        }
    }

    #region ArtefactInventory
    /// <summary>
    /// Called in order to add artefact to player inventory
    /// </summary>
    /// <param name="artefactName"></param>
    /// <param name="artefactPoints"></param>
    public void AddToInventory(string artefactName, int artefactPoints)
    {
        ItemArtefact item = new ItemArtefact();
        item.name = artefactName;
        item.points = artefactPoints;
        int emptySlot = FindEmptyInventorySlot();
        if (emptySlot > -1)
        {
            state.Inventory[emptySlot].ItemName = artefactName;
            state.Inventory[emptySlot].ItemPoints = artefactPoints;
            FindObjectOfType<CanvasUIManager>().PopupArtefactPickupDisplay(item);
            FindObjectOfType<CanvasUIManager>().AddToInventoryScreen(item);
        }
        else
        {
            BoltLog.Error("Inventory is full");
        }
    }

    /// <summary>
    /// Called in order to remove from artefact from inventory
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="points"></param>
    public void RemoveFromInventory(int index, string name, int points)
    {
        state.Inventory[index].ItemName = "";
        state.Inventory[index].ItemPoints = 0;
        ItemArtefact itemArtefact;
        itemArtefact.name = name;
        itemArtefact.points = points;
        FindObjectOfType<CanvasUIManager>().RemoveFromInventoryScreen(itemArtefact);
    }

    /// <summary>
    /// Find empty inventory slot from player inventory
    /// </summary>
    /// <returns></returns>
    private int FindEmptyInventorySlot()
    {
        for (int i = 0; i < state.Inventory.Length; i++)
        {
            if(state.Inventory[i].ItemName == "")
            {
                return i;
            }
        }
        return -1;
    }

    //Remove all items from inventory
    public void ClearInventory()
    {
        FindObjectOfType<CanvasUIManager>().inventoryUI.ClearInventoryScreen();
        for (int i = 0; i < state.Inventory.Length; i++)
        {
            state.Inventory[i].ItemName = "";
            state.Inventory[i].ItemPoints = 0;
        }
    }

    /// <summary>
    /// Check to see whether inventory has any empty slots
    /// </summary>
    /// <returns></returns>
    public bool IsInventoryEmpty()
    {
        for (int i = 0; i < state.Inventory.Length; i++)
        {
            if (state.Inventory[i].ItemName == "")
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Just grab the first item in the player inventory
    /// </summary>
    /// <returns></returns>
    public InventoryItem GrabRandomItem()
    {
        return state.Inventory[0];
    }
    #endregion 

    public override void ControlGained()
    {
        localPlayer = this;
    }

    

    /// <summary>
    /// Called on every update of the owner computer a.k.a computer that created this entity
    /// </summary>
    public override void SimulateOwner()
    {
        Vector3 movement = Vector3.zero;

        abilityInventory.Update();

        if(playerNameText == null)
        {
            playerNameText = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerNameText"));
            playerNameText.transform.SetParent(FindObjectOfType<CanvasUIManager>().playerTextContainer.transform);
            playerNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
            playerNameText.SetActive(true);
            playerNameText.GetComponent<Text>().text = "Player Name: " + state.Name;
        }

        if (loadoutReleased)
        {

            if (Input.GetKey(KeyCode.W)) { movement.z += 1; }
            if (Input.GetKey(KeyCode.S)) { movement.z -= 1; }
            if (Input.GetKey(KeyCode.A)) { movement.x -= 1; }
            if (Input.GetKey(KeyCode.D)) { movement.x += 1; }


            if (movement != Vector3.zero)
            {
                transform.Translate(movement.normalized * state.Speed * BoltNetwork.FrameDeltaTime);
            }

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
            }*/

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (targetedArtefact != null)
                {
                    targetedArtefact.Pickup(this);
                    targetedArtefact = null;
                }
                else if (gameStash != null)
                {
                    gameStash.AddToStashScores(this);
                }
                else if(targetedAbilityPickup != null)
                {
                    targetedAbilityPickup.PickupAbility(this);
                    targetedAbilityPickup = null;
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                BoltLog.Info("F pressed");
                if (targetedPlayerToStealFrom != null)
                {
                    BoltLog.Info("Has a target");
                    if (targetedPlayerToStealFrom.IsInventoryEmpty())
                    {
                        BoltLog.Info("Attempting steal");
                        InventoryItem randomArtefact = targetedPlayerToStealFrom.GrabRandomItem();
                        AddToInventory(randomArtefact.ItemName, randomArtefact.ItemPoints);
                        int indexToRemove = -1;
                        for (int i = 0; i < targetedPlayerToStealFrom.state.Inventory.Length; i++)
                        {
                            if (targetedPlayerToStealFrom.state.Inventory[i].ItemName == randomArtefact.ItemName)
                            {
                                indexToRemove = i;
                            }
                        }
                        var request = InventoryRemove.Create();
                        request.ItemIndex = indexToRemove;
                        request.InventoryEntity = targetedPlayerToStealFrom.entity;
                        request.ItemName = randomArtefact.ItemName;
                        request.ItemPoints = randomArtefact.ItemPoints;
                        request.Send();
                    }
                }
            }
        }
    }


   

    /// <summary>
    /// Sets player name to state
    /// </summary>
    /// <param name="playerName"></param>
    private void Setup(string playerName)
    {
        if(entity.IsOwner)
        {
            BoltLog.Info("Setup: " + playerName);
            state.Name = playerName;
        }
    }

    #region Collision

    public void OnTriggerEnter(Collider collider)
    {
        BoltLog.Info("Colliding with object");
        if (entity.IsOwner)
        {
            if (collider.gameObject.GetComponent<ArtefactBehaviour>())
            {
                BoltLog.Info("Colliding with artefact");
                targetedArtefact = collider.gameObject.GetComponent<ArtefactBehaviour>();
            }
            else if(collider.gameObject.GetComponent<Stash>())
            {
                BoltLog.Info("Can open stash");
                gameStash = collider.gameObject.GetComponent<Stash>();
            }
            else if(collider.gameObject.GetComponent<PlayerController>())
            {
                BoltLog.Info("Colliding with player");
                targetedPlayerToStealFrom = collider.gameObject.GetComponent<PlayerController>();
            }
            else if(collider.gameObject.GetComponent<AbilityPickup>())
            {
                targetedAbilityPickup = collider.gameObject.GetComponent<AbilityPickup>();
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        BoltLog.Info("Exiting collision with object");
        if (entity.IsOwner)
        {
            if (collider != null)
            {
                if (targetedArtefact != null && collider.gameObject == targetedArtefact.gameObject)
                {
                    BoltLog.Info("Exiting from artefact");
                    targetedArtefact = null;
                }
                else if (gameStash != null && collider.gameObject == gameStash.gameObject)
                {
                    BoltLog.Info("Exiting game stash");
                    gameStash = null;
                }
                else if (targetedPlayerToStealFrom != null && collider.gameObject == targetedPlayerToStealFrom.gameObject)
                {
                    BoltLog.Info("Exiting from other player collider");
                    targetedPlayerToStealFrom = null;
                }
                else if (targetedAbilityPickup != null && collider.gameObject == targetedAbilityPickup.gameObject)
                {
                    BoltLog.Info("Exiting from other player collider");
                    targetedAbilityPickup = null;
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Used to set whether we are able to move now or not
    /// </summary>
    /// <param name="value"></param>
    public void SetLoadoutReleased(bool value)
    {
        loadoutReleased = value;
    }

    /// <summary>
    /// Used to spawn in gameobject for player
    /// </summary>
    public static void Spawn()
    {
        Vector3 pos = new Vector3(Random.Range(-16, 16), 0.6f, Random.Range(-16, 16));

        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, pos, Quaternion.identity);
        playerEntity.TakeControl();
        //string playerUsername = FindObjectOfType<PlayerData>().GetUsername(playerEntity.Controller);


        BoltLog.Info("Spawning player");

        PlayerController playerController = playerEntity.GetComponent<PlayerController>();

       // CanvasUIManager.SpawnPlayerNameTextPrefab(playerController);


        if (PlayerPrefs.GetString("username") != null)
        {
            playerController.Setup(PlayerPrefs.GetString("username"));
            BoltLog.Info("Player Username is: " + PlayerPrefs.GetString("username"));
        }
        else
        {
            playerController.Setup("Player #" + Random.Range(1, 100));
        }
    }
}

public struct ItemArtefact
{
    public string name;
    public int points;
}