﻿using System.Collections.Generic;
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

    private bool loadoutReleased;

    //---------------------------
   // public List<ItemArtefact> inventory;

    public static PlayerController localPlayer;
    #endregion

    public override void Attached()
    {
        state.SetTransforms(state.PlayerTransform, transform);
        loadoutReleased = false;
        //Set state transform to be equal to current transform
        if (entity.IsOwner)
        {
            for (int i = 0; i < state.Inventory.Length; i++)
            {
                state.Inventory[i].ItemName = "";
                state.Inventory[i].ItemPoints = 0;
            }
        }
        //inventory = new List<ItemArtefact>();
        if(!entity.IsOwner)
        {
            //Disable other players cameras so that we don't accidentally get assigned to another players camera
            playerCamera.gameObject.SetActive(false);
        }
    }

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

    public void RemoveFromInventory(int index, string name, int points)
    {
        state.Inventory[index].ItemName = "";
        state.Inventory[index].ItemPoints = 0;
        ItemArtefact itemArtefact;
        itemArtefact.name = name;
        itemArtefact.points = points;
        FindObjectOfType<CanvasUIManager>().RemoveFromInventoryScreen(itemArtefact);
    }

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

    public override void ControlGained()
    {
        localPlayer = this;
    }

    public void ClearInventory()
    {
        FindObjectOfType<CanvasUIManager>().inventoryUI.ClearInventoryScreen();
        for (int i = 0; i < state.Inventory.Length; i++)
        {
            state.Inventory[i].ItemName = "";
            state.Inventory[i].ItemPoints = 0;
        }
    }

    public override void SimulateOwner()
    {
        float speed = 4f;
        Vector3 movement = Vector3.zero;

        if(playerNameText == null)
        {
            playerNameText = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerNameText"));
            playerNameText.transform.SetParent(FindObjectOfType<CanvasUIManager>().playerTextContainer.transform);
            playerNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
            playerNameText.SetActive(true);
            playerNameText.GetComponent<Text>().text = "Player Name: " + state.Name;
        }

        if(Input.GetKey(KeyCode.W)) { movement.z += 1; }
        if(Input.GetKey(KeyCode.S)) { movement.z -= 1; }
        if(Input.GetKey(KeyCode.A)) { movement.x -= 1; }
        if(Input.GetKey(KeyCode.D)) { movement.x += 1; }


        if(movement != Vector3.zero)
        {
            transform.Translate(movement.normalized * speed * BoltNetwork.FrameDeltaTime);
        }

        if (Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height) 
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

        if(Input.GetKeyDown(KeyCode.E))
        {
            if (targetedArtefact != null)
            {
                targetedArtefact.Pickup(this);
                targetedArtefact = null;
            }
            if(gameStash != null)
            {
                gameStash.AddToStashScores(this);
            }
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            BoltLog.Info("F pressed");
            if(targetedPlayerToStealFrom != null)
            {
                BoltLog.Info("Has a target");
                if(targetedPlayerToStealFrom.IsInventoryEmpty())
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

    public InventoryItem GrabRandomItem()
    {
        return state.Inventory[0];
    }

    private void Setup(string playerName)
    {
        if(entity.IsOwner)
        {
            BoltLog.Info("Setup: " + playerName);
            state.Name = playerName;
        }
    }

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
            }
        }
    }

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