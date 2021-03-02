﻿using System.Collections;
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

            BoltEntity secondArtefact = BoltNetwork.Instantiate(BoltPrefabs.Artefect);
            secondArtefact.transform.position = new Vector3(-15, -1, 4);
            secondArtefact.GetComponent<ArtefactBehaviour>().PopulateData("Empty Tube Of Toothpaste", 2000);
            secondArtefact.TakeControl();

            BoltEntity stash = BoltNetwork.Instantiate(BoltPrefabs.Stash);
            stash.transform.position = new Vector3(-19 , -1.5f, -20);
            stash.TakeControl();
        }
    }
 

}