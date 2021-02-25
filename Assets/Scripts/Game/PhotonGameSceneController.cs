using System.Collections;
using UnityEngine;
using Bolt;

[BoltGlobalBehaviour("GameScene")]
public class PhotonGameSceneController : GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene)
    {
        PlayerController.Spawn();
    }

    void Start()
    {
        if (BoltNetwork.IsServer)
        {
            BoltEntity artefact = BoltNetwork.Instantiate(BoltPrefabs.Artefect);
            artefact.transform.position = new Vector3(1, -1, -16);
            artefact.GetComponent<ArtefactBehaviour>().PopulateData("Mayan Jar", 100);
            artefact.TakeControl();
        }
    }
 

}
