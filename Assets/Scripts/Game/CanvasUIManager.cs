using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bolt;


public class CanvasUIManager : MonoBehaviour
{

    public Canvas canvas;
    public static GameObject playerNameTextPrefab;
    public GameObject artefactPickupPopup;
    public PlayerInventoryUI inventoryUI;
    public ScoreboardUI scoreboardUI;
    public float popupShowTime;
    public GameObject playerTextContainer;


    public void PopupArtefactPickupDisplay(ItemArtefact artefact)
    {
        artefactPickupPopup.SetActive(true);
        artefactPickupPopup.GetComponent<ArtefactPickupPopupUI>().SetMessage(artefact);
        StartCoroutine(PopupCountdown());
    }

    private IEnumerator PopupCountdown()
    {
        float remainingTime = popupShowTime;

        while(remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
        }

        artefactPickupPopup.SetActive(false);
    }

    public void AddToInventoryScreen(ItemArtefact artefact)
    {
        inventoryUI.AddInventoryItem(artefact);
    }

    public void RemoveFromInventoryScreen(ItemArtefact artefact)
    {
        inventoryUI.SubtractInventoryItem(artefact);
    }

   /* public static void SpawnPlayerNameTextPrefab(PlayerController playerController)
    {
        BoltLog.Info("Inside SpawnPlayerNameTextPrefab");
        if (playerNameTextPrefab == null)
        {
            playerNameTextPrefab = Resources.Load<GameObject>("Prefabs/PlayerNameText");
            if (playerNameTextPrefab == null)
                BoltLog.Error("Resources load failed");
        }

        GameObject go = Instantiate(playerNameTextPrefab);
        go.transform.SetParent(FindObjectOfType<Canvas>().transform);
        go.GetComponent<RectTransform>().position = new Vector3(-18, 271);

        playerController.playerNameText = go;
    }*/
}
