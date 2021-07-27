using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Todo: JoeComment, this is an almost exact replica of the DescriptionBoxUI class. Could we not use only one of them? And does this get used whatsoever?
/// <summary>
/// Shows a description box for the ability hovered over (currently only used for the Ability slot UI )
/// </summary>
public class AbilitySlotDescriptionBoxUI : MonoBehaviour
{
    [SerializeField][Tooltip("Name of Ability")] public GameObject nameText;

    [SerializeField][Tooltip("Ability being hovered over")]private AbilitySlotUI slotDisplaying;


    // Update is called once per frame
    void Update()
    {
        bool overSlotUI = false;
        PointerEventData evntData = new PointerEventData(FindObjectOfType<EventSystem>());
        evntData.position = Input.mousePosition;
        
        List<RaycastResult> results = RaycastAgainstUIAtCurrentMousePos(evntData);

        foreach (RaycastResult result in results)
        {
            //Hover is true
            if (result.gameObject.GetComponent<AbilitySlotUI>())
            {
                overSlotUI = true;
            }
            //If we don't have a description box display, display new one        
            if (slotDisplaying == null)
            {
                if (result.gameObject.GetComponent<AbilitySlotUI>())
                {
                    slotDisplaying = result.gameObject.GetComponent<AbilitySlotUI>();
                    UpdateDisplay();
                    break;
                }
            }
            //If we hover over a new ability, display that one
            if (result.gameObject.GetComponent<AbilityIconUI>() != slotDisplaying)
            {
                slotDisplaying = result.gameObject.GetComponent<AbilitySlotUI>();
                UpdateDisplay();
                break;
            }
        }
        //JoeComment can we not just say in line 35 SwitchScreen(overIconUI)?
        //Turn on description box if we are hovering
        if (overSlotUI && GetComponent<Image>().enabled == false)
        {
            SwitchScreen(true);
        }
        else if (overSlotUI == false && GetComponent<Image>().enabled == true)
        {
            SwitchScreen(false);
            slotDisplaying = null;
        }
        this.transform.position = new Vector2(Input.mousePosition.x + GetComponent<RectTransform>().rect.width / 2, Input.mousePosition.y - GetComponent<RectTransform>().rect.height / 2);
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
        if (slotDisplaying != null)
        {
            nameText.GetComponent<Text>().text = slotDisplaying.GetAbilityName();
        }
    }
    //Todo: Rename function, toggling?
    /// <summary>
    /// Toggles the description box.
    /// </summary>
    public void SwitchScreen(bool switchTo)
    {
        nameText.SetActive(switchTo);
        GetComponent<Image>().enabled = switchTo;
    }


}
