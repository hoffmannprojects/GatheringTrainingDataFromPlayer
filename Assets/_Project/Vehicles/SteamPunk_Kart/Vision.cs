using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class Vision : MonoBehaviour 
{
    [SerializeField] private float visibleDistance = 200f;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
        // Raycasts.
        RaycastHit hit;

        // Raycast left.
        float leftDistance = visibleDistance;
        Vector3 direction = -transform.right;

        Debug.DrawRay(transform.position, direction * visibleDistance, Color.green);

        if (Physics.Raycast(transform.position, direction, out hit, visibleDistance))
        {
            leftDistance = hit.distance;
        }

        // Raycast 45 degrees forward-left.
        float forwardLeftDistance = visibleDistance;
        direction = Quaternion.AngleAxis(-45f, Vector3.up) * transform.forward;

        Debug.DrawRay(transform.position, direction * visibleDistance, Color.cyan);

        if (Physics.Raycast(transform.position, direction, out hit, visibleDistance))
        {
            forwardLeftDistance = hit.distance;
        }

        // Raycast forward.
        float forwardDistance = visibleDistance;
        direction = transform.forward;

        Debug.DrawRay(transform.position, direction * visibleDistance, Color.blue);

        if (Physics.Raycast(transform.position, direction, out hit, visibleDistance))
        {
            forwardDistance = hit.distance;
        }

        // Raycast 45 degrees forward-right.
        float forwardRightDistance = visibleDistance;
        direction = Quaternion.AngleAxis(45f, Vector3.up) * transform.forward;

        Debug.DrawRay(transform.position, direction * visibleDistance, Color.yellow);

        if (Physics.Raycast(transform.position, direction, out hit, visibleDistance))
        {
            forwardRightDistance = hit.distance;
        }

        // Raycast right.
        float rightDistance = visibleDistance;
        direction = transform.right;

        Debug.DrawRay(transform.position, direction * visibleDistance, Color.red);

        if (Physics.Raycast(transform.position, direction, out hit, visibleDistance))
        {
            rightDistance = hit.distance;
        }
    }
}
