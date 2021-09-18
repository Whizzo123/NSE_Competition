using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    //The time we are counting down from or duration of event
    public float timeToCount;
    //The local count through var
    private float currentTime;
    //Whether or not we are currently counting
    public bool counting;
    //Whether this is being based just on time or duration of event
    public bool deltaTimeBased;
    //The image that we are animating for this effect
    private Image timerImage;
    //Colours for rings
    public Color redColor;
    public Color blueColor;
    //Image to display with timer
    public Image timerDisplayImage;

    // Start is called before the first frame update
    public void InitializeDeltaTimeTimer(float time, Sprite spriteForDisplayImage)
    {
        timeToCount = time;
        deltaTimeBased = true;
        counting = false;
        currentTime = 0f;
        timerImage = GetComponent<Image>();
        timerDisplayImage.sprite = spriteForDisplayImage;
    }

    public void InitializeEventTimer(float durationTime, bool badEffect, Sprite spriteForDisplayImage)
    {
        timeToCount = durationTime;
        deltaTimeBased = false;
        counting = false;
        currentTime = 0f;
        timerImage = GetComponent<Image>();
        timerDisplayImage.sprite = spriteForDisplayImage;
        if(badEffect)
            timerImage.color = redColor;
        else
            timerImage.color = blueColor;
    }

    /// <summary>
    /// To begin the animation process
    /// </summary>
    public void StartCount()
    {
        currentTime = 0;
        counting = true;
        if(timerImage == null)
            timerImage = GetComponent<Image>();
        timerImage.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(counting)
        {
            if (deltaTimeBased)
                UpdateDecrementally();
            else
                UpdateUsingVars();
        }
    }

    /// <summary>
    /// Used if just for use with time using Time.deltaTime as the decrementer
    /// </summary>
    private void UpdateDecrementally()
    {
        Debug.Log("UpdateDecrementally");
        timerImage.fillAmount = 1 - (currentTime / timeToCount);
        if (timerImage.fillAmount > 0)
            currentTime += Time.deltaTime;
        else
        {
            counting = false;
            timerImage.fillAmount = 0;
        }
    }

    /// <summary>
    /// Used if counting using external variables
    /// </summary>
    private void UpdateUsingVars()
    {
        Debug.Log("UpdateUsingVars: " + currentTime / timeToCount);
        timerImage.fillAmount = 1 - (currentTime / timeToCount);
        if(timerImage.fillAmount <= 0)
        {
            counting = false;
            timerImage.fillAmount = 0;
        }
    }

    /// <summary>
    /// This is used in conjuction with UpdateUsingVars if we are using an external event
    /// </summary>
    /// <param name="currentTime"></param>
    public void SetCurrentTime(float currentTime)
    {
        this.currentTime = currentTime;
    }
}
