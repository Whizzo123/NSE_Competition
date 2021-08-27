using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerToObstacleInteraction : NetworkBehaviour
{
    [Tooltip("Distance forward from the player for the destruction sphere")] public float lengthOfSphere = 2f;
    [Tooltip("Radius of the obstacle destruction sphere cast")] public float radiusOfSphere = 1f;

    [Header("Player options")]
    [SerializeField] [Tooltip("Time delay before destroying another obstacle")] [Range(0, 1)] private float waitTime = 0.05f;
    [Tooltip("If the tools are currently on cooldown")] private bool toolWait = false;

    public LayerMask obstacles;


    private void Update()
    {

        if (!hasAuthority) { return; };

        if (Input.GetKey(KeyCode.Space) && toolWait == false)//&& state.Paralyzed == false)
        {
            GetComponent<PlayerStates>().playerAnim.SetTrigger("Cut");
            StartCoroutine(Hit());
        }

        if (GetComponent<PlayerMovement>().GetFallingVelocity() < -150)
        {
            CmdServerValidateHit();
        }
    }




    #region Obstacle Destruction

    /// <summary>
    /// Gives a delay to the destroying obstacles function 'HitForward()'.
    /// </summary>
    /// <returns></returns>
    [ClientCallback]
    System.Collections.IEnumerator Hit()
    {
        toolWait = true;
        CmdHitForward();
        yield return new WaitForSeconds(waitTime);
        toolWait = false;
    }

    [Command]
    void CmdHitForward()
    {
        HitForward();
    }
    /// <summary>
    /// Destroys obstacles directly in front of player.
    /// </summary>
    [ClientRpc]
    void HitForward()
    {
        FindObjectOfType<AudioManager>().PlaySound("Cut");

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hit;

        //Sends a sphere in front to hit objects
        hit = Physics.SphereCastAll(ray, radiusOfSphere, lengthOfSphere, obstacles);

        foreach (RaycastHit item in hit)
        {
            if (item.transform.GetComponent<ArtefactBehaviour>())
            {
                item.transform.gameObject.GetComponent<ArtefactBehaviour>().EnableForPickup();
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else if (item.transform.GetComponent<AbilityPickup>())
            {
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                item.transform.GetComponent<AbilityPickup>().CmdSetEnabledForPickup(true);
            }
            else
            {
                Destroy(item.transform.gameObject);
            }
        }

    }

    /// <summary>
    /// Used to destroy the obstacles underneath player if player has accidentaly went on top of obstacles
    /// </summary>
    [Command]
    private void CmdServerValidateHit()
    {
        //Validate the logic//Just a reminder that everything right now is pretty much client authorative. Cheating is easily possible.

        RpcHitDown();
    }
    /// <summary>
    /// Destroy obstacles underneath player, still does normal hit behaviour for interactables
    /// </summary>
    [ClientRpc]
    private void RpcHitDown()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hit;
        hit = Physics.SphereCastAll(ray, radiusOfSphere, lengthOfSphere, obstacles);
        foreach (RaycastHit item in hit)
        {
            if (item.transform.GetComponent<ArtefactBehaviour>())
            {
                item.transform.gameObject.GetComponent<ArtefactBehaviour>().EnableForPickup();
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else if (item.transform.GetComponent<AbilityPickup>())
            {
                item.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
                item.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                item.transform.GetComponent<AbilityPickup>().CmdSetEnabledForPickup(true);
            }
            else
            {
                Destroy(item.transform.gameObject);
            }
        }
    }



    #endregion
}
