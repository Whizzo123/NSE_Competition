using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasUIManager : MonoBehaviour
{

    public Canvas canvas;
    public static GameObject playerNameTextPrefab;
    public GameObject artefactPickupPopup;
    public GameObject popupMessage;
    public GameObject hintMessage;
    public PlayerInventoryUI inventoryUI;
    public ScoreboardUI scoreboardUI;
    public float popupShowTime;
    public float popupMessageShowTime;
    public GameObject playerTextContainer;
    public GameObject loadoutScreen;
    public GameObject targetIconGO;
    public PlayerTrackIconUI playerTrackIcon;
    public WinScreenUI winScreen;
    public Text TimeText;
    public Text loadoutTimeText;

    public void PopupArtefactPickupDisplay(ItemArtefact artefact)
    {
        artefactPickupPopup.SetActive(true);
        artefactPickupPopup.GetComponent<ArtefactPickupPopupUI>().SetMessage(artefact);
        StartCoroutine(PopupCountdown());
    }

    public void ShowHintMessage(string message)
    {
        hintMessage.SetActive(true);
        hintMessage.GetComponent<Text>().text = message;
    }

    public void CloseHintMessage()
    {
        hintMessage.SetActive(false);
    }

    public void PopupMessage(string message)
    {
        popupMessage.SetActive(true);
        popupMessage.GetComponent<Text>().text = message;
        StartCoroutine(PopupMessageCountdown());
    }

    private IEnumerator PopupMessageCountdown()
    {
        float remainingTime = popupMessageShowTime;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
        }

        popupMessage.SetActive(false);
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

    public void SetTimeText(int timeLeft)
    {
        int mins = timeLeft / 60;
        int seconds = timeLeft % 60;
        TimeText.text = mins + ":" + seconds;
    }
}
