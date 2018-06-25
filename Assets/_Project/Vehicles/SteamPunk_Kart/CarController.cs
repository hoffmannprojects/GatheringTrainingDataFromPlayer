using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class CarController : MonoBehaviour 
{
    [SerializeField] private float speed = 100f;
    [SerializeField] private float rotationSpeed = 100.0F;

    private void Update ()
    {
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");
        float translation = translationInput * speed * Time.deltaTime;
        float rotation = rotationInput * rotationSpeed * Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    //TODO: Collect inputs and raycast distances as training data to act as input for the ANN.
}
