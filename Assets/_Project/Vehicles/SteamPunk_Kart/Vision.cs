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
        const int rayCount = 5;
        // Direction of the first raycast.
        var rayDirection = -transform.right;
        var angleStepSize = 45f;

        var hits = new RaycastHit[rayCount];
        var hitDistances = new float[rayCount];
        var rayColors = new Color[] { Color.green, Color.cyan, Color.blue, Color.yellow, Color.red };

        // Cast rays in angleStepSize degree steps.
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.DrawRay(transform.position, rayDirection * visibleDistance, rayColors[i]);

            if (Physics.Raycast(transform.position, rayDirection, out hits[i], visibleDistance))
            {
                hitDistances[i] = hits[i].distance;
            }
            rayDirection = Quaternion.AngleAxis(angleStepSize, Vector3.up) * rayDirection;
        }
    }
}
