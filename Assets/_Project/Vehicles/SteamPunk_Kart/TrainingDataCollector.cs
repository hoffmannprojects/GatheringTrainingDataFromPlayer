using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent( typeof(CarController), typeof(Vision) )]
public class TrainingDataCollector : MonoBehaviour 
{
    private StreamWriter dataSetFile;
    // The collection of instances of (training) data.
    private List<string> dataSet = new List<string>();
    private Vision vision;
    private CarController carController;

	// Use this for initialization
	private void Start () 
	{
        vision = GetComponent<Vision>();
        Assert.IsNotNull(vision);

        carController = GetComponent<CarController>();
        Assert.IsNotNull(carController);

        string path = Application.dataPath + "/_Project/ANN/TrainingData.txt";
        dataSetFile = File.CreateText(path);
	}

    private void LateUpdate ()
    {
        // A row of data in the training set.
        string instance = null;

        // Store hitDistances from raycasts as comma-separated features in the instance.
        for (int i = 0; i < vision.HitDistances.Length; i++)
        {
            // Make short distances have big values (towards 1) 
            // and long distances have small values (towards 0)
            // in order to make neurons react more to short distances.
            float hitDistanceRounded = 1 - RoundToPointFive(vision.HitDistances[i]);

            instance += hitDistanceRounded + ",";
        }

        // Append rounded user input as comma-separated features to the instance.
        instance += RoundToPointFive(carController.TranslationInput) + "," + RoundToPointFive(carController.RotationInput);

        // Collect only new training data.
        if (!dataSet.Contains(instance))
        {
            dataSet.Add(instance);
        }
    }

    private void OnApplicationQuit ()
    {
        foreach (var trainingSet in dataSet)
        {
            dataSetFile.WriteLine(trainingSet);
        }
        dataSetFile.Close();
    }

    /// <summary>
    /// Round to the nearest .5 value.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private float RoundToPointFive (float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2f;
    }
}
