using System.Collections;
using UnityEngine;
using Mirror;

//Todo: Remove the spawning of abilities and put it in it's own script.
/// <summary>
/// Controls the game ending
/// </summary>
public class GameSceneManager : NetworkBehaviour
{
    [SyncVar][Tooltip("Has the game ended via MyLobbyCountdown or Stash")] private bool endedGame = false;
    [SyncVar][Tooltip("")]private bool pickupSpawnCounting = false;


    void Start()
    {
        FindObjectOfType<AudioManager>().ActivateGameMusic();//JoeComment should we activate the music on scene changes, via network manager.
    }

    void LateUpdate()
    {
        //JoeComment the game gets called to end here when the winning threshold is reached yet the game gets called to end in MyLobbyCountdown when the countdown ends
        //Is there a way for us to have them be called in the same script? Is that even more clean?
        if(FindObjectOfType<Stash>().HasPlayerReachedWinningPointsThreshold() && !endedGame)
        {
            endedGame = true;
            FindObjectOfType<MyLobbyCountdown>().CmdCallGameToFinish();
        }
        if(FindObjectOfType<AbilityPickup>() == null)
        {
            if (!pickupSpawnCounting)
            {
                Debug.Log("RunningCoroutine");
                pickupSpawnCounting = true;
                StartCoroutine(RunAbilityPickupSpawnCountdown());
            }
        }
    }

    private IEnumerator RunAbilityPickupSpawnCountdown()
    {
        yield return new WaitForSeconds(30);

        SpawnAbilityCharges();
    }

    private void SpawnAbilityCharges()
    {
        Debug.Log("Spawning Ability Charges");
        string[] traps = FindObjectOfType<AbilityRegister>().GetTrapList();
        SpawnCharge(FindRandomPointOnCircle(new Vector2(4, -20), 14, 90), traps[Random.Range(0, traps.Length)]);
        SpawnCharge(FindRandomPointOnCircle(new Vector2(4, -20), 14, 180), traps[Random.Range(0, traps.Length)]);
        SpawnCharge(FindRandomPointOnCircle(new Vector2(4, -20), 14, 270), traps[Random.Range(0, traps.Length)]);
    }

    private void SpawnCharge(Vector3 spawnPos, string trapName)
    {
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefab => spawnPrefab.name == "AbilityPickup"),
            new Vector3(spawnPos.x, -2f, spawnPos.y), Quaternion.identity);
        NetworkServer.Spawn(go);
        go.GetComponent<AbilityPickup>().SetAbilityOnPickup(trapName);
    }

    private Vector2 FindRandomPointOnCircle(Vector2 centerCirclePoint, float circleRadius, int angle)
    {
        float x = circleRadius * Mathf.Cos(angle) + centerCirclePoint.x;
        float y = circleRadius * Mathf.Sin(angle) + centerCirclePoint.y;

        return new Vector2(x, y);
    }
}
