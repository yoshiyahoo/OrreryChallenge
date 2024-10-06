using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject missle;
    public float rotateSpeedDegrees;
    public float rotationLockDecimal;

    public float zDistance = 30f;


    public float rotationSpeed = 100f; // Speed of rotation
    public float verticalLimit = 45f; // Limit for vertical rotation

    private float horizontalRotation = 0f;
    private float verticalRotation = 0f;
    private void Start()
    {
        transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
    void Update()
    {
        Quaternion originalPos = this.transform.rotation;
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(missle, this.transform.position, this.transform.localRotation);
        }

        // Get input from the keyboard
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float verticalInput = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow

        // Update horizontal and vertical rotation
        horizontalRotation += horizontalInput * rotationSpeed * Time.deltaTime;
        verticalRotation -= verticalInput * rotationSpeed * Time.deltaTime;

        // Clamp vertical rotation to prevent flipping
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLimit, verticalLimit);

        // Apply rotation to the turret (camera)
        transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }

}
