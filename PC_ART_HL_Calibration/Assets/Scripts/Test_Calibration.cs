using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Test_Calibration : MonoBehaviour
{
    [Serializable]
    public class TransformationData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public Transform artHololens;
    public Transform hololens;

    private GameObject calibrator;

    // Start is called before the first frame update
    void Start()
    {
        calibrator = new GameObject();
        ReadFromFile("calibrationMatrix", calibrator.transform);
        ReadFromFile("artHololens", artHololens);
        ReadFromFile("vuforiaHololens", hololens);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadFromFile(string name, Transform transform)
    {
        TransformationData myObj = new TransformationData();
        string path = Application.dataPath + "/Data/" + name + ".json";
        string data = File.ReadAllText(path);
        myObj = JsonUtility.FromJson<TransformationData>(data);
        transform.position = myObj.position;
        transform.rotation = myObj.rotation;
    }
}
