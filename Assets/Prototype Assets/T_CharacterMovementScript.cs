using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region privateVariables
    [Header("PlayerComponents")]

    private Rigidbody playerRigidBody;
    private CharacterController playerCharacterController;
    [Space]

    [Space]

    [Header("Uncategorised")]

    private Vector3 playerMovement;
    public Vector3 playerFallingVelocity;
    [Space]

    #endregion
    #region publicVariables
    [Header("PlayerReferenced")]
    public Transform playerHand;
    [Space]

    [Header("PlayerProperties")]
    [Range(1, 100)]
    public int playerSpeed = 15;
    [Range(-1, -100)]
    public float playerGravity = -10;
    [Range(1, 100)]
    public float playerJumpHeight = 2;
    [Space]

    [Header("PlayerHand")]
    public bool playerHandEmpty = true;
    [Space]

    [Header("PlayerGroundChecks")]
    public Transform playerGroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;



    [Space]
    [Header("NSE PROJECT STUFF")]
    public LayerMask obstacles;
    public int scale = 5;
    public Vector3 dir;

    public GameObject machete;
    public bool pressed = false;
    #endregion

    // Grab components
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerCharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        #region FallingJump
        //Projects a sphere underneath player to check ground layer
        isGrounded = Physics.CheckSphere(playerGroundCheck.position, groundDistance, groundMask);
        
        //Player recieves a constant y velocity from gravity
        playerFallingVelocity.y += playerGravity * Time.deltaTime;

        if(isGrounded && playerFallingVelocity.y < 0)
        {
            playerFallingVelocity.y = -1f;
        }

        //Player can jump
        /*if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerFallingVelocity.y = Mathf.Sqrt(playerJumpHeight * -2 * playerGravity);
        }*/
        #endregion

        #region PlayerMovement
        //Complete player movement
        playerMovement = new Vector3
            (Input.GetAxisRaw("Horizontal") * playerSpeed * Time.deltaTime, playerFallingVelocity.y * Time.deltaTime,
            Input.GetAxisRaw("Vertical") * playerSpeed * Time.deltaTime);


        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }
        playerCharacterController.Move(playerMovement);
        #endregion

        
        if (Input.GetKey(KeyCode.Space) && !pressed)
        {
            //
            pressed = true;
            SwingMachete();
            Debug.Log("Space pressed)");
        }
        if (Input.GetKey(KeyCode.C))
        {

            //HitFoward();
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.15F);


    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(playerGroundCheck.position, groundDistance);
        Gizmos.DrawLine(transform.position, transform.position + dir);
    }

/// <summary>
/// Draws a ray in front of the player to destroy an 'obstacle'.
/// </summary>
/// <param name="scale"></param>
/// <param name="obstacles"></param>
    void HitFoward(int scale, LayerMask obstacles)
    {
        //Once empty is found, we will drop in that area
        RaycastHit hit;
        Ray ray = new Ray(transform.position, dir);

            if (Physics.Raycast(ray,out hit,scale,obstacles))
            {
                //Debug.DrawLine(transform.position, hit.point, Color.red, 500f); //Draws in scene
                //
                //Instantiate(pickedUpObject, originalPosition + direction, transform.rotation); Either play animation or instantiata object. Either have object do collider destroy or physics destroy
                Destroy(hit.transform.gameObject);
            }

    }
    void SwingMachete()
    {
        Debug.Log("In Functions");
        //Instantiate(machete, transform);
        Instantiate(machete, transform.position, Quaternion.identity, transform);

    }
}



