using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Transformation_Saver : MonoBehaviour {

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
    public MultiMarkerReceiver receiver;
    private GameObject[] tempObjects;


    void Start () {
        tempObjects = new GameObject[4];
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for(int i=0; i<4; i++)
            {
                tempObjects[i] = new GameObject();
                tempObjects[i].transform.position = receiver.markers[i + 2].transform.position;
                tempObjects[i].transform.rotation = receiver.markers[i + 2].transform.rotation;
                tempObjects[i].transform.parent = baseModel.transform;
                //print("Local: " + receiver.markers[i + 2].name + " :" + tempObjects[i].transform.localPosition);
            }
            SaveTransformationDataAsJSON();
        }
	}

    void SaveTransformationDataAsJSON()
    {
        TransformationData obj = new TransformationData();
        obj.geometricPosition = tempObjects[0].transform.localPosition;
        obj.geometricRotation = tempObjects[0].transform.localRotation;
        obj.penPosition = tempObjects[1].transform.localPosition;
        obj.penRotation = tempObjects[1].transform.localRotation;
        obj.boatPosition = tempObjects[2].transform.localPosition;
        obj.boatRotation = tempObjects[2].transform.localRotation;
        obj.carPosition = tempObjects[3].transform.localPosition;
        obj.carRotation = tempObjects[3].transform.localRotation;
        CreateFile(caseName, obj);
    }

    void CreateFile(string name, TransformationData obj)
    {
        string path = Application.dataPath + "/Data/Cases/Case" + name + ".json";
        string data = JsonUtility.ToJson(obj);
        File.WriteAllText(path, data);
        print("Case" + name + " is generated!");
    }
}
