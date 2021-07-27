using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

/// <summary>
/// The ability slot elements in the ability bar where players can activate abilities. 
/// </summary>
public class AbilitySlotUI : MonoBehaviour
{
    [Header("Information")]
    [SerializeField][Tooltip("Name of ability")]private string abilityName;
    [Header("States")]
    [SerializeField] [Tooltip("Is this slot not occupied by an ability?")] private bool isEmpty;
    [SerializeField] [Tooltip("Ability is on cooldown")] private bool isCharging;
    [Header("Color")]
    [SerializeField] [Tooltip("Color for when on cooldown")] public Color chargingColor;
    [SerializeField] [Tooltip("Color for when ready to use")] public Color inUseColor;

    void Start()
    {
        isEmpty = true;
        GetComponent<Button>().onClick.AddListener(() => SlotClick());
    }

    /// <summary>
    /// Uses ability clicked, if not in cooldown and player isn't affected by Mortal Spell
    /// </summary>
    public void SlotClick()
    {
        //Activate ability if not on cooldown and player isn't affected by 'Mortal Spell' Ability
        PlayerController localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<PlayerController>();
        if (!isCharging && localPlayer.IsMortal() == false)
        {
            localPlayer.abilityInventory.ActivateAbility(abilityName);
        }
    }

    /// <summary>
    /// Changes color of icon if ability is on cooldown. Sets isCharging to parameter charging
    /// </summary>
    /// <param name="charging"></param>
    public void IsCharging(bool charging)
    {
        if (charging)
        {
            GetComponent<Image>().color = chargingColor;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }

        isCharging = charging;
    }
    /// <summary>
    /// Changes color of icon if ability whether it is used or not.
    /// </summary>
    public void InUse(bool use)
    {
        if (use)
        {
            GetComponent<Image>().color = inUseColor;
        }
        else
        {
            GetComponent<Image>().color = chargingColor;
        }
    }

    /// <summary>
    /// Sets the ability icon if it is empty. Takes in name and sets image, and information.
    /// </summary>
    public bool SetAbilityName(string name)
    {
        if (isEmpty)
        {
            abilityName = name;
            isEmpty = false;
            GetComponent<Image>().sprite = Resources.Load("UI/Abilities/" + abilityName, typeof(Sprite)) as Sprite;
            return true;
        }
        else if(name == string.Empty)
        {
            abilityName = name;
            isEmpty = true;
            GetComponent<Image>().sprite = Resources.Load("UI/blank", typeof(Sprite)) as Sprite;
            return true;
        }
        return false;
        
    }

    public string GetAbilityName()
    {
        return abilityName;
    }
    
}
