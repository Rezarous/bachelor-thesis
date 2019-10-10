using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ART2HL : MonoBehaviour
{

    // ART HoloLens marker to Hololens Transformation
    public ART_Receive networkManager;
    public GameObject receiverObj;
    public GameObject transformedART;
    public GameObject artWorld;

    Mean_Calculator mean;
    Vector3 meanPos;
    Quaternion meanRot;

    // Convert the data from ART to HoloLens Coordinate System
    public GameObject holoLensCamera;
    public GameObject hololensCoordinateObject;
    public Vector3 relativePosition;
    public Quaternion relativeRotation;

    void Start()
    {
        mean = GetComponent<Mean_Calculator>();
        meanPos = mean.meanPos;
        meanRot = mean.meanRot;
        print("Mean Pos: " + meanPos.ToString());
        print("Mean Rot: " + meanRot.ToString());
    }

    void Update()
    {
        receiverObj.transform.position = networkManager.finalPosition;
        receiverObj.transform.rotation = networkManager.finalOrientation;
        TransformART();
        DoTheMagic();
    }


    void TransformART()
    {
        transformedART.transform.rotation = receiverObj.transform.rotation * meanRot;
        Vector3 x = receiverObj.transform.right * meanPos.x;
        Vector3 y = receiverObj.transform.up * meanPos.y;
        Vector3 z = receiverObj.transform.forward * meanPos.z;
        print("x: " + x + ",y: " + y + ",z: " + z);
        transformedART.transform.position = receiverObj.transform.position + x + y + z;
    }


    void DoTheMagic()
    {
        MoveRelatively(transformedART.transform, artWorld.transform, holoLensCamera.transform, hololensCoordinateObject.transform);
        RotateRelatively(transformedART.transform, artWorld.transform, holoLensCamera.transform, hololensCoordinateObject.transform);
    }

    void MoveRelatively(Transform dynamicCamera, Transform staticObj, Transform staticCamera, Transform dynamicObj)
    {
        relativePosition = dynamicCamera.InverseTransformDirection(staticObj.position - dynamicCamera.position);
        Vector3 x = staticCamera.right * relativePosition.x;
        Vector3 y = staticCamera.up * relativePosition.y;
        Vector3 z = staticCamera.forward * relativePosition.z;
        dynamicObj.position = staticCamera.position + x + y + z;
    }

    void RotateRelatively(Transform dynamicCamera, Transform staticObj, Transform staticCamera, Transform dynamicObj)
    {
        relativeRotation = Quaternion.Inverse(dynamicCamera.rotation) * staticObj.rotation;
        dynamicObj.rotation = staticCamera.rotation * relativeRotation;
    }
}
