using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class DebuffTargetIcon : MonoBehaviour
{
    private GameObject targetObject;
    [Tooltip("Image for when target off screen")]public Sprite pointerImage;
    [Tooltip("Image for when target on screen")]public Sprite aimCircleImage;
    private float widthOffset;
    private float heightOffset;
    // Start is called before the first frame update
    void Start()
    {
        widthOffset = GetComponent<RectTransform>().sizeDelta.x / 2;
        heightOffset = GetComponent<RectTransform>().sizeDelta.y / 2;
    }
    public void SetTargetIconObject(GameObject go)
    {
        targetObject = go;
        if (targetObject == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (targetObject != null)
        {
            Camera playerCamera = NetworkClient.localPlayer.GetComponent<PlayerCamera>().GetCamera();
            Vector2 screenPos = playerCamera.WorldToScreenPoint(targetObject.transform.position);
            bool isOffScreen = screenPos.x < widthOffset || screenPos.x > Screen.width - widthOffset || screenPos.y > Screen.height - heightOffset || screenPos.y < heightOffset;
            if (isOffScreen)
            {
                //Outside bounds of screen
                this.transform.localRotation = Quaternion.identity;
                SetImage(pointerImage);
                Vector3 dir = targetObject.transform.position - NetworkClient.localPlayer.transform.position;
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
}

public struct Line
{
    public float gradient;
    public float yIntercept;
}

//screenPos.x = Mathf.Clamp(screenPos.x, 0 + widthOffset, Screen.width - widthOffset);
//screenPos.y = Mathf.Clamp(screenPos.y, 0 + heightOffset, Screen.height - heightOffset);