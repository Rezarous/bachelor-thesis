using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandEyeCalibration : MonoBehaviour {

    public float[] initialValues;
    private Phone2PC phone;
    // Devive These by 10
    public float x;
    public float y;
    public float z;

    // Devive These by 100
    public float q1;
    public float q2;
    public float q3;
    public float q4;

    private void Start()
    {
        phone = GetComponent<Phone2PC>();
        initialValues = new float[7];
        initialValues[0] = x;
        initialValues[1] = y;
        initialValues[2] = z;
        initialValues[3] = q1;
        initialValues[4] = q2;
        initialValues[5] = q3;
        initialValues[6] = q4;
    }

    private void Update()
    {
        x = initialValues[0] + phone.receivedValues[0] / 10;
        y = initialValues[1] + phone.receivedValues[1] / 10;
        z = initialValues[2] + phone.receivedValues[2] / 10;
        q1 = initialValues[3] + phone.receivedValues[3];
        q2 = initialValues[4] + phone.receivedValues[4];
        q3 = initialValues[5] + phone.receivedValues[5];
        q4 = initialValues[6] + phone.receivedValues[6];
    }

}
