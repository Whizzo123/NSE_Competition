﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The bridge between the initial place where the abilities are visually stored and the usable bar where players can activate their abilities - the confirmation bar.
/// <para></para>
/// </summary>
public class LoadoutSelectionBoxUI : MonoBehaviour
{
    [Header("Loadout bar")]
    [Tooltip("The box where ability icons are dragged into")] public GameObject loadoutContent;
    [Space]

    [Header("Points")]
    [Tooltip("The text showing how many points are left out of maximum points")] public GameObject pointsText;
    [SerializeField] [Tooltip("The maximum loadout points a player can use")] private int maxiumumPointsAvaliable;
    [SerializeField] [Tooltip("The loadout points left to use")] private int currentPointsLeft;

    void Start()
    {
        //maxiumumPointsAvaliable = 9;//Set in inspector
        currentPointsLeft = maxiumumPointsAvaliable;

        UpdatePoints(0);
    }

    /// <summary>
    /// Adds the ability gameobject to the loadout bar content
    /// <para>Checks for points</para>
    /// </summary>
    public bool AddGameObjectToContent(GameObject gameObject)
    {
        if (currentPointsLeft - gameObject.GetComponent<AbilityPickBarIconUI>().abilityPoints >= 0)
        {
            UpdatePoints(-(gameObject.GetComponent<AbilityPickBarIconUI>().abilityPoints));

            gameObject.transform.SetParent(loadoutContent.transform);
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
    /// <summary>
    /// Updates points value and text, has error checking for going over max or under 0.
    /// </summary>
    private void UpdatePoints(int incrementForPoints)
    {
        currentPointsLeft += incrementForPoints;

        if (currentPointsLeft > maxiumumPointsAvaliable)
        {
            Debug.LogError("How did we get a refund that pushed us over the limit of points?");
            currentPointsLeft = maxiumumPointsAvaliable;
        }
        else if (currentPointsLeft < 0)
        {
            Debug.LogError("How did we add an ability that cost more points than what we had left?");
            currentPointsLeft = 0;
        }

        pointsText.GetComponent<Text>().text = currentPointsLeft + "/" + maxiumumPointsAvaliable + " pts";
    }

}
