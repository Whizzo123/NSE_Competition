using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace T_Utils
{

public class GenericTimer
{

    public static List<GenericTimer> activeTimerList;
    private static GameObject initGameObject;
    private static void InitIfNeeded()
    {
        if (initGameObject == null)
        {
            initGameObject = new GameObject("GenericTimer_InitGameObject");
            activeTimerList = new List<GenericTimer>();
        }

    }
    public static GenericTimer Create(Action action, float timer,string timerName = null)
    {
        InitIfNeeded();
        GameObject gameObject = new GameObject("GenericTimer", typeof(MonoBehaviourHook));

        GenericTimer genericTimer = new GenericTimer(action, timer, timerName, gameObject);

        gameObject.GetComponent<MonoBehaviourHook>().onUpdate = genericTimer.SelfUpdate;

        activeTimerList.Add(genericTimer);
        return genericTimer;
    }

    private static void RemoveTimer(GenericTimer genericTimer)
    {
        InitIfNeeded();
        activeTimerList.Remove(genericTimer);
    }
    public static void StopTimer(string timerName, bool stopAllTimersWithSameName = true)
    {
        for (int i = 0; i < activeTimerList.Count; i++)
        {
            if (activeTimerList[i].timerName == timerName)
            {
                activeTimerList[i].DestroySelf();
                if (!stopAllTimersWithSameName)
                {
                    return;
                }
                i--;
            }
        }
    }
    private class MonoBehaviourHook : MonoBehaviour
    {
        public Action onUpdate;
        private void Update()
        {
            if (onUpdate != null)
            {
                onUpdate();
            }
        }
    }
    private Action action;
    private float timer;
    private string timerName;
    private GameObject gameObject;
    private bool isDestroyed;

    private GenericTimer(Action action, float timer, string timerName, GameObject gameObject)
    {
        this.action = action;
        this.timer = timer;
        this.timerName = timerName;
        this.gameObject = gameObject;
        isDestroyed = false;
    }


    public void SelfUpdate()
    {
        if (!isDestroyed)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                action();
                DestroySelf();
            }
        }

    }

    private void DestroySelf()
    {
        isDestroyed = true;
        UnityEngine.Object.Destroy(gameObject);   
        RemoveTimer(this);
    }
    
    }
}