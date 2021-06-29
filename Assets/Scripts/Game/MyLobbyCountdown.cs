using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyLobbyCountdown : NetworkBehaviour
{
    [Header("Variables")]
    public float countdownTime;
    private float currentCountdownTime;

    // Update is called once per frame
    void Start()
    {
        currentCountdownTime = countdownTime;
        StartCoroutine(Countdown());
    }

    [Server]
    private IEnumerator Countdown()
    {
        Debug.Log("InsideCountdown");
        var floorTime = Mathf.FloorToInt(countdownTime);

        LoadoutCountdown countdown;
        while (currentCountdownTime > 0)
        {
            yield return null;

            currentCountdownTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(currentCountdownTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                UpdateCountdown(floorTime);
            }
        }
        DisableLoadoutScreen();
    }

    [ClientRpc]
    private void DisableLoadoutScreen()
    {
        FindObjectOfType<AbilitySlotBarUI>().LoadInAbilitiesFromLoadout(FindObjectOfType<LoadoutBarUI>().GetLoadoutForAbilitySlotBar());
        FindObjectOfType<CanvasUIManager>().loadoutScreen.SetActive(false);
        //TODO:- Somehow let local player know that the loadout has been released
    }

    [ClientRpc]
    private void UpdateCountdown(float time)
    {
        FindObjectOfType<CanvasUIManager>().loadoutTimeText.text = "" + time;
    }
}
