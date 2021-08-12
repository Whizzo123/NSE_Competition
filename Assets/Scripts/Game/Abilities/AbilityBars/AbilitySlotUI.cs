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
    [SerializeField] [Tooltip("Overlay mask")] public Image overlayImage;

    void Start()
    {
        isEmpty = true;
        GetComponent<Button>().onClick.AddListener(() => SlotClick());
        overlayImage.fillAmount = 0;
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
    public void SetCharging(bool charging)
    {
        if (charging)
        {
            overlayImage.fillAmount = 1;
        }
        else
        {
            overlayImage.fillAmount = 0;
        }
        isCharging = charging;
    }

    public bool IsCharging()
    {
        return isCharging;
    }

    private void Update()
    {
        //If we are charging
        if(isCharging)
        { 
            //Grab charge values from local player
            float currentCharge = NetworkClient.localPlayer.GetComponent<PlayerController>().abilityInventory.FindAbility(abilityName).GetCurrentCharge();
            float maxCharge = NetworkClient.localPlayer.GetComponent<PlayerController>().abilityInventory.FindAbility(abilityName).GetChargeAmount();
            //Calculate percentage
            float percentage = currentCharge / maxCharge;
            //Set fill amount as 1 - the percentage calculated
            overlayImage.fillAmount = 1 - percentage;
        }
    }
    /// <summary>
    /// Changes color of icon if ability whether it is used or not. //If this is end of August and still doesn't matter remove
    /// </summary>
    public void InUse(bool use)
    {
        /*if (use)
        {
            GetComponent<Image>().color = inUseColor;
        }
        else
        {
            GetComponent<Image>().color = chargingColor;
        }*/
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
            GetComponent<Image>().sprite = Resources.Load("UI/Abilities/blank", typeof(Sprite)) as Sprite;
            return true;
        }
        return false;
        
    }

    public string GetAbilityName()
    {
        return abilityName;
    }
    
}
