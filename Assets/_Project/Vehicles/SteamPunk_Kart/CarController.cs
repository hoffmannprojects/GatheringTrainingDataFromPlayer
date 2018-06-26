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

    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public float RotationSpeed
    {
        get
        {
            return rotationSpeed;
        }

        set
        {
            rotationSpeed = value;
        }
    }
    #endregion

    protected void Update ()
    {
        TranslationInput = Input.GetAxis("Vertical");
        RotationInput = Input.GetAxis("Horizontal");
        float translation = TranslationInput * Speed * Time.deltaTime;
        float rotation = RotationInput * RotationSpeed * Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
}
