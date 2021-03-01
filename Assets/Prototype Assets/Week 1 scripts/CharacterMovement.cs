using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    #region privateVariables
    [Header("PlayerComponents")]
    private Rigidbody playerRigidBody;
    private CharacterController playerCharacterController;
    [Space]

    [Header("PlayerProperties")]
    [Range(1, 100)]
    [SerializeField] private int playerSpeed = 15;
    [Range(-1, -100)]
    [SerializeField] private float playerGravity = -65;
    [Space]
    
    [Header("PlayerGroundChecks")]
    [SerializeField][Tooltip("A GameObject underneath the palyer")] private Transform playerGroundCheck;
    [SerializeField][Tooltip("The radius of a sphere that gets drawn on playerGroundCheck")] private float groundDistance = 0.4f;
    [SerializeField][Tooltip("Floor that player should be able to walk on")] private LayerMask groundMask;
    private bool isGrounded;
    [Space]

    [Space]
    [Header("NSE PROJECT STUFF")]
    [SerializeField][Tooltip("Obstacles should be objects that the player can destroy")] private LayerMask obstacles;
    [SerializeField][Tooltip("Scale is the magnitude of the ray cast to destroy obstacles")] private int scale = 5;
    private Vector3 direction;//direction player is facing based on previous movement
    //[SerializeField]private GameObject machete; //Instantiate gameobject to show visual(machete will need a script or animation
    //[SerializeField] private Animator animation; //Maybe have an animationHandler Script, will play animation, should have no effect on player movement
    //Animation could allow an event where until animation is done, cannot destroy OR use machete again.
    public bool pressed = false;
    [Space]

    [Header("Uncategorised")]
    private Vector3 playerMovement;
    public Vector3 playerFallingVelocity;
    #endregion

    #region publicVariables


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
        #region Falling
        //Projects a sphere underneath player to check ground layer
        isGrounded = Physics.CheckSphere(playerGroundCheck.position, groundDistance, groundMask);
        
        //Player recieves a constant y velocity from gravity
        playerFallingVelocity.y += playerGravity * Time.deltaTime;

        if(isGrounded && playerFallingVelocity.y < 0)
        {
            playerFallingVelocity.y = -1f;
        }
        #endregion

        #region PlayerMovement
        //Complete player movement
        playerMovement = new Vector3
            (Input.GetAxisRaw("Horizontal") * playerSpeed * Time.deltaTime, playerFallingVelocity.y * Time.deltaTime,
            Input.GetAxisRaw("Vertical") * playerSpeed * Time.deltaTime);


        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }
        playerCharacterController.Move(playerMovement);
        #endregion

        PlayerRotation();
        if (Input.GetKey(KeyCode.C))
        {
            HitFoward();
        }


    }


    //If we wanted the model to remain upright, we can attach this to a child that isn't visible, but has the same parameters as the player(height etc)
    #region obstacleDestruction+Slopes


    void PlayerRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundMask))
        {
            Quaternion pop;
            Quaternion qa;
            //transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            Vector3 axisOfRotation = (Vector3.Cross(hit.normal, Vector3.up)).normalized;//gets the axis to rotate on a slope.
            float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(for somereason it's negative either way round)
            Quaternion aA = Quaternion.AngleAxis(-rotationAngle, axisOfRotation);//Quaternion of the rotation necessary to angle the player on a slope
            Quaternion lR = Quaternion.LookRotation(direction, Vector3.up);//Quaternion of the direction of player movement
            pop = aA.normalized * lR;
            transform.rotation = pop;
            Debug.Log(pop + "Pop Quaternion");
            Debug.Log(pop.eulerAngles + "Eular Angle of Pop");
            Debug.Log(aA + "Quaternion of rotation");
            Debug.Log(aA.eulerAngles + "Eular Angles of Rotation");
            Debug.Log(lR + "Quaternion Look Rotation");
            Debug.Log(lR.eulerAngles + "Eular Angle of Look Rotation");
            Debug.Log(rotationAngle + "Rotation Angles");
            qa = Quaternion.Euler(transform.rotation.eulerAngles);
            //Negative directions
            //f = new Vector3(0, 14, -180); THIS is what it needs to be instead, the rotation is (0,180, -14) OR (0, -180, 346) It has the general pattern but it's not it

        }

    }
    /// <summary>
    /// Destroys obstacles directly in front of player
    /// </summary>#
    /// 
    void HitFoward()
    {
        RaycastHit hit;
        //it was just transform.forward for the ray. I fecking hate myself
        Ray ray = new Ray(transform.position, transform.forward);
        //Debug.Log(transform.rotation.eulerAngles.normalized);
        if (Physics.Raycast(ray,out hit,scale,obstacles))
            {
            //Debug.DrawLine(transform.position, hit.point, Color.red, 500f); //Draws in scene
                //Instantiate(pickedUpObject, originalPosition + direction, transform.rotation); Either play animation or instantiata object. Either have object do collider destroy or physics destroy
                Destroy(hit.transform.gameObject);
            //Debug.Log("HIITEITE");
            }
    }
    #endregion

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position + axisOfRotation);//shows axisOfRotation
        //Gizmos.DrawSphere(playerGroundCheck.position, groundDistance);//Shows ground check
        //Gizmos.DrawLine(transform.position, transform.position + direction);//Shows Direction facing in
        //Gizmos.DrawRay(transform.position, transform.position + (direction + transform.up));
        //Gizmos.DrawRay(transform.position, qa.eulerAngles);
        Gizmos.DrawRay(transform.position, transform.forward * scale);
        //Gizmos.DrawRay(transform.position, Vector3.ProjectOnPlane(transform.position, hi);
       // Gizmos.DrawRay(transform.position, f);
    }

}



/*void SwingMachete()
{
    Debug.Log("In Functions");
    //Instantiate(machete, transform);
    Instantiate(machete, transform.position, Quaternion.identity, transform);

}*/
