using System.Collections;
using UnityEngine;
using Mirror;

public class GameSceneManager : NetworkBehaviour
{

    private bool abilityPickupsSpawned = false;
    private float abilitySpawnTime = 30;
    private float currentAbilitySpawnTime = 0;
    private int counter = 0;
    [SyncVar]
    private bool endedGame = false;


    void Start()
    {
        FindObjectOfType<AudioManager>().ActivateGameMusic();
    }

    void LateUpdate()
    {
        if(FindObjectOfType<Stash>().HasPlayerReachedWinningPointsThreshold() && !endedGame)
        {
            endedGame = true;
            FindObjectOfType<MyLobbyCountdown>().CmdCallGameToFinish();
        }
    }

    private IEnumerator RunAbilityPickupSpawnCountdown()
    {
        yield return new WaitForSeconds(30);
        //Do event stuff y -> -3.5
        if (abilityPickupsSpawned)
            yield return null;
        else
        {
            SpawnAbilityCharges();
        }
    }

    private void SpawnAbilityCharges()
    {
        string[] traps = FindObjectOfType<AbilityRegister>().GetTrapList();
        AbilityPickupSpawn request = AbilityPickupSpawn.Create();
        request.SpawnLocationOne = FindRandomPointOnCircle(new Vector2(4, -20), 14, 90);
        request.AbilityOneName = traps[Random.Range(0, traps.Length)];
        request.SpawnLocationTwo = FindRandomPointOnCircle(new Vector2(4, -20), 14, 180);
        request.AbilityTwoName = traps[Random.Range(0, traps.Length)];
        request.SpawnLocationThree = FindRandomPointOnCircle(new Vector2(4, -20), 14, 270);
        request.AbilityThreeName = traps[Random.Range(0, traps.Length)];
        request.Send();
        abilityPickupsSpawned = true;
    }

    private Vector2 FindRandomPointOnCircle(Vector2 centerCirclePoint, float circleRadius, int angle)
    {
        float x = circleRadius * Mathf.Cos(angle) + centerCirclePoint.x;
        float y = circleRadius * Mathf.Sin(angle) + centerCirclePoint.y;

        return new Vector2(x, y);
    }
}
