using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Bolt;

public class PlayerController : EntityBehaviour<IGamePlayerState>
{

    public Text playerNameText;

    public override void Attached()
    {
        //Set state transform to be equal to current transform
        state.SetTransforms(state.PlayerTransform, transform);

        //Add a callback that whenever state.Name is modified change playerNameText.text 
        state.AddCallback("Name", () => 
        {
            if (entity.IsOwner)
            {
                playerNameText.text = state.Name;
            }
            else
            {
                //Stops other players name text showing up on screen
                playerNameText.gameObject.SetActive(false);
            }
        });
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
            transform.position = transform.position + (movement.normalized * speed * BoltNetwork.FrameDeltaTime);
        }
    }

    private void Setup(string playerName)
    {
        if(entity.IsOwner)
        {
            state.Name = playerName;
        }
    }

    public static void Spawn()
    {
        Vector3 pos = new Vector3(Random.Range(-16, 16), 0.6f, Random.Range(-16, 16));

        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, pos, Quaternion.identity);
        playerEntity.TakeControl();

        PlayerController playerController = playerEntity.GetComponent<PlayerController>();

        if(PlayerPrefs.GetString("username") != null)
        {
            playerController.Setup(PlayerPrefs.GetString("username"));
        }
        else
        {
            playerController.Setup("Player #" + Random.Range(1, 100));
        }
    }
}
