﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityIconUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{

    private bool dragging;
    public string abilityName { get; private set; }
    public string abilityDescription { get; private set; }
    public int abilityPoints { get; private set; }
    //Set to true when in loadout and if is true once it returns to abilitybar we know to refund points
    public bool iconInLoadout { get; private set; }

    void Start()
    {
        dragging = false;
        //PopulateAbilityIcon("Speed", "this is the best ability", Random.Range(3, 7), null);
    }

    public void PopulateAbilityIcon(string name, string description, int points, Sprite imageSprite)
    {
        abilityName = name;
        abilityDescription = description;
        abilityPoints = points;
        GetComponent<Image>().sprite = imageSprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("InsideOnBeginDrag");
        dragging = true;
        this.transform.SetParent(FindObjectOfType<Canvas>().transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = RaycastAgainstUIAtCurrentMousePos(eventData);
        foreach (RaycastResult result in results)
        {
            Debug.Log("Result GameObject: " + result.gameObject.name);
            if(result.gameObject.GetComponent<AbilityBarUI>())
            {
                result.gameObject.GetComponent<AbilityBarUI>().AddGameObjectToContent(gameObject);
                if(iconInLoadout)
                {
                    FindObjectOfType<LoadoutBarUI>().RefundPoints(abilityPoints);
                    SetIconAsPartOfLoadout(false);
                }
            }
            else if(result.gameObject.GetComponent<LoadoutBarUI>())
            {
                //Check whether we have enough points left to add the ability
                if(result.gameObject.GetComponent<LoadoutBarUI>().AddGameObjectToContent(gameObject) == false)
                {
                    //If not then add to ability bar instead
                    FindObjectOfType<AbilityBarUI>().AddGameObjectToContent(gameObject);
                }
            }
        }
        if(this.transform.parent == FindObjectOfType<Canvas>().transform)
        {
            FindObjectOfType<AbilityBarUI>().AddGameObjectToContent(gameObject);
            if (iconInLoadout)
            {
                FindObjectOfType<LoadoutBarUI>().RefundPoints(abilityPoints);
                SetIconAsPartOfLoadout(false);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicking");
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = Input.mousePosition;
    }

    public List<RaycastResult> RaycastAgainstUIAtCurrentMousePos(PointerEventData eventData)
    {
        GraphicRaycaster m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        eventData.position = Input.mousePosition;
        m_Raycaster.Raycast(eventData, results);
        return results;
    }

    public void SetIconAsPartOfLoadout(bool partOfLoadout)
    {
        iconInLoadout = partOfLoadout;
    }
}
