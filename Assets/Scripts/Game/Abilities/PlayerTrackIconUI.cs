using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerTrackIconUI : MonoBehaviour
{

    private PlayerController target;
    [Tooltip("Image for when target off screen")] public Sprite pointerImage;
    [Tooltip("Image for when target on screen")] public Sprite aimCircleImage;
    public Image imageComponent;
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
            Camera playerCamera = NetworkClient.localPlayer.GetComponent<PlayerController>().playerCamera;
            
            Vector3 screenPos = playerCamera.WorldToScreenPoint(target.transform.position);
            bool isOffScreen = screenPos.x < widthOffset || screenPos.x > Screen.width - widthOffset || screenPos.y > Screen.height - heightOffset || screenPos.y < heightOffset;
           
            if (isOffScreen)
            {
                ///////////////////////////////////////////////////
                if (screenPos.z < 0)
                {
                    screenPos *= -1;
                }
                Debug.LogError(screenPos);

                Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

                screenPos -= screenCenter;

                float angle = Mathf.Atan2(screenPos.y, screenPos.x);
                angle -= 270 * Mathf.Rad2Deg;

                float cos = Mathf.Cos(angle);
                float sin = -Mathf.Sin(angle);

                screenPos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

                float m = cos / sin;

                Vector3 screenBounds = screenCenter * 0.9f;
                Debug.LogWarning(screenPos);

                if (cos > 0)
                {
                    screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
                }
                else
                {
                    screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
                }

                if (screenPos.x > screenBounds.x)
                {
                    screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
                }
                else if (screenPos.x < -screenBounds.x)
                {
                    screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
                }

                //screenPos += screenCenter;
                Debug.Log(screenBounds);
                //screenPos.x = Screen.width / screenPos.x;
                this.transform.localPosition = screenPos;
                this.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                SetImage(pointerImage);

                /////////////////////////////////////////////

                ////Outside bounds of screen
                //this.transform.localRotation = Quaternion.identity;
                //SetImage(pointerImage);
                //Vector3 dir = target.transform.position - NetworkClient.localPlayer.transform.position;
                //if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                //{
                //    if (dir.x < 0)
                //    {
                //        this.transform.position = new Vector3(widthOffset, Screen.height / 2, 0);
                //        this.transform.Rotate(new Vector3(0, 0, 90.0f));
                //    }
                //    else if (dir.x > 0)
                //    {
                //        this.transform.position = new Vector3(Screen.width - widthOffset, Screen.height / 2, 0);
                //        this.transform.Rotate(new Vector3(0, 0, -90.0f));
                //    }
                //}
                //else
                //{
                //    if (dir.z < 0)
                //    {
                //        this.transform.position = new Vector3(Screen.width / 2, heightOffset, 0);
                //        this.transform.Rotate(new Vector3(0, 0, 180.0f));
                //    }
                //    else if (dir.z > 0)
                //    {
                //        this.transform.position = new Vector3(Screen.width / 2, Screen.height - heightOffset, 0);
                //    }
                //}
            }
            else
            {
                //Inside bounds of screen
                SetImage(aimCircleImage);
                transform.position = screenPos;
                this.transform.Rotate(new Vector3(0, 0, 90 * Time.deltaTime));
            }


        }
    }

    private void SetImage(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;
    }

    public void SetIconTarget(PlayerController player)
    {
        target = player;
        if(target == null)
        {
            imageComponent.enabled = false;
        }
        else
        {
            imageComponent.enabled = true;
        }
    }
}
