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

}
