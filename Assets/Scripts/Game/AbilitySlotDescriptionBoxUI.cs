using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilitySlotDescriptionBoxUI : MonoBehaviour
{
    public GameObject nameText;

    private AbilitySlotUI slotDisplaying;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool overSlotUI = false;
        PointerEventData evntData = new PointerEventData(FindObjectOfType<EventSystem>());
        evntData.position = Input.mousePosition;
        List<RaycastResult> results = RaycastAgainstUIAtCurrentMousePos(evntData);
        foreach (RaycastResult result in results)
        {
            Debug.Log("Results Name: " + result.gameObject.name);
            if (result.gameObject.GetComponent<AbilitySlotUI>())
            {
                overSlotUI = true;
            }
            if (slotDisplaying == null)
            {
                if (result.gameObject.GetComponent<AbilitySlotUI>())
                {
                    slotDisplaying = result.gameObject.GetComponent<AbilitySlotUI>();
                    UpdateDisplay();
                    break;
                }
            }
            if (result.gameObject.GetComponent<AbilityIconUI>() != slotDisplaying)
            {
                slotDisplaying = result.gameObject.GetComponent<AbilitySlotUI>();
                UpdateDisplay();
                break;
            }
        }
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

    private void UpdateDisplay()
    {
        if (slotDisplaying != null)
        {
            nameText.GetComponent<Text>().text = slotDisplaying.GetAbilityName();
        }
    }

    public void SwitchScreen(bool switchTo)
    {
        nameText.SetActive(switchTo);
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
