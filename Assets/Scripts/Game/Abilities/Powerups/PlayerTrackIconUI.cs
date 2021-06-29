using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTrackIconUI : MonoBehaviour
{

    private PlayerController target;
    public Image iconImage;
    private float widthOffset;
    private float heightOffset;

    // Use this for initialization
    void Start()
    {
        widthOffset = GetComponent<RectTransform>().sizeDelta.x / 2;
        heightOffset = GetComponent<RectTransform>().sizeDelta.y / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //Do stuff to have icon move around screen
        if(target != null)
        {
            //Vector2 screenPos = //PlayerController.localPlayer.playerCamera.WorldToScreenPoint(target.transform.position);
            //screenPos.x = Mathf.Clamp(screenPos.x, 0 + widthOffset, Screen.width - widthOffset);
            //screenPos.y = Mathf.Clamp(screenPos.y, 0 + heightOffset, Screen.height - heightOffset);
          //  transform.position = screenPos;
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
