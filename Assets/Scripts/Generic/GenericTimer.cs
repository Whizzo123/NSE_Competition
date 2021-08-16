using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTimer : MonoBehaviour
{
    private Action timerCallback;
    private float timer;

    private static GenericTimer existant;

    private void Awake()
    {
        if (existant == null) { existant = this; }
        else { Destroy(this.gameObject); return; }

        DontDestroyOnLoad(this.gameObject);
    }

    public void SetTimer(float timer, Action timerCallback)
    {
        this.timer = timer;
        this.timerCallback = timerCallback;
        //StartCoroutine(TimeCount(this.timer, this.timerCallback));
    }

    private IEnumerator TimeCount(float time, Action act)
    {
       yield return new WaitForSeconds(time);
       act();
    }



    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (IsTimerComplete())
            {
                timerCallback();
            }
        }

    }

    private bool IsTimerComplete()
    {
        return timer <= 0f;
    }
}
