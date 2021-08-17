using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// The containing class for <see cref="AbilitySlotUI"/>. Has functionality for adding, removing and setting slot states.
/// </summary>
public class AbilitySlotBarUI : MonoBehaviour
{
    [SerializeField][Tooltip("All the AbilitySlotUI Elements contained")]private AbilitySlotUI[] slots;

    void Start()
    {
        slots = transform.GetComponentsInChildren<AbilitySlotUI>();
    }

    /// <summary>
    /// Fills in ability slots with abilities chosen by player, and adds ability to inventory
    /// </summary>
    public void LoadInAbilitiesFromLoadout(List<string> loadout)
    {
        foreach (string loadoutItem in loadout)
        {
            //Adds ability to slot ui
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].SetAbilityName(loadoutItem)) break; else continue;
            }
            //Adds ability to inventory
            NetworkClient.localPlayer.GetComponent<PlayerController>().abilityInventory.AddAbilityToInventory(FindObjectOfType<AbilityRegister>().Clone(loadoutItem));
        }
    }
    /// <summary>
    /// Adds ability to an empty slot
    /// </summary>
    public void AddAbilityToLoadoutBar(string abilityName)
    {
        Ability ability = NetworkClient.localPlayer.GetComponent<PlayerController>().abilityInventory.FindAbility(abilityName);
        if (ability == null)
        {
            foreach (AbilitySlotUI slot in slots)
            {
                if (slot.GetAbilityName() == null || slot.GetAbilityName() == string.Empty)
                {
                    slot.SetAbilityName(abilityName);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Sets slot's name to null, if it matches abilityName.
    /// </summary>
    /// <param name="abilityName"></param>
    public void RemoveItemFromBar(string abilityName)
    {
        GetSlot(abilityName).SetAbilityName(string.Empty);
    }

    /// <summary>
    /// Sets an ability to the parameter 'state' for isCharging bool
    /// </summary>
    public void SetSlotChargingState(string name, bool state)
    {
        GetSlot(name).SetCharging(state);
    }
    public bool GetSlotChargingState(string name)
    {
        return GetSlot(name).IsCharging();
    }
    /// <summary>
    /// Changes ability icon color based on state
    /// </summary>
    public void SetSlotUseState(string name, bool state)
    {
        GetSlot(name).InUse(state);
    }

    /// <summary>
    /// Gets the first slot instance where abilityName matches the ability slot UI name
    /// </summary>
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
