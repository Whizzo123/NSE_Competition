using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilitySlotUI : MonoBehaviour
{
    private string abilityName;
    private bool isEmpty;

    void Start()
    {
        isEmpty = true;
        GetComponent<Button>().onClick.AddListener(() => SlotClick());
    }

    public void SlotClick()
    {
        BoltLog.Info("Slot clicking");
        PlayerController.localPlayer.abilityInventory.ActivateAbility(abilityName);
    }

    public string GetAbilityName()
    {
        return abilityName;
    }

    public bool SetAbilityName(string name)
    {
        if (isEmpty)
        {
            abilityName = name;
            isEmpty = false;
            return true;
        }
        return false;
        
    }
}
