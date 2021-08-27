using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Controls the player camera's(Main camera, FreeLook Camera, and Dev Camera)
/// Contains:
/// <list type="bullet">
/// <listheader>
/// <item><term>Input</term></item><description> Controls the input and response</description>
/// <item><term>Setup</term><description> Sets up all components</description></item>
/// </listheader>
/// </list>
/// </summary>
public class PlayerCamera : NetworkBehaviour
{
    [Header("Components")]
    [Tooltip("Camera attatched to the player, already in the world space")] private Camera playerCamera;
    [Tooltip("Virtual camera controlling the playerCamera, already in the world space")] private Cinemachine.CinemachineFreeLook vCam;
    [Space]

    [Header("DevMode")]
    [Tooltip("Devmode allows us to disconnect ourselves from the player")] private bool devMode;
    [Tooltip("Free look camera")] private GameObject devCam;
    [Space]

    [Header("Sensitivity")]
    [Tooltip("Sensitivity for delta of scrollwheel")] private float scrollSensitivity = 0.5f;
    [SerializeField][Tooltip("Movement sensitivity for mouse X sensitivity")] private float xAxisSensitivity = 200f;
    [SerializeField] [Tooltip("Movement sensitivity for mouse Y sensitivity")] private float yAxisSensitivity = 20f;


    public override void OnStartAuthority()
    {
        //Attatches Camera
        vCam = GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        if (vCam != null)
        {
            Invoke("SetCamera", 2);
        }
        base.OnStartAuthority();
    }

    /// <summary>
    /// Attatches existing cameras, in exception of devcam where we instantiate it.
    /// </summary>
    [Client]
    void SetCamera()
    {
        //Finds Cameras
        vCam = GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        playerCamera = Camera.main;

        //Sets FreeLook camera components
        vCam.LookAt = this.gameObject.transform;
        vCam.Follow = this.gameObject.transform;


        if (!hasAuthority)
        {
            Debug.LogError("Has authority needed");
            //Disable other players cameras so that we don't accidentally get assigned to another players camera
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
        }
        //if (devCam == null)
        //{
        //    //REMINDER, you can't find the object if they are not in the same section ie dontdestroysection
        //    devCam = GameObject.Find("DevCam");
        //    Debug.LogError(devCam);
        //    devCam.SetActive(false);
        //}
        //else
        //{
        devCam = Instantiate(devCam);
        devCam.SetActive(false);
        //}
    }



    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; };

        EnableDevMode();

        CameraRotation();
    }

    private void CameraRotation()
    {

        //If right clicking, allow movement of left and right camera rotation
        if (Input.GetMouseButton(1))
        {
            vCam.m_XAxis.m_MaxSpeed = xAxisSensitivity;
        }
        else
        {
            vCam.m_XAxis.m_MaxSpeed = 0;
        }
        //If Middle mouse clicking, allow movement of up and down camera rotation
        if (Input.GetMouseButton(2))// || Input.mouseScrollDelta.y * scrollSensitivity > 2)
        {
            vCam.m_YAxis.m_MaxSpeed = yAxisSensitivity;
        }
        else
        {
            vCam.m_YAxis.m_MaxSpeed = 0;
        }
    }

    private void EnableDevMode()
    {
        if (Input.GetKey(KeyCode.P)) { devMode = true; }
        if (Input.GetKey(KeyCode.O)) { devMode = false; }
        DevModeOn();
        if (devMode) { return; }
        //Stop other scripts
    }
    private void DevModeOn()
    {
        if (vCam != null) vCam.enabled = !devMode;
        if (playerCamera != null) playerCamera.enabled = !devMode;

        devCam.SetActive(devMode);

        FindObjectOfType<Canvas>().enabled = !devMode;
    }

    #region GETTERS/SETTERS
    public Camera GetCamera()
    {
        return playerCamera;
    }
    public bool GetDevMode()
    {
        return devMode;
    }
    #endregion
}
