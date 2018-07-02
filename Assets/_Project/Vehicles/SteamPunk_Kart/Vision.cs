using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class Vision : MonoBehaviour
{
    #region FIELDS
    private float visibleDistance = 50f;
    private const int rayCount = 5;
    #endregion

    #region PROPERTIES
    public float[] ProcessedHitDistances { get; private set; } = new float[rayCount];
    public bool ControlledByBrain { get; private set; } = false;
    #endregion

    private void Awake()
    {
        ControlledByBrain = GetComponent<Brain>() ? true : false;
        if (ControlledByBrain) Debug.Log("Controlled by Brain. Raycasting in Update() disabled.");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ControlledByBrain)
        {
            CastRays();
        }
    }

    /// <summary>
    /// Rays are being cast in constant steps starting from the left vector [0].
    /// Resulting hits are stored in an array.
    /// </summary>
    public void CastRays()
    {
        // Direction of the first raycast.
        var nextRayDirection = -transform.right;
        var angleStepSize = 45f;

        var hits = new RaycastHit[rayCount];
        var rayColors = new Color[] { Color.green, Color.cyan, Color.blue, Color.yellow, Color.red };

        // Cast rays in angleStepSize degree steps.
        for (var i = 0; i < hits.Length; i++)
        {
            Debug.DrawRay(transform.position, nextRayDirection * visibleDistance, rayColors[i]);

            ProcessedHitDistances[i] = 0;

            if (Physics.Raycast(transform.position, nextRayDirection, out hits[i], visibleDistance))
            {
                // Normalize to a range between 0 and 1.
                var normalizedHitDistance = hits[i].distance / visibleDistance;

                // Make short distances have big values (towards 1) 
                // and long distances have small values (towards 0)
                // in order to make neurons react more to short distances.
                ProcessedHitDistances[i] = 1 - Helpers.RoundToPointFive(normalizedHitDistance);
            }
            nextRayDirection = Quaternion.AngleAxis(angleStepSize, Vector3.up) * nextRayDirection;
        }
    }
}
