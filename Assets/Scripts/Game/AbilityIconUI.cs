using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityIconUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{

    private bool dragging;
    private string abilityName;
    private string abilityDescription;
    private int abilityPoints;

    void Start()
    {
        dragging = false;
    }

    public void PopulateAbilityIcon(string name, string description, int points, Sprite imageSprite)
    {
        abilityName = name;
        abilityDescription = description;
        abilityPoints = points;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
            }
            else if(result.gameObject.GetComponent<LoadoutBarUI>())
            {
                result.gameObject.GetComponent<LoadoutBarUI>().AddGameObjectToContent(gameObject);
            }
        }
        if(this.transform.parent == FindObjectOfType<Canvas>().transform)
        {
            FindObjectOfType<AbilityBarUI>().AddGameObjectToContent(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

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

}
