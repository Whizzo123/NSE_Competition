using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerToObstacleInteraction : NetworkBehaviour
{

    [SerializeField] [Tooltip("Time delay before destroying another obstacle")] [Range(0, 1)] private float waitTime = 0.5f;

    public void Cut(PlayerController controller)
    {
        controller.playerAnim.SetTrigger("Cut");
        StartCoroutine(Hit(controller));
    }

    /// <summary>
    /// Gives a delay to the destroying obstacles function 'HitForward()'.
    /// </summary>
    /// <returns></returns>
    [ClientCallback]
    System.Collections.IEnumerator Hit(PlayerController controller)
    {
        controller.SetToolWait(true);
        CmdHitForward(controller);
        yield return new WaitForSeconds(waitTime);
        controller.SetToolWait(false);
    }

    [Command]
    void CmdHitForward(PlayerController controller)
    {
        HitForward(controller);
    }
    /// <summary>
    /// Destroys obstacles directly in front of player.
    /// </summary>
    [ClientRpc]
    void HitForward(PlayerController controller)
    {
        FindObjectOfType<AudioManager>().PlaySound("Cut");

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hit;

        //Sends a sphere in front to hit objects
        hit = Physics.SphereCastAll(ray, controller.radiusOfSphere, controller.lengthOfSphere, controller.obstacles);

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
    public void CmdServerValidateHit(PlayerController controller)
    {
        //Validate the logic//Just a reminder that everything right now is pretty much client authorative. Cheating is easily possible.

        RpcHitDown(controller);
    }
    /// <summary>
    /// Destroy obstacles underneath player, still does normal hit behaviour for interactables
    /// </summary>
    [ClientRpc]
    public void RpcHitDown(PlayerController controller)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hit;
        hit = Physics.SphereCastAll(ray, controller.radiusOfSphere, controller.lengthOfSphere, controller.obstacles);
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



}
