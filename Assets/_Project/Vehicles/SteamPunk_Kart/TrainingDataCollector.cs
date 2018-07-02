using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CarController), typeof(Vision))]
public class TrainingDataCollector : MonoBehaviour
{
    private StreamWriter dataSetFile = null;
    // The collection of instances of (training) data.
    private List<string> dataSet = new List<string>();
    private Vision vision;
    private CarController carController;
    private string filePath = null;

    // Use this for initialization
    private void Start()
    {
        vision = GetComponent<Vision>();
        Assert.IsNotNull(vision);

        carController = GetComponent<CarController>();
        Assert.IsNotNull(carController);

        filePath = Application.dataPath + "/_Project/ANN/Data/Speed" + carController.Speed + "_Rotation" + carController.RotationSpeed + "_" + "TrainingData.txt";
        // Protect against overwrating an existing file.
        if (!File.Exists(filePath))
        {
            dataSetFile = File.CreateText(filePath);
        }
        else
        {
            Debug.LogWarning("Data set file already exists. Rename or delete before recording new training data.");
        }
    }

    private void LateUpdate()
    {
        CollectNewInstance();
    }

    /// <summary>
    /// Collects a new Instance (row of data) with features (columns of data) in the training set, if data is new.
    /// </summary>
    private void CollectNewInstance()
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

    private void OnApplicationQuit()
    {
        WriteDataSetToFileAndClose();
    }

    private void WriteDataSetToFileAndClose()
    {
        if (dataSetFile != null)
        {
            foreach (var instance in dataSet)
            {
                dataSetFile.WriteLine(instance);
            }
            dataSetFile.Close();
            Debug.Log("New training data written to file.");
        }
        else
        {
            Debug.LogWarning("No data has been written to file.");
        }
    }
}
