using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilitySlotUI : MonoBehaviour
{
    private string abilityName;
    private bool isEmpty;
    public Color containAbilityColor;
    public Color emptyColor;
    public Color chargingColor;
    public Color inUseColor;
    private bool isCharging;

    void Start()
    {
        isEmpty = true;
        GetComponent<Button>().onClick.AddListener(() => SlotClick());
    }

    public void SlotClick()
    {
        BoltLog.Info("Slot clicking");
        if (!isCharging && PlayerController.localPlayer.state.Mortal == false)
        {
            PlayerController.localPlayer.abilityInventory.ActivateAbility(abilityName);
        }
    }

    public string GetAbilityName()
    {
        return abilityName;
    }

    public void IsCharging(bool charging)
    {
        if(charging)
        {
            GetComponent<Image>().color = chargingColor;
        }
        else
        {
            GetComponent<Image>().color = containAbilityColor;
        }
        isCharging = charging;
    }

    public void InUse(bool use)
    {
        if(use)
        {
            GetComponent<Image>().color = inUseColor;
        }
        else
        {
            GetComponent<Image>().color = chargingColor;
        }
    }

    public bool SetAbilityName(string name)
    {
        if (isEmpty)
        {
            abilityName = name;
            isEmpty = false;
            GetComponent<Image>().color = containAbilityColor;
            return true;
        }
        else if(name == string.Empty)
        {
            abilityName = name;
            isEmpty = true;
            GetComponent<Image>().color = emptyColor;
            return true;
        }
        return false;
        
    }
}
