using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarUI : MonoBehaviour
{

    public GameObject barContent;
    public GameObject abilityIconPrefab;
    private AbilityRegister register;
    
    void Start()
    {
        register = FindObjectOfType<AbilityRegister>();
        register.Initialize();
        SpawnInAbilities();
    }

    public void SpawnInAbilities()
    {
        List<Ability> loadoutList = register.GetLoadoutList();
        foreach (Ability ability in loadoutList)
        {
            GameObject go = Instantiate(abilityIconPrefab);
            go.GetComponent<AbilityIconUI>().PopulateAbilityIcon(ability.GetAbilityName(), ability.GetAbilityDescription(), ability.GetAbilityCost(), null);
            AddGameObjectToContent(go);
        }
    }
    

    public void AddGameObjectToContent(GameObject gameObject)
    {
        gameObject.transform.SetParent(barContent.transform);
    }
}
