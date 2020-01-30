using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarFarmScript : MonoBehaviour {
    [Header("Tilting")]
    [SerializeField] GameObject panels;
    [SerializeField] public float maxTiltAngle = 30f; //desired angle
    [SerializeField] public float minTiltAngle = -30f;
    [Space]
    [SerializeField] public float currentTiltSpeed;
    [SerializeField] public float maxTiltSpeed = 1f;
    [SerializeField] public float minTiltSpeed = 0f;
    [SerializeField] public float accelerateTiltSpeed = .5f;
    [SerializeField] public float decelerateTiltSpeed = 1f;
    [SerializeField] public float decelerateOffset = 10f;
    private float minTiltAngleChecked;


    [Header("Rotation")]
    [SerializeField] GameObject gears;
    [SerializeField] private float currentRotationSpeed;
    [SerializeField] public float maxRotationSpeed = 1f;
    [SerializeField] public float minRotationSpeed = 0f;
    [SerializeField] public float accelerateRotaSpeed = 0.1f;



    private float zAngle;

    // Start is called before the first frame update
    void Start() {

        if (minTiltAngle > maxTiltAngle) { //check for input error
            //Debug.LogWarning("SOLARFARMSCRIPT BuildingTilt Max Angle needs to be lower than Min Angle!");
        }

        if (minTiltAngle <= 0) {//Adjusting for euler angles
            minTiltAngleChecked = minTiltAngle - 360;
        } else {
            minTiltAngleChecked = minTiltAngle;
        }
    }

    // Update is called once per frame
    void Update() {
        BuildingTilt();
        BuildingRotation();
    }

    private void BuildingTilt() {
        if (panels.transform.eulerAngles.x <= maxTiltAngle && panels.transform.eulerAngles.x >= minTiltAngleChecked) { //if between the 

            if (panels.transform.eulerAngles.x + decelerateOffset >= maxTiltAngle) {
                currentTiltSpeed = currentTiltSpeed - (decelerateTiltSpeed * Time.deltaTime); //decelerate
                //Debug.Log("decelerate" + panels.transform.eulerAngles.x + decelerateOffset);

            } else {
            currentTiltSpeed = currentTiltSpeed + (accelerateTiltSpeed * Time.deltaTime); //accelerate
                //Debug.Log("ACCCSELERATE"+ panels.transform.eulerAngles.x + decelerateOffset);
                //Debug.Log("EULER"+ panels.transform.eulerAngles.x);
            }

        currentTiltSpeed = Mathf.Clamp(currentTiltSpeed, minTiltSpeed, maxTiltSpeed);
        panels.transform.Rotate(currentTiltSpeed, 0f, 0f, Space.Self);
        } 
    }

    private void BuildingRotation() {
        currentRotationSpeed = currentRotationSpeed + (accelerateRotaSpeed * Time.deltaTime); //accelerate
        currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
        gears.transform.Rotate(0f, currentRotationSpeed, zAngle, Space.Self);
    }
}
