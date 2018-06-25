using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class Vision : MonoBehaviour 
{
    [SerializeField] private float visibleDistance = 200f;
    private const int rayCount = 5;

    #region PROPERTIES
    public float[] HitDistances { get; private set; } = new float[rayCount];
    #endregion  
	
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
        // Direction of the first raycast.
        var nextRayDirection = -transform.right;
        var angleStepSize = 45f;

        var hits = new RaycastHit[rayCount];
        var rayColors = new Color[] { Color.green, Color.cyan, Color.blue, Color.yellow, Color.red };

        // Cast rays in angleStepSize degree steps.
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.DrawRay(transform.position, nextRayDirection * visibleDistance, rayColors[i]);

            HitDistances[i] = 0;

            if (Physics.Raycast(transform.position, nextRayDirection, out hits[i], visibleDistance))
            {
                // Importandt!:
                // Normalize to a range between 0 and 1.
                var normalizedHitDistance = hits[i].distance / visibleDistance;

                HitDistances[i] = normalizedHitDistance;
            }
            nextRayDirection = Quaternion.AngleAxis(angleStepSize, Vector3.up) * nextRayDirection;
        }
    }
}
