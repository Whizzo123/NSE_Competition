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
    public GameObject loadoutScreen;


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

    public void OnLoadoutReadyButtonClick()
    {
        if(FindObjectOfType<LoadoutBarUI>().NumberOfLoadoutAbilitiesEquipped() > 0)
        {
            //Close screen and deal with equipping of loadouts on player
            loadoutScreen.SetActive(false);
        }
    }

    public void AddToInventoryScreen(ItemArtefact artefact)
    {
        inventoryUI.AddInventoryItem(artefact);
    }

    public void RemoveFromInventoryScreen(ItemArtefact artefact)
    {
        inventoryUI.SubtractInventoryItem(artefact);
    }
}
