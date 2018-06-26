using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class AnnCarController : CarController 
{
    // VisibleDistance see Vision.cs
    // speed & rotationSpeed are inherited.
    // translation, rotation see parent class.

    [SerializeField] private int epochs = 1000;

    private bool trainingDone = false;
    private float trainingProgress = 0;
    private double sumSquaredError = 0;
    private double lastSumSquaredError = 1;
    private ANN ann;

    // Use this for initialization
    private void Start () 
	{
        ann = new ANN(5, 2, 1, 10, 0.5);

    }

    //TODO: Is the inherited Update() automatically called?
}
