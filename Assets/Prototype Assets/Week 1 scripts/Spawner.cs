using UnityEngine;

/// <summary>
/// Script is instantiated on generators and shoots ray down to instantiate actual objects on ground
/// </summary>
public class Spawner : MonoBehaviour
{
    [SerializeField][Tooltip("Distance spawner is from the ground")] private float raycastDistance = 100f;
    [SerializeField][Tooltip("The angle of the slope where objects won't spawn")][Range(1, 90)] private float slopeAngle = 30f;
    //Might change this into an array of objects - Not needed as objects are passed into functions by map generator
    //[SerializeField] private GameObject[] objectToSpawn;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask indestructables;

    /// <summary>
    /// Shoots ray down from spawn location. 'objectToSpawn'.ref.Spawner is then instantiated at the hit point if the point is ground. It will also align the object to the rotation of the land.
    /// If it finds a indestructable object or the slope is too steep it will return true and not instantiate gameobject.
    /// </summary>
    public bool ObstacleSpawner(GameObject ob)
    {
        //If indestructable object is found, return true.
        RaycastHit hit;
        if ((Physics.Raycast(transform.position, Vector3.down, raycastDistance, indestructables)))
        {
            return true;
        }
        //If ground is found
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, ground))
        {
            //If angle hit is bigger than 'slopeAngle' then return true
            float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(dot product)
            if (rotationAngle > slopeAngle)
            {
                return true;
            }
            else//Spawn obstacles and return false;
            {
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);//Quaternion for orientating the GO to be perpendicular to the ground
                Instantiate(ob, hit.point, spawnRotation * Quaternion.Euler(0, 90, -90));//The extra quaternion is for the temp grass model which did not come with an upright rotation immediately.
                Debug.Log("Grass is has a rotated model. The code reflects this and will apply some extra rotation. Return to 'Spawner.cs' to remove when models have upright rotation");
                return false;
            }


        }
        return false;

    }

    /// <summary>
    /// Shoots ray down from spawn location. 'objectToSpawn'.ref.Spawner is then instantiated at the hit point if the point is ground. It will also align the object to the rotation of the land.
    /// </summary>
    public void ArtefactSpawner(GameObject ob)
    {
        //If ground is hit, spawn artefact
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, ground))
        {
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Instantiate(ob, hit.point, spawnRotation);
        }
        ///////////////Destroy(this.gameobject);
    }

    /// <summary>
    /// Shoots down ray from spawn location. If angle of slope is higher than 30, will not spawn object. Otherwise spawn object.
    /// </summary>
    /// <param name="ob"></param>
    public void IndestructableSpawner(GameObject ob)
    {
        //If ground is hit
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, ground))
        {
            //If angle hit is bigger than 'slopeAngle' then return true
            float rotationAngle = Vector3.Angle(hit.normal, Vector3.up);//angle between true Y and slope normal(dot product)
            if (rotationAngle > slopeAngle)
            {
                Destroy(this.gameObject);
            }
            else //Spawn indestructable
            {
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Instantiate(ob, hit.point, spawnRotation);
                Destroy(this.gameObject);
            }

        }

    }
}
