using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsSettings : MonoBehaviour
{

    [SerializeField] [Tooltip("Can the camera be moved on the x axis with mouse controls")] public bool xCamMovement = true;
    [SerializeField] [Tooltip("Can the camera be moved on the y axis with mouse controls")] public bool yCamMovement = true;
    [SerializeField] [Tooltip("Whether the player has to press the rmb to use move the camera with mouse")] public bool manualCamera = true;

    [SerializeField] [Tooltip("Whether the player has to press the rmb to use move the camera with mouse")] public float xCamSensitivity = 0.5f;
    [SerializeField] [Tooltip("Whether the player has to press the rmb to use move the camera with mouse")] public float yCamSensitivity =0.5f;

    public void XCamMovement(bool on)
    {
        xCamMovement = on;
    }
    public void YCamMovement(bool on)
    {
        yCamMovement = on;
    }
    public void ManualCamera(bool on)
    {
        manualCamera = on;
    }    
    public void XCamSensitivity(float sensitivity)
    {
        xCamSensitivity = sensitivity;
    }    
    public void YCamSensitivity(float sensitivity)
    {
        yCamSensitivity = sensitivity;
    }
    public void UpdateControls(PlayerController playerObject)
    {
        playerObject.yCamMovementEnabled = yCamMovement;
        playerObject.xCamMovementEnabled = xCamMovement;
        playerObject.xCamSensitivity = xCamSensitivity;
        playerObject.yCamSensitivity = yCamSensitivity;
        playerObject.manualMouseControl = manualCamera;
    }
}
