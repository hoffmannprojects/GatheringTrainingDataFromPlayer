using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

//TODO: Can the inheritance be avoided and instead a regular CarController used separately?
// This would become a Brain class?
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
        string dataSetFilePath = Application.dataPath + "/_Project/ANN/" + dataSetFileName;

        // An instance of training (row in the dataSet).
        string instance;

        if (File.Exists(dataSetFilePath))
        {
            int instanceCount = File.ReadAllLines(dataSetFilePath).Length;
            StreamReader dataSetFile = File.OpenText(dataSetFilePath);
            var calculatedOutputs = new List<double>();
            var inputs = new List<double>();
            var desiredOutputs = new List<double>();

            for (int i = 0; i < epochs; i++)
            {
                sumSquaredError = 0;

                // Set file pointer to beginning of file whenn looping through epochs.
                dataSetFile.BaseStream.Position = 0;

                // Read one instance (line) at a time until the end of the dataSet.
                while ((instance = dataSetFile.ReadLine()) != null)
                {
                    // Separate each feature (column) of the current instance (row).
                    string[] features = instance.Split(',');

                    // The error we get from a particular instance.
                    // If nothing to be learned, ignore this line.
                    float thisError = 0;

                    // Ignore instances, where no user input was recorded.
                    // They provide no useful information.
                    if (System.Convert.ToDouble(features[5]) != 0 
                        && System.Convert.ToDouble(features[6]) != 0)
                    {
                        inputs.Clear();
                        desiredOutputs.Clear();

                        // Assign the first five features (raycast distances) to inputs.
                        for (int j = 0; j < 5; j++)
                        {
                            inputs.Add(System.Convert.ToDouble(features[j]));
                        }

                        // Assign the remaining two features (user input) to outputs.
                        for (int j = 5; j < 7; j++)
                        {
                            //TODO: Where is Map() defined???
                            double output = Map(0, 1, -1, 1, System.Convert.ToSingle(features[j]));
                            desiredOutputs.Add(output);
                        }

                        // Train the Neural Network.
                        calculatedOutputs = ann.Train(inputs, desiredOutputs);

                        // Calculate individual squaredErrors.
                        float output0ErrorSquared = Mathf.Pow((float)(desiredOutputs[0] - calculatedOutputs[0]), 2);
                        float output1ErrorSquared = Mathf.Pow((float)(desiredOutputs[1] - calculatedOutputs[1]), 2);
                        // Calculate averaged sum of squared errors.
                        thisError = (output0ErrorSquared + output1ErrorSquared) / 2f;
                    }
                    sumSquaredError += thisError;
                }
                // Percentage value.
                trainingProgress = i / epochs;

                // Calculate average sumOfSquaredErrors.
                sumSquaredError /= instanceCount;

                lastSumSquaredError = sumSquaredError;

                yield return null;
            }
        }
        trainingDone = true;
    }

}
