using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelectionBoxUI : MonoBehaviour
{

    public GameObject loadoutContent;
    public GameObject pointsText;
    private int maxiumumPointsAvaliable;
    private int currentPointsLeft;

    void Start()
    {
        maxiumumPointsAvaliable = 9;
        currentPointsLeft = maxiumumPointsAvaliable;
        UpdatePoints(0);
    }

    public bool AddGameObjectToContent(GameObject gameObject)
    {
        if (currentPointsLeft - gameObject.GetComponent<AbilityPickBarIconUI>().abilityPoints >= 0)
        {
            gameObject.transform.SetParent(loadoutContent.transform);
            UpdatePoints(-(gameObject.GetComponent<AbilityPickBarIconUI>().abilityPoints));
            gameObject.GetComponent<AbilityPickBarIconUI>().SetIconAsPartOfLoadout(true);
            return true;
        }
        return false;
    }

    public int NumberOfLoadoutAbilitiesEquipped()
    {
        return gameObject.GetComponentsInChildren<AbilityPickBarIconUI>().Length;
    }

    public void RefundPoints(int pointsToRefund)
    {
        UpdatePoints(pointsToRefund);
    }

    public List<string> GetLoadoutForAbilitySlotBar()
    {
        AbilityPickBarIconUI[] loadout = transform.GetComponentsInChildren<AbilityPickBarIconUI>();
        List<string> loadoutNames = new List<string>();
        for (int i = 0; i < loadout.Length; i++)
        {
            loadoutNames.Add(loadout[i].abilityName);
        }
        return loadoutNames;
    }

    private void UpdatePoints(int incrementForPoints)
    {
        currentPointsLeft += incrementForPoints;
        if (currentPointsLeft > maxiumumPointsAvaliable)
        {
            Debug.LogError("How did we get a refund that pushed us over the limit of points?");
            currentPointsLeft = maxiumumPointsAvaliable;
        }
        else if(currentPointsLeft < 0)
        {
            Debug.LogError("How did we add an ability that cost more points than what we had left?");
            currentPointsLeft = 0;
        }
        pointsText.GetComponent<Text>().text = currentPointsLeft + "/" + maxiumumPointsAvaliable + " pts";
    }

}
