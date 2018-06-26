using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


public class AnnCarController : CarController 
{
    // VisibleDistance see Vision.cs
    // speed & rotationSpeed are inherited.
    // translation, rotation see parent class.

    [SerializeField] private int epochs = 1000;
    [SerializeField] private string dataSetFileName = "TrainingData.txt";

    private bool trainingDone = false;
    private float trainingProgress = 0;
    private double sumSquaredError = 0;
    private double lastSumSquaredError = 1;
    private ANN ann;
    private Text[] debugTexts;

    // Use this for initialization
    private void Start () 
	{
        ann = new ANN(5, 2, 1, 10, 0.5);
        StartCoroutine(LoadTrainingSet());

        debugTexts = GameObject.Find("DebugTexts").GetComponentsInChildren<Text>();
        Assert.IsNotNull(debugTexts);

        // Might have to go into Update().
        debugTexts[0].text = "SSE: " + lastSumSquaredError;
        debugTexts[1].text = "Alpha: " + ann.alpha;
        debugTexts[2].text = "Trained: " + trainingProgress;
    }

    private IEnumerator LoadTrainingSet ()
    {
        string filePath = Application.dataPath + "/_Project/ANN/" + dataSetFileName;
        string trainingSet;
        //TODO: Implementation...

        yield return 0;
    }
}
