using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class CarController : MonoBehaviour 
{
    [SerializeField] private float speed = 100f;
    [SerializeField] private float rotationSpeed = 100.0F;

    #region PROPERTIES
    public float TranslationInput { get; private set; } = 0;
    public float RotationInput { get; private set; } = 0; 
    #endregion

    private void Update ()
    {
        TranslationInput = Input.GetAxis("Vertical");
        RotationInput = Input.GetAxis("Horizontal");
        float translation = TranslationInput * speed * Time.deltaTime;
        float rotation = RotationInput * rotationSpeed * Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    //TODO: Collect inputs and raycast distances as training data to act as input for the ANN.
}
