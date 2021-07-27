using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is where the original abilities are contained. They must be dragged into the loadout bar to be used by the player.
/// <para>Spawns in all abilities in the <see cref="AbilityRegister"/></para>
/// </summary>
public class AbilityPickBarUI : MonoBehaviour
{

    [Tooltip("JoeComment")] public GameObject barContent;
    [Tooltip("JoeComment")]public GameObject abilityIconPrefab;
    [Tooltip("Where all abilities are stored")]private AbilityRegister register;
    
    void Start()
    {
        register = FindObjectOfType<AbilityRegister>();
        register.Initialize();

        SpawnInAbilities();
    }

    /// <summary>
    /// Instatiates all abilites from the <see cref="AbilityRegister"/>.
    /// </summary>
    public void SpawnInAbilities()
    {
        List<Ability> loadoutList = register.GetLoadoutList();

        foreach (Ability ability in loadoutList)
        {
            GameObject go = Instantiate(abilityIconPrefab);
            go.GetComponent<AbilityPickBarIconUI>().PopulateAbilityIcon(ability.GetAbilityName(), ability.GetAbilityDescription(), ability.GetAbilityCost(), Resources.Load("UI/Abilities/" + ability.GetAbilityName(), typeof(Sprite)) as Sprite);
            AddGameObjectToContent(go);
        }
    }
    /// <summary>
    /// Set the parent of 'gameObject' to barContent
    /// </summary>
    public void AddGameObjectToContent(GameObject gameObject)
    {
        gameObject.transform.SetParent(barContent.transform);
    }
}
