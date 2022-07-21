//using System.Collections;
//using UnityEngine;
//using Mirror;

//public class PlayerMovement : NetworkBehaviour
//{
//    //Vector-data holders
//    [Tooltip("Direction of player movement, by input and physics")] private Vector3 playerMovement = Vector3.zero;
//    [Tooltip("Direction player is moving in by input, not physics")] private Vector3 direction;
//    //Gravity
//    [SerializeField] [Tooltip("How fast the player is currently falling by y axis")] private Vector3 playerFallingVelocity;
//    [SerializeField] private float playerGravity = -65;
//    public LayerMask ground;
//    [SerializeField] [Tooltip("The distance from the player to the ground to check if they're grounded")] private float groundDistance = 2.5f;
//    public bool isGrounded { get; private set; } = true;

//    public void UpdateMovement(PlayerController controller)
//    {
//        if (isGrounded)
//        {
//            playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
//            playerMovement = controller.playerCamera.transform.TransformDirection(playerMovement);//Allows player to move along camera rotation axis
//            if (Input.GetAxisRaw("Vertical") < -0.5)//To stop backwards going up
//            {
//                playerMovement.y = 0;
//            }
//        }



//        //Animations, with movement checks
//        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
//        {
//            direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

//            controller.playerAnim.SetBool("moving", true);

//            //Voodoo poison
//            if (controller.IsVoodooPoisoned())
//            {
//                playerMovement = new Vector3(playerMovement.x * -1, playerMovement.y, playerMovement.z * -1);
//                direction *= -1;
//            }
//        }
//        else
//        {
//            controller.playerAnim.SetBool("moving", false);
//        }

//        #region FALLING

//        //Projects a sphere underneath player to check ground layer
//        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 2, 0), groundDistance, ground);

//        //Player recieves a constant y velocity from gravity
//        playerFallingVelocity.y += playerGravity;// * Time.deltaTime;

//        //If player is fully grounded then apply some velocity down, this will change the 'floating' period before plummeting.
//        if (isGrounded && playerFallingVelocity.y < 0)
//        {
//            playerFallingVelocity.y = -5f;
//        }

//        #endregion

//        controller.playerCharacterController.Move(playerMovement * controller.speed * Time.deltaTime);
//        controller.playerCharacterController.Move(playerFallingVelocity * Time.deltaTime);
//        PlayerRotation(controller);

//        //Todo: Find a different way to stop players from climbing obstacles as this is unreliable
//        if (playerFallingVelocity.y < -200)
//        {
//            GetComponent<PlayerToObstacleInteraction>().CmdServerValidateHit(controller);
//        }
//    }

//    /// <summary>
//    /// Rotates player according to slope and movement direction.
//    /// If we wanted the model to remain upright, we can attach this to a child that isn't visible, but has the same parameters as the player(height etc)
//    /// <para>Primarily used for the purpose of aiming the forward ray when on slopes</para>
//    /// </summary>
//    [ClientCallback]
//    void PlayerRotation(PlayerController controller)
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(transform.position, Vector3.down, out hit, ground))
//        {
//            if (playerMovement.x != 0 && playerMovement.z != 0)
//            {
//                float ta = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg;// + cam.transform.eulerAngles.y;
//                Quaternion rot = Quaternion.Euler(0f, ta, 0f);// * slopeQuat.normalized;
//                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);

//            }
//        }

//        if (Input.GetMouseButton(1))
//        {
//            controller.vCam.m_XAxis.m_MaxSpeed = 200;

//        }
//        else
//        {
//            controller.vCam.m_XAxis.m_MaxSpeed = 0;
//        }
//        if (Input.GetMouseButton(2))
//        {
//            controller.vCam.m_YAxis.m_MaxSpeed = 20;//Adjustable sensitivity
//        }
//        else
//        {
//            controller.vCam.m_YAxis.m_MaxSpeed = 0;
//        }
//    }


//    [Command(requiresAuthority = false)]
//    public void CmdMovePlayer(Vector3 position, string playerName)
//    {
//        transform.position = position;
//        RpcMovePlayer(position, playerName);
//    }

//    [ClientRpc]
//    public void RpcMovePlayer(Vector3 position, string playerName)
//    {
//        if (GetComponent<PlayerController>().playerName == playerName)
//        {
//            transform.position = position;
//        }
//    }
//}
