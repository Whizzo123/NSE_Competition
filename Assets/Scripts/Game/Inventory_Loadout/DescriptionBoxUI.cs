using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// Shows a description box for the ability hovered over (currently only used for the loadout bars)
/// </summary>
public class DescriptionBoxUI : MonoBehaviour
{
    [Header("Description box elements")]
    [Tooltip("Name of ability")] public GameObject nameText;
    [Tooltip("Description of ability")] public GameObject descriptionText;
    [Tooltip("The point cost of ability")] public GameObject pointsText;

    [Tooltip("The targetted ability icon")]private AbilityIconUI iconUIDisplaying;

    private AbilityPickBarIconUI iconUIDisplaying;

    // Update is called once per frame
    void Update()
    {
        bool overIconUI = false;
        PointerEventData evntData = new PointerEventData(FindObjectOfType<EventSystem>());
        evntData.position = Input.mousePosition;

        List<RaycastResult> results = RaycastAgainstUIAtCurrentMousePos(evntData);

        foreach(RaycastResult result in results)
        {
            //Hover is true,
            if (result.gameObject.GetComponent<AbilityIconUI>())
            {
                overIconUI = true;
            }
            //If we don't have a description box display, display new one
            if (iconUIDisplaying == null)
            {
                if(result.gameObject.GetComponent<AbilityPickBarIconUI>())
                {
                    iconUIDisplaying = result.gameObject.GetComponent<AbilityPickBarIconUI>();
                    UpdateDisplay();
                    break;
                }
            }
            //If we hover over a new ability, display that one
            if(result.gameObject.GetComponent<AbilityIconUI>() != iconUIDisplaying)
            {
                iconUIDisplaying = result.gameObject.GetComponent<AbilityPickBarIconUI>();
                UpdateDisplay();
                break;
            }
        }
        SwitchScreen(overIconUI);
        if (!overIconUI)
            iconUIDisplaying = null;

        this.transform.position = new Vector2(Input.mousePosition.x - GetComponent<RectTransform>().rect.width / 2, Input.mousePosition.y - GetComponent<RectTransform>().rect.height /2);
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

    /// <summary>
    /// Updates the visuals for the selected iconUIDisplaying, if it's not null
    /// </summary>
    private void UpdateDisplay()
    {
        if (iconUIDisplaying != null)
        {
            nameText.GetComponent<Text>().text = iconUIDisplaying.abilityName;
            descriptionText.GetComponent<Text>().text = iconUIDisplaying.abilityDescription;
            pointsText.GetComponent<Text>().text = "" + iconUIDisplaying.abilityPoints + " pts";
        }
    }
    //Todo: Rename function, toggling?
    /// <summary>
    /// Toggles the description box.
    /// </summary>
    public void SwitchScreen(bool switchTo)
    {
        nameText.SetActive(switchTo);
        descriptionText.SetActive(switchTo);
        pointsText.SetActive(switchTo);
        GetComponent<Image>().enabled = switchTo;
    }


}
