using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent (typeof (Vision))]
public class Brain : MonoBehaviour 
{
    [SerializeField] private int epochs = 1000;
    [SerializeField] private string dataSetFileName = "TrainingData.txt";

    private bool trainingDone = false;
    private float trainingProgress = 0;
    private double sumSquaredError = 0;
    private double lastSumSquaredError = 1;
    private ANN ann;
    private Text[] debugTexts;
    private Vision vision;
    private CarController carController;

    // Use this for initialization
    private void Start () 
	{
        ann = new ANN(5, 2, 1, 10, 0.5);
        StartCoroutine(LoadTrainingSet());

        debugTexts = GameObject.Find("DebugTexts").GetComponentsInChildren<Text>();
        Assert.IsNotNull(debugTexts);

        vision = GetComponent<Vision>();
        Assert.IsNotNull(vision);

        carController = GetComponent<CarController>();
        Assert.IsNotNull(carController);
    }

    /// <summary>
    /// Loads the training dataset.
    /// </summary>
    /// <returns> null (Coroutine). </returns>
    private IEnumerator LoadTrainingSet ()
    {
        string dataSetFilePath = Application.dataPath + "/_Project/ANN/" + dataSetFileName;

        // An instance of training (row in the dataSet).
        string instance;

        if (File.Exists(dataSetFilePath))
        {
            Debug.Log("Training data file found at: " + dataSetFilePath);

            int instanceCount = File.ReadAllLines(dataSetFilePath).Length;
            StreamReader dataSetFile = File.OpenText(dataSetFilePath);
            var calculatedOutputs = new List<double>();
            var inputs = new List<double>();
            var desiredOutputs = new List<double>();

            for (var i = 0; i < epochs; i++)
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

                        // TODO: Check that training data and inputs are calculated the same (rounding, normalizing, etc.).
                        // Assign the first five features (raycast distances) to inputs.
                        for (int j = 0; j < 5; j++)
                        {
                            inputs.Add(System.Convert.ToDouble(features[j]));
                        }

                        // Assign the remaining two features (user input) to outputs.
                        for (int j = 5; j < 7; j++)
                        {
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
                trainingProgress = (float)i / (float)epochs;

                // Calculate average sumOfSquaredErrors.
                sumSquaredError /= instanceCount;

                lastSumSquaredError = sumSquaredError;

                yield return null;
            }
        }
        else
        {
            Debug.LogError("No training data file found at: " + dataSetFilePath);
        }
        trainingDone = true;
    }

    /// <summary>
    /// Maps a value from an original range to a new range.
    /// </summary>
    /// <returns></returns>
    private float Map (float newFrom, float newTo, float originalFrom, float originalTo, float value)
    {
        if (value <= originalFrom)
            return newFrom;
        else if (value >= originalTo)
            return newTo;
        //TODO: See if * and + in next line were transcribed correctly from video.
        return (newTo - newFrom) * ((value - originalFrom) / (originalTo - originalFrom)) + newFrom;
    }

    /// <summary>
    /// Round to the nearest .5 value.
    /// </summary>
    /// <param name="x"> Value to round. </param>
    /// <returns> Value rounded to nearest .5. </returns>
    private float RoundToPointFive (float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2f;
    }

    private void Update ()
    {
        if (!trainingDone) return;

        var inputs = new List<double>();
        for (var i = 0; i < vision.HitDistances.Length; i++)
        {
            inputs.Add(vision.HitDistances[i]);
        }

        // TODO: Refactor: Not functional here, but needs to be provided to the Ann.cs code.
        var desiredOutputs = new List<double> { 0, 0 };

        var calculatedOutputs = new List<double>();
        // Calculates outputs without updating weight values as opposed to Train().
        calculatedOutputs = ann.CalcOutput(inputs, desiredOutputs);

        // Map back from normalized values to GetAxis()-compatible controller values.
        float translationInput = Map(-1, 1, 0, 1, (float)calculatedOutputs[0]);
        float rotationInput = Map(-1, 1, 0, 1, (float)calculatedOutputs[1]);

        carController.TranslationInput = translationInput;
        carController.RotationInput = rotationInput;

    }

    private void OnGUI ()
    {
        // Update DebugTexts.
        debugTexts[0].text = "SSE: " + lastSumSquaredError;
        debugTexts[1].text = "Alpha: " + ann.alpha;
        debugTexts[2].text = "Trained: " + trainingProgress.ToString("P");
    }
}
