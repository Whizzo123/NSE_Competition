using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DescriptionBoxUI : MonoBehaviour
{

    public GameObject nameText;
    public GameObject descriptionText;
    public GameObject pointsText;

    private AbilityIconUI iconUIDisplaying;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool overIconUI = false;
        PointerEventData evntData = new PointerEventData(FindObjectOfType<EventSystem>());
        evntData.position = Input.mousePosition;
        List<RaycastResult> results = RaycastAgainstUIAtCurrentMousePos(evntData);
        foreach(RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<AbilityIconUI>())
            {
                overIconUI = true;
            }
            if (iconUIDisplaying == null)
            {
                if(result.gameObject.GetComponent<AbilityIconUI>())
                {
                    iconUIDisplaying = result.gameObject.GetComponent<AbilityIconUI>();
                    UpdateDisplay();
                    break;
                }
            }
            if(result.gameObject.GetComponent<AbilityIconUI>() != iconUIDisplaying)
            {
                iconUIDisplaying = result.gameObject.GetComponent<AbilityIconUI>();
                UpdateDisplay();
                break;
            }
        }
        if(overIconUI && GetComponent<Image>().enabled == false)
        {
            SwitchScreen(true);
        }
        else if(overIconUI == false && GetComponent<Image>().enabled == true)
        {
            SwitchScreen(false);
            iconUIDisplaying = null;
        }
        this.transform.position = new Vector2(Input.mousePosition.x - GetComponent<RectTransform>().rect.width / 2, Input.mousePosition.y - GetComponent<RectTransform>().rect.height /2);
    }

    private void UpdateDisplay()
    {
        if (iconUIDisplaying != null)
        {
            nameText.GetComponent<Text>().text = iconUIDisplaying.abilityName;
            descriptionText.GetComponent<Text>().text = iconUIDisplaying.abilityDescription;
            pointsText.GetComponent<Text>().text = "" + iconUIDisplaying.abilityPoints + " pts";
        }
    }

    public void SwitchScreen(bool switchTo)
    {
        nameText.SetActive(switchTo);
        descriptionText.SetActive(switchTo);
        pointsText.SetActive(switchTo);
        GetComponent<Image>().enabled = switchTo;
    }

    public List<RaycastResult> RaycastAgainstUIAtCurrentMousePos(PointerEventData eventData)
    {
        GraphicRaycaster m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        eventData.position = Input.mousePosition;
        m_Raycaster.Raycast(eventData, results);
        return results;
    }
}
