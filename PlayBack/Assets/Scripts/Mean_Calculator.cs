using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Mean_Calculator : MonoBehaviour
{

    [Serializable]
    public class TransformationData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public GameObject average;

    float[] positionValues = new float[3];
    float[] rotationValues = new float[4];
    public Vector3 meanPos;
    public Quaternion meanRot;

    void Awake()
    {
        string directoryPath = Application.dataPath + "/Data/Calibration";
        int numberOfFiles = Directory.GetFiles(directoryPath).Length/2;
        for (int i=1; i< numberOfFiles+1; i++)
        {
            ReadFromFile("" + i, i);
        }

        meanPos = new Vector3(positionValues[0]/numberOfFiles, positionValues[1] / numberOfFiles, positionValues[2] / numberOfFiles);
        meanRot = new Quaternion(rotationValues[0] / numberOfFiles, rotationValues[1] / numberOfFiles, rotationValues[2] / numberOfFiles, rotationValues[3] / numberOfFiles);
        print("Mean Pos: " + meanPos);
        print("Mean Rot: " + meanRot);
        if(average != null)
        {
            average.transform.position = meanPos;
            average.transform.rotation = meanRot;
        }
    }

    void ReadFromFile(string name, int i)
    {
        TransformationData myObj = new TransformationData();
        string path = Application.dataPath + "/Data/Calibration/" + name + ".json";
        string data = File.ReadAllText(path);
        myObj = JsonUtility.FromJson<TransformationData>(data);

        positionValues[0] += myObj.position.x;
        positionValues[1] += myObj.position.y;
        positionValues[2] += myObj.position.z;

        rotationValues[0] += myObj.rotation.x;
        rotationValues[1] += myObj.rotation.y;
        rotationValues[2] += myObj.rotation.z;
        rotationValues[3] += myObj.rotation.w;
    }

    private void Start()
    {
        
    }
}
