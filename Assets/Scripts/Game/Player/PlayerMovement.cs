using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Player options")]
    //Gravity
    [SerializeField] private float playerGravity = -30;
    [SerializeField] [Tooltip("The distance from the player to the ground to check if they're grounded")] private float groundDistance = 1f;
    [Tooltip("Is the player touching the ground")] private bool isGrounded = true;
    [SerializeField] [Tooltip("How fast the player is currently falling by y axis")] private Vector3 playerFallingVelocity;
    [Space]
    //Movement
    public float voodooSpeed = 1f;
    [Tooltip("The current speed of the player")] [SyncVar] public float speed = 10f;
    [Tooltip("The original speed of the player")] readonly public float normalSpeed = 10f;
    [Tooltip("Direction of player movement, by input and physics")] private Vector3 playerMovement = Vector3.zero;
    [Space]

    [Header("LayerMasks and Components")]
    [Tooltip("Ground to check for isGrounded")] [SerializeField] private LayerMask ground;
    [Space]
    [Tooltip("Character controller reference")] private CharacterController playerCharacterController;//See attached()
    public Animator playerAnim;


    public void Awake()
    { 
        playerCharacterController = this.gameObject.GetComponent<CharacterController>();
    }
    private void Start()
    {
        playerAnim = GetComponent<NetworkAnimator>().animator;
    }
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; };

       
            #region MOVEMENT_AND_ANIMATION
            if (isGrounded)
            {
                PlayerInput();
                AnimatePlayer();
            }
        PlayerFalling();
 



            playerCharacterController.Move(playerMovement * speed * voodooSpeed * Time.deltaTime);
            playerCharacterController.Move(playerFallingVelocity * Time.deltaTime);
            PlayerRotation();
            #endregion
    }

    private void PlayerInput()
    {
        playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        playerMovement = GetComponent<PlayerCamera>().GetCamera().transform.TransformDirection(playerMovement);//Allows player to move along camera rotation axis
        if (Input.GetAxisRaw("Vertical") < -0.5)//To stop backwards going up
        {
            playerMovement.y = 0;
        }

        //direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void AnimatePlayer()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {

            playerAnim.SetBool("moving", true);
        }
        else
        {
            playerAnim.SetBool("moving", false);
        }
    }



    void PlayerRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, ground))
        {

            if (playerMovement.x != 0 && playerMovement.z != 0)
            {
                float ta = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg;// + cam.transform.eulerAngles.y;
                Quaternion rot = Quaternion.Euler(0f, ta, 0f);// * slopeQuat.normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);

            }
        }

    }
    private void PlayerFalling()
    {
        //Projects a sphere underneath player to check ground layer
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 2, 0), groundDistance, ground);

        //Player recieves a constant y velocity from gravity
        playerFallingVelocity.y += playerGravity;// * Time.deltaTime;

        //If player is fully grounded then apply some velocity down, this will change the 'floating' period before plummeting.
        if (isGrounded && playerFallingVelocity.y < 0)
        {
            playerFallingVelocity.y = -5f;
        }
    }
    public float GetFallingVelocity()
    {
        return playerFallingVelocity.y;
    }
}


#region Archive

/* Old code for player rotation on surface [ARCHIVE]
Quaternion finalQuat;
Vector3 axisOfRotation = (Vector3.Cross(hit.normal, Vector3.up)).normalized;//gets the axis to rotate on a slope.
float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(for somereason it's negative either way round) (dot product)
Quaternion slopeQuat = Quaternion.identity;// = Quaternion.AngleAxis(-rotationAngle, axisOfRotation);//Quaternion of the rotation necessary to angle the player on a slope

Quaternion lookQuat = Quaternion.LookRotation(direction, Vector3.up);//Quaternion of the direction of player movement

if (rotationAngle < 45)
{
    slopeQuat = Quaternion.AngleAxis(-rotationAngle, axisOfRotation);//Quaternion of the rotation necessary to angle the player on a slope

    //finalQuat = slopeQuat.normalized;// * lookQuat; //Quaternion rotation of look rotation and slope rotation
    //transform.rotation = finalQuat;
}*/
#endregion