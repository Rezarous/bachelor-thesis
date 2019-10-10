using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Calibrator : MonoBehaviour
{

    [Serializable]
    public class TransformationData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public Transform artHololens;
    public Transform artMarker;
    public Transform vuforiaHololens;
    public Transform vuforiaMarker;
    public Transform vuforiaParent;

    public Transform vuforiaHololensOriginal;
    public Transform vuforiaMarkerOriginal;

    public GameObject target;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    Vector3 positionFixerVector = new Vector3(0,0,0);
    Quaternion rotationFixer = new Quaternion(0,0,0,1);

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = target.transform.position;
        targetRotation = target.transform.rotation;

        ReadFromFile("artHololens", artHololens);
        ReadFromFile("artMarker", artMarker);
        ReadFromFile("vuforiaHololens", vuforiaHololensOriginal);
        ReadFromFile("vuforiaMarker", vuforiaMarkerOriginal);

        MoveVuforiaToCenter();
        CalculateTheTransformation();
        CalibrateArtToHoloLens();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveCalibrationData();
            print("Calculated!");
        }
    }

    void SaveCalibrationData()
    {
        string path = Application.dataPath + "/Data/calibrationMatrix.json";
        print("Vector: " + positionFixerVector.x + "," + positionFixerVector.y + "," + positionFixerVector.z);
        print("Quaternion: " + rotationFixer.x + "," + rotationFixer.y + "," + rotationFixer.z + "," + rotationFixer.w);
        TransformationData obj = new TransformationData
        {
            position = positionFixerVector,
            rotation = rotationFixer
        };
        print("Object: " + obj.position.x.ToString());
        string data = JsonUtility.ToJson(obj);
        File.WriteAllText(path, data);
    }

    void MoveVuforiaToCenter()
    {
        vuforiaHololens.transform.localRotation = vuforiaHololensOriginal.transform.rotation;
        vuforiaMarker.transform.localRotation = vuforiaMarkerOriginal.transform.rotation;

        Quaternion rotationFixer = targetRotation * Quaternion.Inverse(vuforiaMarkerOriginal.transform.rotation);
        vuforiaParent.transform.rotation = rotationFixer * vuforiaParent.transform.rotation;

        Vector3 relativePosition = vuforiaMarkerOriginal.InverseTransformDirection(vuforiaHololensOriginal.position - vuforiaMarkerOriginal.position);
        print(relativePosition.ToString());
        Vector3 x = vuforiaMarker.right * relativePosition.x;
        Vector3 y = vuforiaMarker.up * relativePosition.y;
        Vector3 z = vuforiaMarker.forward * relativePosition.z;
        vuforiaHololens.position = vuforiaMarker.position + x + y + z;
    }

    void CalculateTheTransformation()
    {
        rotationFixer = Quaternion.Inverse(artHololens.rotation) * vuforiaHololens.rotation;

        artHololens.rotation = artHololens.rotation * rotationFixer;

        //positionFixerVector = vuforiaHololens.position - artHololens.position;
        positionFixerVector = artHololens.transform.InverseTransformDirection(vuforiaHololens.position - artHololens.position);
        
        
        //print("Rotation: " + rotationFixer);
    }

    void CalibrateArtToHoloLens()
    {

        Vector3 x = artHololens.right * positionFixerVector.x;
        Vector3 y = artHololens.up * positionFixerVector.y;
        Vector3 z = artHololens.forward * positionFixerVector.z;
        artHololens.position = artHololens.position + x + y + z;
    }


    void ReadFromFile(string name, Transform transform)
    {
        TransformationData myObj = new TransformationData();
        string path = Application.dataPath + "/Data/" + name + ".json";
        string data = File.ReadAllText(path);
        myObj = JsonUtility.FromJson<TransformationData>(data);
        transform.localPosition = myObj.position;
        transform.localRotation = myObj.rotation;
    }
}
