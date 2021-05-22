using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySlotBarUI : MonoBehaviour
{

    private AbilitySlotUI[] slots;

    void Start()
    {
        slots = transform.GetComponentsInChildren<AbilitySlotUI>();
    }

    public void LoadInAbilitiesFromLoadout(List<string> loadout)
    {
        foreach (string loadoutItem in loadout)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].SetAbilityName(loadoutItem)) break; else continue;
            }
            PlayerController.localPlayer.abilityInventory.AddAbilityToInventory(FindObjectOfType<AbilityRegister>().Clone(loadoutItem));
        }
    }

    public void SetSlotChargingState(string name, bool state)
    {
        GetSlot(name).IsCharging(state);
    }

    public void RemoveItemFromBar(string abilityName)
    {
        GetSlot(abilityName).SetAbilityName(string.Empty);
    }

    public void SetSlotUseState(string name, bool state)
    {
        GetSlot(name).InUse(state);
    }

    private AbilitySlotUI GetSlot(string abilityName)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetAbilityName() == abilityName)
            {
                return slots[i];
            }
        }
        return null;
    }

}
