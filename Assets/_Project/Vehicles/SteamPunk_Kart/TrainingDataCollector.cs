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
            trainingSet += vision.HitDistances[i] + ",";
        }

        trainingSet += carController.TranslationInput + "," + carController.RotationInput;

        collectedTrainingData.Add(trainingSet);
    }

    private void OnApplicationQuit ()
    {
        foreach (var trainingSet in collectedTrainingData)
        {
            trainingDataFile.WriteLine(trainingSet);
        }
        trainingDataFile.Close();
    }
}
