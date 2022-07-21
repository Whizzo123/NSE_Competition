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
            Vector2 screenPos = playerCamera.WorldToScreenPoint(target.transform.position);
            bool isOffScreen = screenPos.x < widthOffset || screenPos.x > Screen.width - widthOffset || screenPos.y > Screen.height - heightOffset || screenPos.y < heightOffset;
            if (isOffScreen)
            {
                //Outside bounds of screen
                this.transform.localRotation = Quaternion.identity;
                SetImage(pointerImage);
                Vector3 dir = target.transform.position - NetworkClient.localPlayer.transform.position;
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    if (dir.x < 0)
                    {
                        this.transform.position = new Vector3(widthOffset, Screen.height / 2, 0);
                        this.transform.Rotate(new Vector3(0, 0, 90.0f));
                    }
                    else if (dir.x > 0)
                    {
                        this.transform.position = new Vector3(Screen.width - widthOffset, Screen.height / 2, 0);
                        this.transform.Rotate(new Vector3(0, 0, -90.0f));
                    }
                }
                else
                {
                    if (dir.z < 0)
                    {
                        this.transform.position = new Vector3(Screen.width / 2, heightOffset, 0);
                        this.transform.Rotate(new Vector3(0, 0, 180.0f));
                    }
                    else if (dir.z > 0)
                    {
                        this.transform.position = new Vector3(Screen.width / 2, Screen.height - heightOffset, 0);
                    }
                }
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
