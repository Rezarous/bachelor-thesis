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

    // Convert the data from ART to HoloLens Coordinate System
    public GameObject hololensCamera;
    public Vector3 relativePosition;
    public Quaternion relativeRotation;

    // Convert Markers from ART to Hololens SLAM
    public GameObject finalHoloLensWorld;
    public GameObject finalArtWorld;
    public GameObject normalHololensWorld;
    public GameObject secondMarkerReceiver;
    public GameObject finalSecondMarker;
    bool hololensWorldRegistered = false;
    CameraStateDetector cameraState;
    GazeDirection gazeDirection;

    // Update Hologram position
    private int currentPose = 1; // To avoid updating the hologram to (0,0,0)
    private Vector3[] temporaryPoses;
    public int numberOfSamples;
    private Vector3 finalPositionOfHologram;
    private Vector3 previousFinalPosition;
    private float distanceOfcurrentAndPreviousPosition;
    public float firstThreshold;

    // Cubes
    public float cubeZDistance;
    public GameObject cubeART;
    public GameObject cuberArtInHololens;
    public GameObject cubeHololens;
    bool startTheMagic = false;

    // Calibration
    Mean_Calculator mean;
    Vector3 meanPos;
    Quaternion meanRot;
    public HandEyeCalibration handEye;
    Quaternion newMeanRot;
    public Phone2PC phoneData;

    // Adjustment Methods
    public enum adjustmentMotheds {Jump, Lerp, BehindTheBack};
    public adjustmentMotheds adjust;
    public float adjustmentSpeed;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool firstRegistrationDone = false;

    void Start()
    {
        temporaryPoses = new Vector3[numberOfSamples];
        cameraState = hololensCamera.GetComponent<CameraStateDetector>();
        gazeDirection = hololensCamera.GetComponent<GazeDirection>();
        mean = GetComponent<Mean_Calculator>();
        meanPos = mean.meanPos;
        meanRot = mean.meanRot;
        print("Mean Pos: " + meanPos.ToString());
        print("Mean Rot: " + meanRot.ToString());
        // StartCoroutine("WriteTheFirstHologramValue");
    }

    void Update()
    {
        // Dynamic ART
        receiverObj.transform.position = networkManager.finalFirstMarkerPositionSignal;
        receiverObj.transform.rotation = networkManager.finalOrientation;

        // Transform other markers from ART to Hololens Coordinate System
        TransformART();


        // Hololens SLAM initialisation
        if (!hololensWorldRegistered && cameraState.cameraIsStatic && gazeDirection.gazeHit)
        {
            UpdateHologramInHololensSlam();
            hololensWorldRegistered = true;
        }

        // Gathering samples and updating hologram's position
        CollectAndUpdate();
        if (adjust == adjustmentMotheds.Lerp && firstRegistrationDone)
        {
            finalHoloLensWorld.transform.position = Vector3.Lerp(finalHoloLensWorld.transform.position, targetPosition, adjustmentSpeed);
            finalHoloLensWorld.transform.rotation = Quaternion.Lerp(finalHoloLensWorld.transform.rotation, targetRotation, adjustmentSpeed);
        }
        else if (adjust == adjustmentMotheds.BehindTheBack && firstRegistrationDone)
        {
            if (!IsVisibleByHololens(finalHoloLensWorld))
            {
                finalHoloLensWorld.transform.position = targetPosition;
                finalHoloLensWorld.transform.rotation = targetRotation;
            }
        }

        

    }

    void CollectAndUpdate()
    {
        if (cameraState.cameraIsStatic && gazeDirection.gazeHit)
        {
            //print("hit");
            temporaryPoses[currentPose] = finalArtWorld.transform.position;
            currentPose++;
        }

        if(currentPose % numberOfSamples == 0)
        {
            
            //if (previousFinalPosition == Vector3.zero)
            //if(!firstRegistrationDone)
            //{
            //    calculateTheMeanPosition();
            //    //print("First Update");
            //    UpdateFinalHologram(finalPositionOfHologram);
            //    previousFinalPosition = finalPositionOfHologram;
            //}

            calculateTheMeanPosition();
            distanceOfcurrentAndPreviousPosition = Vector3.Distance(previousFinalPosition, finalPositionOfHologram);

            if (distanceOfcurrentAndPreviousPosition > firstThreshold)
            {
                calculateTheMeanPosition();
                //print("Normal Update");
                UpdateFinalHologram(finalPositionOfHologram);
                previousFinalPosition = finalPositionOfHologram;
            }
            currentPose = 0;
        }
    }

    void calculateTheMeanPosition()
    {
        finalPositionOfHologram = Vector3.zero;
        foreach (Vector3 pos in temporaryPoses)
        {
            finalPositionOfHologram += pos / numberOfSamples;
        }
    }

    bool IsVisibleByHololens(GameObject obj)
    {
        Camera cam = hololensCamera.GetComponent<Camera>();
        Vector3 viewPos = cam.WorldToViewportPoint(obj.transform.position);
        if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
        {
            return true;
        }
        return false;
    }

    void UpdateFinalHologram(Vector3 position)
    {
        if (adjust == adjustmentMotheds.Jump)
        {
            finalHoloLensWorld.transform.position = position;
            finalHoloLensWorld.transform.rotation = finalArtWorld.transform.rotation;
        }
        else
        {
            targetPosition = position;
            targetRotation = finalArtWorld.transform.rotation;
        }
    }

    void UpdateHologramInHololensSlam()
    {
        normalHololensWorld.transform.position = finalArtWorld.transform.position;
        normalHololensWorld.transform.rotation = finalArtWorld.transform.rotation;

        finalHoloLensWorld.transform.position = finalArtWorld.transform.position;
        finalHoloLensWorld.transform.rotation = finalArtWorld.transform.rotation;
        //firstRegistrationDone = true;
        previousFinalPosition = finalHoloLensWorld.transform.position;
        StartCoroutine("AllowAdjustment");
    }

    IEnumerator AllowAdjustment()
    {
        yield return new WaitForSeconds(1);
        firstRegistrationDone = true;
    }

    void TransformART()
    {

        float q1 = meanRot.x + handEye.q1/ 100 + phoneData.receivedValues[3] / 100;
        float q2 = meanRot.y + handEye.q2/ 100 + phoneData.receivedValues[4] / 100;
        float q3 = meanRot.z + handEye.q3/ 100 + phoneData.receivedValues[5] / 100;
        float q4 = meanRot.w + handEye.q4/ 100 + phoneData.receivedValues[6] / 100;

        newMeanRot = new Quaternion(q1, q2, q3, q4);
        transformedART.transform.rotation = receiverObj.transform.rotation * newMeanRot;

        Vector3 x = - transformedART.transform.right * (meanPos.x - handEye.x / 10 - phoneData.receivedValues[0] / 10);
        Vector3 y = - transformedART.transform.up * (meanPos.x - handEye.y / 10 - phoneData.receivedValues[1] / 10);
        Vector3 z = - transformedART.transform.forward * (meanPos.x - handEye.z / 10 - phoneData.receivedValues[2] / 10);

        // print("x: " + x + ",y: " + y + ",z: " + z);
        transformedART.transform.position = receiverObj.transform.position + x + y + z;
        DoTheMagic();
    }


    void DoTheMagic()
    {
        MoveRelatively(transformedART.transform, artWorld.transform, hololensCamera.transform, finalArtWorld.transform);
        RotateRelatively(transformedART.transform, artWorld.transform, hololensCamera.transform, finalArtWorld.transform);

        MoveRelatively(transformedART.transform, secondMarkerReceiver.transform, hololensCamera.transform, finalSecondMarker.transform);
        RotateRelatively(transformedART.transform, secondMarkerReceiver.transform, hololensCamera.transform, finalSecondMarker.transform);
    }

    void MoveRelatively(Transform dynamicCamera, Transform staticObj, Transform staticCamera, Transform dynamicObj)
    {
        relativePosition = dynamicCamera.InverseTransformDirection(staticObj.position - dynamicCamera.position);
        Vector3 x = staticCamera.right * relativePosition.x;
        Vector3 y = staticCamera.up * relativePosition.y;
        Vector3 z = staticCamera.forward * relativePosition.z;
        //dynamicObj.position = staticCamera.position + x + y + z;
        dynamicObj.localPosition = staticCamera.position + x + y + z;
    }

    void RotateRelatively(Transform dynamicCamera, Transform staticObj, Transform staticCamera, Transform dynamicObj)
    {
        relativeRotation = Quaternion.Inverse(dynamicCamera.rotation) * staticObj.rotation;
        dynamicObj.rotation = staticCamera.rotation * relativeRotation;
    }

    void SetTheCubeTransformation(float distanceForward)
    {
        cubeART.transform.position = transformedART.transform.position + transformedART.transform.forward * distanceForward;
        cubeHololens.transform.position = hololensCamera.transform.position + hololensCamera.transform.forward * distanceForward;
        cuberArtInHololens.transform.position = hololensCamera.transform.position + hololensCamera.transform.forward * distanceForward;
    }

    void DoTheMagicWithCubes()
    {
        MoveRelatively(transformedART.transform, cubeART.transform, hololensCamera.transform, cuberArtInHololens.transform);
        RotateRelatively(transformedART.transform, cubeART.transform, hololensCamera.transform, cuberArtInHololens.transform);
    }
}


//void CheckTheConditionAndUpdateHologram()
//{
//    if (!IsVisibleByHololens(finalHoloLensWorld))
//    {
//        print("Updating Hologram");
//        UpdateHologram();
//    }
//    //UpdateHologram();
//}

//if (Input.GetKeyDown(KeyCode.Space))
//{
//    //CheckTheConditionAndUpdateHologram();
//    //UpdateHologram();
//}

//CheckTheConditionAndUpdateHologram();

//distanceBetweenHolograms = Vector3.Distance(finalHoloLensWorld.transform.position, finalArtWorld.transform.position);
////print(distanceBetweenHolograms);
//if (distanceBetweenHolograms > 0.05) // 5 cm
//{
//    CheckTheConditionAndUpdateHologram();
//}


// Static ART
// receiverObj.transform.position = networkManager.firstPosition;
// receiverObj.transform.rotation = networkManager.firstOrientation;

//bool startTheMagic = false;


//Cubes
//if (Input.GetKeyDown(KeyCode.Space))
//{
//    SetTheCubeTransformation(distance);
//    startTheMagic = true;
//}

//if (startTheMagic)
//{
//    DoTheMagicWithCubes();
//}


//void SetTheCubeTransformation(float distanceForward)
//{
//    cubeART.transform.position = transformedART.transform.position + transformedART.transform.forward * distanceForward;
//    cubeHololens.transform.position = hololensCamera.transform.position + hololensCamera.transform.forward * distanceForward;
//    cuberArtInHololens.transform.position = hololensCamera.transform.position + hololensCamera.transform.forward * distanceForward;
//}

//void DoTheMagicWithCubes()
//{
//    MoveRelatively(transformedART.transform, cubeART.transform, hololensCamera.transform, cuberArtInHololens.transform);
//    RotateRelatively(transformedART.transform, cubeART.transform, hololensCamera.transform, cuberArtInHololens.transform);
//}


//Vector3 x = receiverObj.transform.right * meanPos.x;
//Vector3 y = receiverObj.transform.up * meanPos.y;
//Vector3 z = receiverObj.transform.forward * meanPos.z;
//print("x: " + x + ",y: " + y + ",z: " + z);
//transformedART.transform.position = receiverObj.transform.position + x + y + z;

//transformedART.transform.position = receiverObj.transform.position;
//transformedART.transform.rotation = receiverObj.transform.rotation * meanRot;