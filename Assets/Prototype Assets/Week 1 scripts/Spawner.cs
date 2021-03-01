using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField][Tooltip("Distance spawner is from the ground")] private float raycastDistance = 100f;
    //Might change this into an array of objects
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private LayerMask ground;

    void Start()
    {
        PositionRaycast();
    }

    /// <summary>
    /// Shoots ray down from spawn location. 'objectToSpawn'.ref.Spawner is then instantiated at the hit point if the point is ground. It will also align the object to the rotation of the land.
    /// </summary>
    void PositionRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, ground))
        {
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);//Quaternion for orientating the GO to be perpendicular to the ground
            Instantiate(objectToSpawn, hit.point,spawnRotation * Quaternion.Euler(0, 90, -90));//The extra quaternion is for the temp grass model which did not come with an upright rotation immediately.
            Debug.Log("Grass is has a rotated model. The code reflects this and will apply some extra rotation. Return to 'Spawner.cs' to remove when models have upright rotation");
        }
    }
}
