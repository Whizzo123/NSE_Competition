using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

public class AbilitySlotUI : MonoBehaviour
{
    private string abilityName;
    private bool isEmpty;
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
        PlayerController localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<PlayerController>();
        if (!isCharging && localPlayer.IsMortal() == false)
        {
            localPlayer.abilityInventory.ActivateAbility(abilityName);
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
            GetComponent<Image>().color = Color.white;
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
            GetComponent<Image>().sprite = Resources.Load("UI/" + abilityName, typeof(Sprite)) as Sprite;
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
}
