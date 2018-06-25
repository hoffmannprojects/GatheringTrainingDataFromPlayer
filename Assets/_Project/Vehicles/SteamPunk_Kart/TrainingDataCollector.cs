using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent( typeof(CarController), typeof(Vision) )]
public class TrainingDataCollector : MonoBehaviour 
{
    private StreamWriter trainingDataFile;
    private List<string> collectedTrainingData = new List<string>();
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
        trainingDataFile = File.CreateText(path);
	}

    private void LateUpdate ()
    {
        string trainingSet = null;

        for (int i = 0; i < vision.HitDistances.Length; i++)
        {
            // Make short distances have big values (towards 1) 
            // and long distances have small values (towards 0)
            // in order to make neurons react more to short distances.
            float hitDistanceRounded = 1 - RoundToPointFive(vision.HitDistances[i]);

            trainingSet += hitDistanceRounded + ",";
        }

        trainingSet += RoundToPointFive(carController.TranslationInput) + "," + RoundToPointFive(carController.RotationInput);

        // Collect only new training data.
        if (!collectedTrainingData.Contains(trainingSet))
        {
            collectedTrainingData.Add(trainingSet);
        }
    }

    private void OnApplicationQuit ()
    {
        foreach (var trainingSet in collectedTrainingData)
        {
            trainingDataFile.WriteLine(trainingSet);
        }
        trainingDataFile.Close();
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
