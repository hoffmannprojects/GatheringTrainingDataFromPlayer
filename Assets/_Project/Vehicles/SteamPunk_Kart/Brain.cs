using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Vision))]
public class Brain : MonoBehaviour
{
    [SerializeField] private int epochs = 1000;
    [SerializeField] private string dataSetFileName = "TrainingData.txt";
    [SerializeField] private string weightsFileName = "Speed50_Rotation100_Weights.txt";
    [SerializeField] private bool loadWeightsFromFile = false;

    private bool trainingDone = false;
    private float trainingProgress = 0;
    private double sumSquaredError = 0;
    private double lastSumSquaredError = 1;
    private ANN ann;
    private Text[] debugTexts;
    private Vision vision;
    private CarController carController;
    private string dataSetFolder = "/_Project/ANN/Data/";

    // Use this for initialization
    private void Start()
    {
        InitializeReferences();

        ann = new ANN(5, 2, 1, 10, 0.05);

        if (loadWeightsFromFile)
        {
            trainingDone = true;
        }
        else
        {
            StartCoroutine(LoadTrainingSet());
        }
    }

    private void InitializeReferences()
    {
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
    private IEnumerator LoadTrainingSet()
    {
        string dataSetFilePath = Application.dataPath + dataSetFolder + dataSetFileName;

        // An instance of training (row in the dataSet).
        string instance;

        if (File.Exists(dataSetFilePath))
        {
            Debug.Log("Using training data file found at: " + dataSetFilePath);

            int instanceCount = File.ReadAllLines(dataSetFilePath).Length;
            StreamReader dataSetFile = File.OpenText(dataSetFilePath);
            var calculatedOutputs = new List<double>();
            var inputs = new List<double>();
            var desiredOutputs = new List<double>();

            for (var i = 0; i < epochs; i++)
            {
                sumSquaredError = 0;

                // Set file pointer to beginning of file.
                dataSetFile.BaseStream.Position = 0;

                string currentWeights = ann.PrintWeights();

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
                    // TODO: Fix floating point number comparison!?
                    if (System.Convert.ToDouble(features[5]) != 0
                        && System.Convert.ToDouble(features[6]) != 0)
                    {
                        inputs.Clear();
                        desiredOutputs.Clear();

                        // TODO: Check that training data and inputs are calculated the same way (rounding, normalizing, etc.).
                        // Assign the first five features (raycast distances) to inputs.
                        for (int j = 0; j < 5; j++)
                        {
                            inputs.Add(System.Convert.ToDouble(features[j]));
                        }

                        // Assigns the remaining two features (user input) to outputs.
                        for (int j = 5; j < 7; j++)
                        {
                            double output = Helpers.Map(0, 1, -1, 1, System.Convert.ToSingle(features[j]));
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

                AdaptLearning(currentWeights);

                yield return null;
            }
        }
        else
        {
            Debug.LogError("No training data file found at: " + dataSetFilePath);
        }
        trainingDone = true;

        if (!loadWeightsFromFile)
        {
            SaveWeightsToFile();
        }
    }

    /// <summary>
    /// Adapts alpha value of the Neural Network dynamically. 
    /// Discards the current iteration's results if no improvement was made.
    /// </summary>
    /// <param name="currentWeights"> The weights to be re-loaded, if no improvement was made. </param>
    private void AdaptLearning(string currentWeights)
    {
        if (lastSumSquaredError < sumSquaredError)
        {
            // SumSquaredError hasn't improved over lastSumquaredError.

            ann.LoadWeights(currentWeights);
            // Decrease alpha.
            ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f, 0.01f, 0.9f);
        }
        else
        {
            // SumSquaredError has improved.

            // Increase alpha.
            ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f, 0.01f, 0.9f);
            lastSumSquaredError = sumSquaredError;
        }
    }

    private void Update()
    {
        if (!trainingDone) return;

        List<double> inputs = GetInputsFromRaycasts();

        // TODO: Refactor: Not functional here, but some values need to be passed to Ann.CalcOutput().
        var desiredOutputs = new List<double> { 0, 0 };

        List<double> calculatedOutputs = CalculateOutputs(inputs, desiredOutputs);

        ControlCar(calculatedOutputs);
    }

    private void ControlCar(List<double> calculatedOutputs)
    {
        // Map back from normalized values to GetAxis()-compatible controller values.
        float translationInput = Helpers.Map(-1, 1, 0, 1, (float)calculatedOutputs[0]);
        float rotationInput = Helpers.Map(-1, 1, 0, 1, (float)calculatedOutputs[1]);

        carController.TranslationInput = translationInput;
        carController.RotationInput = rotationInput;
        Debug.LogFormat("Frame {0} - {1}: Input values set in CarController.", Time.frameCount, this);

        carController.MoveCar();
    }

    /// <summary>
    /// Calculate outputs without updating weight values as opposed to Train().
    /// </summary>
    private List<double> CalculateOutputs(List<double> inputs, List<double> desiredOutputs)
    {
        var calculatedOutputs = new List<double>();
        calculatedOutputs = ann.CalcOutput(inputs, desiredOutputs);
        return calculatedOutputs;
    }

    private List<double> GetInputsFromRaycasts()
    {
        vision.CastRays();

        var inputs = new List<double>();
        for (var i = 0; i < vision.ProcessedHitDistances.Length; i++)
        {
            inputs.Add(vision.ProcessedHitDistances[i]);
        }

        return inputs;
    }

    private void OnGUI()
    {
        // Update DebugTexts.
        debugTexts[0].text = "SSE: " + lastSumSquaredError;
        debugTexts[1].text = "Alpha: " + ann.alpha;
        debugTexts[2].text = "Trained: " + trainingProgress.ToString("P");
    }

    private void SaveWeightsToFile()
    {
        string weightsFilePath = Application.dataPath + dataSetFolder + weightsFileName;
        StreamWriter weightsFile = File.CreateText(weightsFilePath);
        weightsFile.WriteLine(ann.PrintWeights());
        weightsFile.Close();
    }

    private void LoadWeightsFromFile()
    {
        string weightsFilePath = Application.dataPath + dataSetFolder + weightsFileName;
        StreamReader weightsFile = File.OpenText(weightsFilePath);

        if (File.Exists(weightsFilePath))
        {
            string line = weightsFile.ReadLine();
            ann.LoadWeights(line);
        }
    }
}
