using System.Collections;
using UnityEngine;
using Bolt;

[BoltGlobalBehaviour("GameScene")]
public class PlayerTestSuite : GlobalEventListener
{
    
    public void InitializeTest()
    {
        PlayerController.localPlayer.AddToInventory("TestArtefact", Random.Range(500, 1000));
    }

}
