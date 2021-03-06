using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTrackIconUI : MonoBehaviour
{

    private PlayerController target;
    public Image iconImage;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Do stuff to have icon move around screen
        if(target != null)
        {

        }
    }

    public void SetIconTarget(PlayerController player)
    {
        target = player;
        if(target == null)
        {
            iconImage.enabled = false;
        }
        else
        {
            iconImage.enabled = true;
        }
    }
}
