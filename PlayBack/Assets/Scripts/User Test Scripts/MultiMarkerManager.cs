using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMarkerManager : MonoBehaviour {

    //Network
    private MultiMarkerReceiver networkManager;
    private int numberOfMarkers;

    //Camera
    public GameObject markerOnHololens;
    public GameObject transformedART;
    public GameObject hololensCamera;
    public Vector3 relativePosition;
    public Quaternion relativeRotation;

    // Calibration
    Mean_Calculator mean;
    Vector3 meanPos;
    Quaternion meanRot;
    HandEyeCalibration handEye;
    Quaternion newMeanRot;
    Phone2PC phoneData;

    //Objects
    public GameObject[] floatingHologram;
    public GameObject[] staticHolograms;
    public GameObject[] dynamicHolograms;
    //public GameObject finalHoloLensWorld;
    //public GameObject normalHololensWorld;

    // Adjustment Methods
    public enum adjustmentMotheds { Jump, Lerp, BehindTheBack, SLAM };
    public adjustmentMotheds adjust;
    public float adjustmentSpeed;
    private Vector3[] targetPosition;
    private Quaternion[] targetRotation;
    private bool firstRegistrationDone = false;

    // Update Hologram position
    private int currentPose = 1; // To avoid updating the hologram to (0,0,0)
    private Vector3[][] temporaryPoses;
    public int numberOfSamples;
    private Vector3[] finalPositionOfHologram;
    private Vector3[] previousFinalPosition;
    private float distanceOfcurrentAndPreviousPosition;
    public float firstThreshold;
    public TextMesh textMesh;

    // Convert Markers from ART to Hololens SLAM
    bool hololensWorldRegistered = false;
    CameraStateDetector cameraState;
    GazeDirection gazeDirection;

    // Number of System update
    public int numberOfCoordinateUpdates;
    public int numberOfReports;

    void Start ()
    {
        numberOfCoordinateUpdates = 0;
        numberOfReports = 0;

        cameraState = hololensCamera.GetComponent<CameraStateDetector>();
        cameraState.waitingNumberOfFrames = 90;

        gazeDirection = hololensCamera.GetComponent<GazeDirection>();

        networkManager = GetComponent<MultiMarkerReceiver>();
        numberOfMarkers = networkManager.expectedNumberOfMarkers;

        handEye = GetComponent<HandEyeCalibration>();
        phoneData = GetComponent<Phone2PC>();

        mean = GetComponent<Mean_Calculator>();
        meanPos = mean.meanPos;
        meanRot = mean.meanRot;

        int lengthOfObjects = dynamicHolograms.Length;
        targetPosition = new Vector3[lengthOfObjects];
        targetRotation = new Quaternion[lengthOfObjects];
        finalPositionOfHologram = new Vector3[lengthOfObjects];
        previousFinalPosition = new Vector3[lengthOfObjects];

        temporaryPoses = new Vector3[lengthOfObjects][];
        for(int i=0; i<lengthOfObjects; i++)
        {
            temporaryPoses[i] = new Vector3[numberOfSamples];
        }
    }

    private int nextHologram = 1;
    public GameObject gazeInitSphere;
    private int gazeCounter = 0;
    public int numberOfShownHolograms = 0;

    void GoNext()
    {
        //gazeInitSphere.GetComponent<SphereCollider>().enabled = false;
        numberOfShownHolograms++;
        gazeInitSphere.SetActive(false);
        if (!hololensWorldRegistered)
        {
            TurnTheHologramsOn();
            HologramsInitialization(nextHologram);
            nextHologram++;
            hololensWorldRegistered = true;
        }
        else
        {
            HologramsInitialization(nextHologram);
            nextHologram++;
        }
        if(nextHologram != 5)
        {
            StartCoroutine(turnOnSphereCollider());
        }
    }

    IEnumerator turnOnSphereCollider()
    {
        yield return new WaitForSeconds(5);
        gazeInitSphere.SetActive(true);
        gazeInitSphere.GetComponent<SphereCollider>().enabled = true;
    }

    void TurnTheHologramsOn()
    {
        if (adjust == MultiMarkerManager.adjustmentMotheds.SLAM)
        {
            staticHolograms[0].SetActive(true);
        }
        else
        {
            dynamicHolograms[0].SetActive(true);
        }
    }


    void Update ()
    {
        TransformART();

        if (cameraState.cameraIsStatic && gazeDirection.gazeInit)
        {
            gazeCounter++;
            // gazeInitSphere.GetComponent<Renderer>().material.color = new Color(0, (gazeCounter/100), 0);
            if(gazeCounter == 100)
            {
                gazeCounter = 0;
                GoNext();
            }
        }
        else
        {
            gazeCounter = 0;
        }

        textMesh.text = "" + (gazeCounter) + "%";

        CollectAndUpdate();

        if (firstRegistrationDone)
        {
            CallEveryFrame(); //For Lerp and BehindTheBack
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Reported!");
            numberOfReports++;
        }
        
    }

    void CollectAndUpdate()
    {
        if (cameraState.cameraIsStatic && (gazeDirection.gazeHit || gazeDirection.gazeInit))
        {
            for (int i = 0; i < dynamicHolograms.Length; i++)
            {
                temporaryPoses[i][currentPose] = floatingHologram[i].transform.position;
            }
            currentPose++;
        }

        if (currentPose % numberOfSamples == 0)
        {
            calculateTheMeanPosition();
            for(int i=0; i<dynamicHolograms.Length; i++)
            {
                distanceOfcurrentAndPreviousPosition = Vector3.Distance(previousFinalPosition[i], finalPositionOfHologram[i]);
                if (distanceOfcurrentAndPreviousPosition > firstThreshold)
                {
                    calculateTheMeanPosition();
                    UpdateFinalHologram(i, finalPositionOfHologram[i]);
                    previousFinalPosition[i] = finalPositionOfHologram[i];
                }
            }
            currentPose = 0;
        }
    }

    void calculateTheMeanPosition()
    {
        for (int i = 0; i < dynamicHolograms.Length; i++)
        {
            finalPositionOfHologram[i] = Vector3.zero;
            foreach (Vector3 pos in temporaryPoses[i])
            {
                finalPositionOfHologram[i] += pos / numberOfSamples;
            }
        }
    }

    void UpdateFinalHologram(int index, Vector3 position)
    {
        numberOfCoordinateUpdates++;
        if (adjust == adjustmentMotheds.Jump)
        {
            dynamicHolograms[index].transform.position = position;
            dynamicHolograms[index].transform.rotation = floatingHologram[index].transform.rotation;
        }
        else
        {
            targetPosition[index] = position;
            targetRotation[index] = floatingHologram[index].transform.rotation;
        }
    }

    void HologramsInitialization(int index)
    {
        for (int i=0; i<dynamicHolograms.Length; i++)
        {
            UpdatePositionAndOrientation(staticHolograms[i], floatingHologram[i].transform.position, floatingHologram[i].transform.rotation);
            UpdatePositionAndOrientation(dynamicHolograms[i], floatingHologram[i].transform.position, floatingHologram[i].transform.rotation);
            previousFinalPosition[i] = dynamicHolograms[i].transform.position;
        }
        staticHolograms[0].transform.GetChild(0).transform.GetChild(index).gameObject.SetActive(true);
        dynamicHolograms[0].transform.GetChild(0).transform.GetChild(index).gameObject.SetActive(true);
        StartCoroutine("AllowAdjustment");
    }

    IEnumerator AllowAdjustment()
    {
        yield return new WaitForSeconds(1);
        cameraState.waitingNumberOfFrames = 45;
        firstRegistrationDone = true;
    }

    void UpdatePositionAndOrientation(GameObject obj, Vector3 pos, Quaternion rot)
    {
        obj.transform.position = pos;
        obj.transform.rotation = rot;
    }

    void CallEveryFrame()
    {
        if (adjust == adjustmentMotheds.Lerp)
        {
            for (int i = 0; i < dynamicHolograms.Length; i++)
            {
                dynamicHolograms[i].transform.position = Vector3.Lerp(dynamicHolograms[i].transform.position, targetPosition[i], adjustmentSpeed);
                dynamicHolograms[i].transform.rotation = Quaternion.Lerp(dynamicHolograms[i].transform.rotation, targetRotation[i], adjustmentSpeed);
            }
        }
        else if (adjust == adjustmentMotheds.BehindTheBack)
        {
            for (int i = 0; i < dynamicHolograms.Length; i++)
            {
                if (!IsVisibleByHololens(dynamicHolograms[i]))
                {
                    dynamicHolograms[i].transform.position = targetPosition[i];
                    dynamicHolograms[i].transform.rotation = targetRotation[i];
                }
            } 
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

    void TransformART()
    {

        float q1 = meanRot.x + handEye.q1 / 100;
        float q2 = meanRot.y + handEye.q2 / 100;
        float q3 = meanRot.z + handEye.q3 / 100;
        float q4 = meanRot.w + handEye.q4 / 100;

        newMeanRot = new Quaternion(q1, q2, q3, q4);
        transformedART.transform.rotation = markerOnHololens.transform.rotation * newMeanRot;

        Vector3 x = -(transformedART.transform.right * (meanPos.x + handEye.x / 10));
        Vector3 y = -(transformedART.transform.up * (meanPos.y + handEye.y / 10));
        Vector3 z = -(transformedART.transform.forward * (meanPos.z + handEye.z / 10));

        transformedART.transform.position = markerOnHololens.transform.position + x + y + z;
        ApplyTransformationOFWorldCoordinates();
        //print("x: " + meanPos.x + handEye.x / 10);
        //print("y: " + meanPos.y + handEye.y / 10);
        //print("z: " + meanPos.z + handEye.z / 10);
        //print("q1: " + q1);
        //print("q2: " + q2);
        //print("q3: " + q3);
        //print("q4: " + q4);
    }


    void ApplyTransformationOFWorldCoordinates()
    {
        for (int i = 0; i < floatingHologram.Length; i++)
        {
            MoveRelatively(transformedART.transform, networkManager.markers[i + 1].transform, hololensCamera.transform, floatingHologram[i].transform);
            RotateRelatively(transformedART.transform, networkManager.markers[i + 1].transform, hololensCamera.transform, floatingHologram[i].transform);
        }
    }

    void MoveRelatively(Transform dynamicCamera, Transform staticObj, Transform staticCamera, Transform dynamicObj)
    {
        relativePosition = dynamicCamera.InverseTransformDirection(staticObj.position - dynamicCamera.position);
        Vector3 x = staticCamera.right * relativePosition.x;
        Vector3 y = staticCamera.up * relativePosition.y;
        Vector3 z = staticCamera.forward * relativePosition.z;
        dynamicObj.localPosition = staticCamera.position + x + y + z;
    }

    void RotateRelatively(Transform dynamicCamera, Transform staticObj, Transform staticCamera, Transform dynamicObj)
    {
        relativeRotation = Quaternion.Inverse(dynamicCamera.rotation) * staticObj.rotation;
        dynamicObj.rotation = staticCamera.rotation * relativeRotation;
    }
}
