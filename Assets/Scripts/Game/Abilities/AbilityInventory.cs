using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Mirror;

/// <summary>
/// Players abilities that they can use is stored here. The call of ability functionality is here.
/// </summary>
public class AbilityInventory
{
    [Tooltip("Player that has this instance of AbilityInventory")]private PlayerController player;
    
    [Tooltip("Ability list that player can use")]readonly SyncList<Ability> abilities = new SyncList<Ability>();

    [Tooltip("JoeComment why is this necessary")] readonly SyncList<Ability> removeList = new SyncList<Ability>();

    public List<Ability> getAbilities()
    {
        return abilities.ToList<Ability>();
    }

    public AbilityInventory()
    {

    }
    public AbilityInventory(PlayerController playerOwningInventory)
    {
        player = playerOwningInventory;
    }

    #region ADDITION_AND_REMOVAL_OF_ABILITIES
    [Command]
    private void CmdAddToAbilities(Ability ability)
    {
        abilities.Add(ability);
    }
    [Command]
    private void CmdRemoveFromAbilities(Ability ability)
    {
        abilities.Remove(ability);
    }

    [Command]
    private void CmdAddToRemoveList(Ability ability)
    {
        removeList.Add(ability);
    }
    [Command]
    private void CmdRemoveFromRemoveList(Ability ability)
    {
        removeList.Remove(ability);
    }
    /// <summary>
    /// Clears remove list, add 'ability' to remove list and removes the ui
    /// </summary>
    /// <param name="ability"></param>
    public void RemoveAbilityFromInventory(Ability ability)
    {
        removeList.Clear();
        CmdAddToRemoveList(ability);

        GameObject.FindObjectOfType<AbilitySlotBarUI>().RemoveItemFromBar(ability.GetAbilityName());
    }
    #endregion


    public void Update()
    {
        //Remove abilities from Abilities if the exist in remove list
        if (removeList != null)
        {
            foreach (Ability ability in removeList)
            {
                CmdRemoveFromAbilities(ability);
            }
            removeList.Clear();
        }
        
        foreach (Ability ability in abilities)
        {
            ability.UpdateAbility();
        }
    }

    /// <summary>
    /// Uses ability if name matches and there charge is full
    /// </summary>
    public void ActivateAbility(string abilityName)
    {
        foreach (Ability ability in abilities)
        {
            if(ability.GetAbilityName() == abilityName && ability.GetCurrentCharge() == ability.GetChargeAmount())
            {
                ability.Use();
                break;
            }
        }
    }
    /// <summary>
    /// Finds ability if it exists in our inventory
    /// </summary>
    public Ability FindAbility(string abilityName)
    {
        foreach (Ability ability in abilities)
        {
            if (ability.GetAbilityName() == abilityName)
            {
                return ability;
            }
        }
        return null;
    }

    //Todo: Comment this, this will change with the new ability system
    public void AddAbilityToInventory(Ability ability)
    {
        if(ability.GetType().IsSubclassOf(typeof(Powerup)))
        {
            Powerup powerup = (Powerup)ability;
            powerup.SetPlayerToEmpower(NetworkClient.localPlayer.GetComponent<PlayerController>());
            powerup.SetInventory(this);
            CmdAddToAbilities(powerup);
        }
        else if(ability.GetType().IsSubclassOf(typeof(Debuff)))
        {
            Debuff debuff = (Debuff)ability;
            debuff.SetCastingPlayer(NetworkClient.localPlayer.GetComponent<PlayerController>());
            debuff.SetInventory(this);
            CmdAddToAbilities(debuff);
        }
        else if(ability.GetType().IsSubclassOf(typeof(Trap)))
        {
            if (FindAbility(ability.GetAbilityName()) == null)
            {
                Debug.Log("Setting trap player");
                Trap trap = (Trap)ability;
                trap.SetInventory(this);
                trap.SetPlacingPlayer(NetworkClient.localPlayer.GetComponent<PlayerController>());
                CmdAddToAbilities(trap);
            }
            else
            {
                Trap trap = (Trap)FindAbility(ability.GetAbilityName());
                trap.ResetUseCount();
            }
        }
        else
        {
            Debug.LogError("ERROR: ABILITY ATTEMPTING TO BE ADDED TO INVENTORY THAT HAS NO SUBTYPE");
        }
        
    }
}
