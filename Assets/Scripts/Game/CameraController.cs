using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class CameraController : Bolt.EntityBehaviour<ICubeState>
{

    public GameObject followingPlayer;
    private float followingDistance;

    public override void Attached()
    {
        followingDistance = 5f;
    }

    public override void SimulateOwner()
    {
        float distanceToPlayer = Vector3.Distance(this.transform.position, followingPlayer.transform.position);
        if (distanceToPlayer > followingDistance + 1f)
        {
            this.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, followingPlayer.transform.position.x, Time.deltaTime), this.transform.position.y, Mathf.Lerp(this.transform.position.z, followingPlayer.transform.position.z, Time.deltaTime));
        }
        if (distanceToPlayer < followingDistance - 1f)
        {
            float xTarget = ((followingPlayer.transform.position.x - this.transform.position.x) * -1) + this.transform.position.x;
            float zTarget = ((followingPlayer.transform.position.z - this.transform.position.z) * -1) + this.transform.position.z;
            this.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, xTarget, Time.deltaTime), this.transform.position.y, Mathf.Lerp(this.transform.position.z, zTarget, Time.deltaTime));
        }

        this.transform.LookAt(followingPlayer.transform);
    }



}
