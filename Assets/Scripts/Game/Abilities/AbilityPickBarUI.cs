using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickBarUI : MonoBehaviour
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
            go.GetComponent<AbilityPickBarIconUI>().PopulateAbilityIcon(ability.GetAbilityName(), ability.GetAbilityDescription(), ability.GetAbilityCost(), Resources.Load("UI/Abilities/" + ability.GetAbilityName(), typeof(Sprite)) as Sprite);
            AddGameObjectToContent(go);
        }
    }
    

    public void AddGameObjectToContent(GameObject gameObject)
    {
        gameObject.transform.SetParent(barContent.transform);
    }
}
