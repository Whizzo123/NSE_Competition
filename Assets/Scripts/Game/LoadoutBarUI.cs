using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutBarUI : MonoBehaviour
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
        if (currentPointsLeft - gameObject.GetComponent<AbilityIconUI>().abilityPoints >= 0)
        {
            gameObject.transform.SetParent(loadoutContent.transform);
            UpdatePoints(-(gameObject.GetComponent<AbilityIconUI>().abilityPoints));
            gameObject.GetComponent<AbilityIconUI>().SetIconAsPartOfLoadout(true);
            return true;
        }
        return false;
    }

    public int NumberOfLoadoutAbilitiesEquipped()
    {
        return gameObject.GetComponentsInChildren<AbilityIconUI>().Length;
    }

    public void RefundPoints(int pointsToRefund)
    {
        UpdatePoints(pointsToRefund);
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
