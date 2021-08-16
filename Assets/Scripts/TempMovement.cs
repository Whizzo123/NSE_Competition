using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class TempMovement : MonoBehaviour
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    // Drag & Drop the camera in this field, in the inspector
    public Camera cam;
    public CinemachineFreeLook vCam;
    public Transform cameraTransform;
    private Vector3 moveDirection = Vector3.zero;
    public Camera go;
    public CinemachineFreeLook vo;
    private void Start()
    {
        go = Instantiate(cam);
        vo = Instantiate(vCam);

        vo.Follow = this.gameObject.transform;
        vo.LookAt = this.gameObject.transform;
        cameraTransform = go.transform;
    }
    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = cameraTransform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
        if (moveDirection.x != 0 && moveDirection.z != 0)
        {
            float ta = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;// + cam.transform.eulerAngles.y;
            Quaternion rot = Quaternion.Euler(0f, ta, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);

        }
        if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
        {
            vo.m_XAxis.m_MaxSpeed = 200;
            vo.m_YAxis.m_MaxSpeed = 20;//Adjustable sensitivity
        }
        else
        {
            vo.m_XAxis.m_MaxSpeed = 0;
            vo.m_YAxis.m_MaxSpeed = 0;

        }
        // = cam.transform.eulerAngles.y * Mathf.Rad2Deg;//transform.rotation.eulerAngles.z;
    }
    //public Rigidbody rb;
    //public CharacterController cc;
    //public Camera cam;
    //public CinemachineFreeLook vCam;

    //public GameObject foe;

    //public Vector3 playerMovement;
    //public Vector3 playerFallingVelocity;
    //public Vector3 direction;
    //public bool isGrounded;
    //public LayerMask ground;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    Camera go = Instantiate(cam);
    //    CinemachineFreeLook vo = Instantiate(vCam);

    //    vo.Follow = this.gameObject.transform;
    //    vo.LookAt = this.gameObject.transform;
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //    playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), playerFallingVelocity.y, Input.GetAxisRaw("Vertical")).normalized;
    //    //playerMovement = cam.transform.forward * playerMovement.z + cam.transform.right * playerMovement.x;
    //    playerMovement = cam.transform.TransformDirection(playerMovement);
    //    Debug.Log(playerMovement);
    //    playerMovement.y = 0;
    //    cc.Move(playerMovement * 5 * Time.deltaTime);


    //    if (playerMovement.x != 0)
    //    {

    //    }
    //    direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //    if (direction.y == 0)
    //    {
    //        //playerMovement += 
    //    }


    //    //Quaternion lookQuat = Quaternion.LookRotation(direction , Vector3.up);//Quaternion of the direction of player movement
    //    //transform.rotation = lookQuat;
    //    if (playerMovement.x != 0 && playerMovement.z != 0)
    //    {
    //        float ta = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
    //        Quaternion rot = Quaternion.Euler(0f, ta, 0f);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);

    //    }
    //}
}
