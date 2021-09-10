using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTimerContainer : MonoBehaviour
{

    private Dictionary<string, TimerUI> childTimers;
    private GameObject timerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        childTimers = new Dictionary<string, TimerUI>();
        timerPrefab = Resources.Load<GameObject>("UI/Abilities/TimerUI");
    }

    public void AddTimer(string abilityName, float fullDuration, bool badEffect)
    {
        GameObject timer = Instantiate(timerPrefab, this.transform);
        timer.GetComponent<TimerUI>().InitializeEventTimer(fullDuration, badEffect, Resources.Load<Sprite>("UI/Abilities/AbilitiesIcons/" + abilityName));
        timer.GetComponent<TimerUI>().StartCount();
        childTimers.Add(abilityName, timer.GetComponent<TimerUI>());
    }

    private void RemoveTimer(string dictionaryKey)
    {
        Debug.Log("Removing Timer");
        Destroy(childTimers[dictionaryKey].gameObject);
        childTimers.Remove(dictionaryKey);
    }

    void Update()
    {
        List<string> removeKeys = new List<string>();
        foreach (string key in childTimers.Keys)
        {
            if (!childTimers[key].counting)
                removeKeys.Add(key);
        }    
        if(removeKeys.Count > 0)
        {
            foreach (string name in removeKeys)
            {
                RemoveTimer(name);
            }
        }
    }

    public void UpdateTimer(string abilityName, float newDuration)
    {
        if(childTimers.ContainsKey(abilityName) == false)
        {
            Debug.Log("Ability Name: " + abilityName + " is not found count was: " + childTimers.Count);
        }
        else
            childTimers[abilityName].SetCurrentTime(newDuration);
    }

    public bool Contains(string abilityName)
    {
        return childTimers.ContainsKey(abilityName);
    }
}
