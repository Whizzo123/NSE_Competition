using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DebuffTargetIcon : MonoBehaviour
{
    private GameObject targetObject;
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
        if(targetObject == null)
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
        if(targetObject != null)
        {
            Vector2 screenPos = NetworkClient.localPlayer.GetComponent<PlayerController>().playerCamera.WorldToScreenPoint(targetObject.transform.position);
            screenPos.x = Mathf.Clamp(screenPos.x, 0 + widthOffset, Screen.width - widthOffset);
            screenPos.y = Mathf.Clamp(screenPos.y, 0 + heightOffset, Screen.height - heightOffset);
            transform.position = screenPos;
        }
        this.transform.Rotate(new Vector3(0, 0, 90 * Time.deltaTime));
    }
}
