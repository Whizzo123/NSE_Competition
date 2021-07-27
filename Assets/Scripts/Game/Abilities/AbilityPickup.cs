using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Todo: Add a gameobject model to show what ability will be picked up, maybe set up ability on spawn instead of on pickup? This would work well for the gamemodel idea
/// <summary>
/// The GameObject for ability pickup. Contains functionality for picking up and setting information.
/// </summary>
public class AbilityPickup : NetworkBehaviour
{
    [SyncVar][SerializeField][Tooltip("The name of the ability to give to the player")]private string abilityName;
    [SyncVar][SerializeField][Tooltip("Can the ability be picked up")]private bool enabledForPickup;
    [Tooltip("Rotation quaternion for visuals")]private Quaternion rotation = Quaternion.Euler(45, 45, 0);

    public void FixedUpdate()
    {
        //Rotate
        rotation *= Quaternion.Euler(2, 2, 2);
        this.gameObject.transform.rotation = rotation;
    }

    #region SETTING_UP_ABILITY
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        CmdSetEnabledForPickup(true);
    }

    public void SetAbilityOnPickup(string name)
    {
        CmdSetAbilityName(name);
        CmdSetEnabledForPickup(true);
    }
    [Command(requiresAuthority = false)]
    public void CmdSetAbilityName(string name)
    {
        abilityName = name;
    }
    [Command(requiresAuthority = false)]
    public void CmdSetEnabledForPickup(bool value)
    {
        enabledForPickup = value;
    }
    #endregion


    public string GetAbilityName()
    {
        return abilityName;
    }
    public bool IsEnabledForPickup()
    {
        return enabledForPickup;
    }

    /// <summary>
    /// Adds ability to player inventory and destroys object if it's enabled for pickup
    /// </summary>
    /// <param name="player"></param>
    public void PickupAbility(PlayerController player)
    {
        if (enabledForPickup)
        {
            FindObjectOfType<AbilitySlotBarUI>().AddAbilityToLoadoutBar(abilityName);
            player.abilityInventory.AddAbilityToInventory(FindObjectOfType<AbilityRegister>().Clone(abilityName));

            NetworkServer.Destroy(this.gameObject);
        }
    }


}
