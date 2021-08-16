using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereExpansion : MonoBehaviour
{

    [SerializeField] [Tooltip("Using Vector3.Distance() as a measure")] private float distToExpandTo = 50;
    [SerializeField][Tooltip("Time till sphere gets destroyed when fully expanded")] private float wait = 5;
    [SerializeField] [Tooltip("Delay time for expansion")] private readonly float delayInit = 0.02f;
    private float delay = 0.02f;
    public GameObject go;


    Vector3 unitVector = Vector3.one;
    Vector3 currentSizePlus;

    private void Update()
    {
        currentSizePlus = this.transform.lossyScale + unitVector;
        float dist = Vector3.Distance(go.transform.position, this.transform.position);
        if (dist <= distToExpandTo && delay <= -0.01f)
        {
            this.transform.localScale = currentSizePlus;
            delay = delayInit - (0.002f * dist) - 0.002f;
            //Debug.LogWarning(delay);
        }
        else
        {
            delay -= Time.deltaTime;
        }

        if (dist >= distToExpandTo)
        {
            if (wait <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                wait -= Time.deltaTime;
            }
        }
        //Debug.LogError((go.transform.position - this.transform.position).magnitude);
        //Debug.LogError((FindObjectOfType<TempMovement>().transform.position - this.transform.position).magnitude);//Vector3.Distance(this.transform.position, FindObjectOfType<TempMovement>().transform.position)

    }
}
