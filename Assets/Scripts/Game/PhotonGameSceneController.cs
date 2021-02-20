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
}
