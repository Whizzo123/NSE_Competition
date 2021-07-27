using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Original ability icons, has dragging functionality and information
/// </summary>
public class AbilityPickBarIconUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{

    private bool dragging;
    //JoeComment i don't really know what these are used for
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

    /// <summary>
    /// Sets the ability icon ui information and image
    /// </summary>
    public void PopulateAbilityIcon(string name, string description, int points, Sprite imageSprite)
    {
        abilityName = name;
        abilityDescription = description;
        abilityPoints = points;
        GetComponent<Image>().sprite = imageSprite;
    }
    public void SetIconAsPartOfLoadout(bool partOfLoadout)
    {
        iconInLoadout = partOfLoadout;
    }

    #region DRAGS
    /// <summary>
    /// Drags the this(abilityIconUI) with the mouse and sets parent to canvas transform
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("InsideOnBeginDrag");
        
        dragging = true;
        this.transform.SetParent(FindObjectOfType<Canvas>().transform);
    }
    /// <summary>
    /// Drags icon with cursor
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = Input.mousePosition;
    }
    /// <summary>
    /// Adds and removes this gameobject from <see cref="AbilityBarUI"/> and <see cref="LoadoutBarUI"/>. Refunds points.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = RaycastAgainstUIAtCurrentMousePos(eventData);

        foreach (RaycastResult result in results)
        {
            Debug.Log("Result GameObject: " + result.gameObject.name);
            //If on original ability bar, add back to ability bar. If it was in loadout, refund ability and set it as no longer a part of loadout
            //Returning to ability bar
            if (result.gameObject.GetComponent<AbilityPickBarUI>())
            {
                result.gameObject.GetComponent<AbilityPickBarUI>().AddGameObjectToContent(gameObject);
                if(iconInLoadout)
                {
                    FindObjectOfType<LoadoutSelectionBoxUI>().RefundPoints(abilityPoints);
                    SetIconAsPartOfLoadout(false);
                }
            }
            //Adding to loadout selection box
            else if (result.gameObject.GetComponent<LoadoutSelectionBoxUI>())
            {
                //Check whether we have enough points left to add the ability
                if(result.gameObject.GetComponent<LoadoutSelectionBoxUI>().AddGameObjectToContent(gameObject) == false)
                {
                    //If not then add to ability bar instead
                    FindObjectOfType<AbilityPickBarUI>().AddGameObjectToContent(gameObject);
                }
            }
            //Todo: Return ability back with points if not hovering over anything. 
            //Also make sure that the bug where it stays on screen after holding while timer ends is gone
        }
        //JoeComment
        if(this.transform.parent == FindObjectOfType<Canvas>().transform)
        {
            FindObjectOfType<AbilityPickBarUI>().AddGameObjectToContent(gameObject);
            if (iconInLoadout)
            {
                FindObjectOfType<LoadoutSelectionBoxUI>().RefundPoints(abilityPoints);
                SetIconAsPartOfLoadout(false);
            }
        }
    }
    /// <summary>
    /// Returns a list of iu raycasts from mouse position
    /// </summary>
    public List<RaycastResult> RaycastAgainstUIAtCurrentMousePos(PointerEventData eventData)
    {
        GraphicRaycaster m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        eventData.position = Input.mousePosition;
        m_Raycaster.Raycast(eventData, results);
        return results;
    }
    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicking");
    }

}
