using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityPickupManager : NetworkBehaviour
{
    [SyncVar] [Tooltip("")] private bool pickupSpawnCounting = false;


    void LateUpdate()
    {
        if (FindObjectOfType<AbilityPickup>() == null)
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
