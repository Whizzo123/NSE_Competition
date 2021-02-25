using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bolt;

public class PlayerController : EntityBehaviour<IGamePlayerState>
{
    public GameObject playerNameText;
    public Camera playerCamera;
    public GameObject nameTextPrefab;
    private Vector3 lastMousePos;
    public bool inverted;
    public float rotationSpeed;
    private ArtefactBehaviour targetedArtefact;
    private List<ItemArtefact> inventory;

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            playerNameText = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerNameText"));
            playerNameText.transform.SetParent(FindObjectOfType<Canvas>().transform);
            playerNameText.GetComponent<RectTransform>().position = new Vector3(-18, 271);
        }
        //Set state transform to be equal to current transform
        state.SetTransforms(state.PlayerTransform, transform);

        //Add a callback that whenever state.Name is modified change playerNameText.text 
        state.AddCallback("Name", () => 
        {
            if (entity.IsOwner)
            {
               //playerNameText.GetComponent<Text>().text = state.Name;
            }
            else
            {
                //Stops other players name text showing up on screen
                //playerNameText.gameObject.SetActive(false);
            }
        });

        inventory = new List<ItemArtefact>();
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
        inventory.Add(item);
        FindObjectOfType<CanvasUIManager>().PopupArtefactPickupDisplay(item);
        FindObjectOfType<CanvasUIManager>().AddToInventoryScreen(item);
    }

    public override void SimulateOwner()
    {
        float speed = 4f;
        Vector3 movement = Vector3.zero;

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

        if(Input.GetKeyDown(KeyCode.E) && targetedArtefact != null)
        {
            targetedArtefact.Pickup(this);
            targetedArtefact = null;
        }
    }

    private void Setup(string playerName)
    {
        if(entity.IsOwner)
        {
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
        }
    }

    public static void Spawn()
    {
        Vector3 pos = new Vector3(Random.Range(-16, 16), 0.6f, Random.Range(-16, 16));

        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, pos, Quaternion.identity);
        playerEntity.TakeControl();

        BoltLog.Info("Spawning player");

        PlayerController playerController = playerEntity.GetComponent<PlayerController>();

        CanvasUIManager.SpawnPlayerNameTextPrefab(playerController);


        if (PlayerPrefs.GetString("username") != null)
        {
            playerController.Setup(PlayerPrefs.GetString("username"));
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