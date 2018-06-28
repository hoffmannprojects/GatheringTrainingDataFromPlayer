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
        CollectNewInstance();
    }

    /// <summary>
    /// Collects a new Instance (row of data) with features (columns of data) in the training set, if data is new.
    /// </summary>
    private void CollectNewInstance ()
    {
        string instance = null;

        // Collect ProcessedhitDistances from raycasts as features.
        for (int i = 0; i < vision.ProcessedHitDistances.Length; i++)
        {
            instance += vision.ProcessedHitDistances[i] + ",";
        }

        // Append rounded user input as features.
        instance += Helpers.RoundToPointFive(carController.TranslationInput) + "," + Helpers.RoundToPointFive(carController.RotationInput);

        // Collect only new training data.
        if (!dataSet.Contains(instance))
        {
            dataSet.Add(instance);
        }
    }

    private void OnApplicationQuit ()
    {
        WriteDataSetToFileAndClose();
    }

    private void WriteDataSetToFileAndClose ()
    {
        foreach (var instance in dataSet)
        {
            dataSetFile.WriteLine(instance);
        }
        dataSetFile.Close();
    }
}
