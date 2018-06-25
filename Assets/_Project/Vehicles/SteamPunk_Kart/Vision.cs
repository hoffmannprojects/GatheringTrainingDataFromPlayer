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
        CastRays();
    }

    /// <summary>
    /// Rays are being cast in constant steps starting from the left vector [0].
    /// Resulting hits are stored in an array.
    /// </summary>
    private void CastRays ()
    {
        const int numberOfRays = 5;
        // Direction of the first raycast.
        var raycastDirection = -transform.right;
        var angleStepSize = 45f;

        var hits = new RaycastHit[numberOfRays];
        var hitDistances = new float[numberOfRays];
        var raycastColors = new Color[] { Color.green, Color.cyan, Color.blue, Color.yellow, Color.red };

        // Cast rays in angleStepSize degree steps.
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.DrawRay(transform.position, raycastDirection * visibleDistance, raycastColors[i]);

            if (Physics.Raycast(transform.position, raycastDirection, out hits[i], visibleDistance))
            {
                hitDistances[i] = hits[i].distance;
            }
            raycastDirection = Quaternion.AngleAxis(angleStepSize, Vector3.up) * raycastDirection;
        }
    }
}
