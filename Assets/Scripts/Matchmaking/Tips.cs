using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using T_Utils;
using UnityEngine.UI;
public class Tips : MonoBehaviour
{
    [SerializeField] private List<string> _tips = new List<string>();
    int _index = 0;
    [SerializeField] private Text _text;
    [SerializeField][Range(0.0f, 10.0f)] private float timer;

    public void Start()
    {
        _text.text = _tips[_index];
        _index++;
        EnqueNextTip();
    }

    public void ForceNextTip()
    {
        GenericTimer.StopTimer("Tip");
        NextTip();
    }
    private void EnqueNextTip()
    {
        GenericTimer.Create(() => { NextTip(); }, timer, "Tip");//Will throw bomb after 3 seconds, if it hasn't already been used
    }
    private void NextTip()
    {
        EnqueNextTip();
        _text.text = _tips[_index];
        _index++;
        if (_index >= _tips.Count)
        {
            _index = 0;
        }
    }
}
