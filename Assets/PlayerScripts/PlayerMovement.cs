using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayerMovementControl()
    {
        //playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        //Vector3 move = (transform.right * playerMovement.x + transform.forward * playerMovement.z);
        ////Projects a sphere underneath player to check ground layer

        ////Player recieves a constant y velocity from gravity
        ////playerFallingVelocity.y += playerGravity;// * Time.deltaTime;

        ////If player is fully grounded then apply some velocity down, this will change the 'floating' period before plummeting.
        ////if (isGrounded && playerFallingVelocity.y < 0)
        ////{
        ////    playerFallingVelocity.y = -5f;
        ////}
        //_rigidbody.MovePosition(transform.position + (move * speed * Time.deltaTime));
        ////move.y = _rigidbody.velocity.y;
        ////_rigidbody.velocity = move * Time.deltaTime;


    }
}
