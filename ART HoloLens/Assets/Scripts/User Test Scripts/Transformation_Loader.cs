using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Transformation_Loader : MonoBehaviour {

    [Serializable]
    public class TransformationData
    {
        public Vector3 geometricPosition;
        public Quaternion geometricRotation;
        public Vector3 penPosition;
        public Quaternion penRotation;
        public Vector3 boatPosition;
        public Quaternion boatRotation;
        public Vector3 carPosition;
        public Quaternion carRotation;
    }

    public string caseName;
    public GameObject baseModel;
    public GameObject geometry;
    public GameObject pen;
    public GameObject boat;
    public GameObject car;
    public GameObject staticHolograms;
    public GameObject dynamicHolograms;

    private TransformationData dataAsJson = new TransformationData();
    

    void Awake()
    {
        ReadFromFile();
        if(staticHolograms != null && dynamicHolograms != null)
        {
            GameObject object1 = Instantiate(baseModel);
            object1.transform.parent = staticHolograms.transform;
            GameObject object2 = Instantiate(baseModel);
            object2.transform.parent = dynamicHolograms.transform;
            object1.SetActive(true);
            object2.SetActive(true);
        }
    }

    void Start()
    {
        
    }

    void ReadFromFile()
    {
        TransformationData myObj = new TransformationData();
        string path = Application.dataPath + "/Data/Cases/Case" + caseName + ".json";
        string data = File.ReadAllText(path);
        myObj = JsonUtility.FromJson<TransformationData>(data);

        geometry.transform.localPosition = myObj.geometricPosition;
        geometry.transform.localRotation = myObj.geometricRotation;
        pen.transform.localPosition = myObj.penPosition;
        pen.transform.localRotation = myObj.penRotation;
        boat.transform.localPosition = myObj.boatPosition;
        boat.transform.localRotation = myObj.boatRotation;
        car.transform.localPosition = myObj.carPosition;
        car.transform.localRotation = myObj.carRotation;
    }
}
