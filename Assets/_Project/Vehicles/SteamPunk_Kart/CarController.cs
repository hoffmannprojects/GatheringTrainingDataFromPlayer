﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class CarController : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float rotationSpeed = 100.0F;

    #region PROPERTIES
    public float TranslationInput { get; set; } = 0;
    public float RotationInput { get; set; } = 0;
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
    public bool ControlledByBrain { get; private set; } = false;
    #endregion

    private void Awake()
    {
        ControlledByBrain = GetComponent<Brain>() ? true : false;
        if (ControlledByBrain) Debug.Log("Controlled by Brain. User control disabled.");
    }

    protected void Update()
    {
        if (!ControlledByBrain)
        {
            TranslationInput = Input.GetAxis("Vertical");
            RotationInput = Input.GetAxis("Horizontal");

            MoveCar();
        }
    }

    public void MoveCar()
    {
        float translation = TranslationInput * Speed * Time.deltaTime;
        float rotation = RotationInput * RotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
}
