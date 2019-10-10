using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataAnalyzer : MonoBehaviour
{
    public bool logTheData;
    public PlayBack manager;
    public string user;
    public string caseNumber;
    public string method;

    public GameObject[] staticObjects;
    public GameObject[] dynamicObjects;

    public float[] positionDifferences;
    private Quaternion[] rotationDifferences;
    public Vector3[] rotationDifferenceEuler;
    public float totalTime;


    void Start()
    {
        positionDifferences = new float[4];
        rotationDifferences = new Quaternion[4];
        rotationDifferenceEuler = new Vector3[4];
        GetUserCaseMethod();
    }

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            CalculatePositionDifference(i);
            CalculateRotationDifference(i);
            GetRotationAroundY(i);
        }
        totalTime = GetTotalTime();

        if (manager.done && logTheData)
        {
            LogData();
            logTheData = false;
        }
    }


    void LogData()
    {
        string path = Application.dataPath + "/Data/FinalData/FinalData.csv";
        string data = GenerateData();
        File.AppendAllText(path, data);
    }

    string GenerateData()
    {
        string result = user + "," + caseNumber + "," + method + "," + totalTime + "," + manager.numberOfSlamUpdates + "," + manager.numberOfUserReports + "," +
            GetObjectData(0) + "," + GetObjectData(1) + "," + GetObjectData(2) + "," + GetObjectData(3) + "," + "0,0,0,";
        return result;
    }

    string GetObjectData(int index)
    {
        string result = dynamicObjects[index].GetComponent<ChangeColorOnMovement>().timeForPlacement + "," + positionDifferences[index] + "," +
            rotationDifferenceEuler[index].y + "," + dynamicObjects[index].GetComponent<ChangeColorOnMovement>().numberOfUserAdjustments;
        return result;
    }

    void CalculateRotationDifference(int index)
    {
        rotationDifferences[index] = staticObjects[index].transform.rotation * Quaternion.Inverse(dynamicObjects[index].transform.rotation);
    }

    void GetRotationAroundY(int index)
    {
        rotationDifferenceEuler[index] = rotationDifferences[index].eulerAngles;
        if (rotationDifferenceEuler[index].x > 180)
        {
            rotationDifferenceEuler[index].x -= 360;
        }
        if (rotationDifferenceEuler[index].y > 180)
        {
            rotationDifferenceEuler[index].y -= 360;
        }
        if (rotationDifferenceEuler[index].z > 180)
        {
            rotationDifferenceEuler[index].z -= 360;
        }
    }

    void CalculatePositionDifference(int index)
    {
        positionDifferences[index] = Vector3.Distance(staticObjects[index].transform.position, dynamicObjects[index].transform.position);
    }

    float GetTotalTime()
    {
        float result = 0;
        for (int i = 0; i < 4; i++)
        {
            result += dynamicObjects[i].GetComponent<ChangeColorOnMovement>().timeForPlacement;
        }
        return result;
    }

    void GetUserCaseMethod()
    {
        string fileName = manager.fileName;
        print(fileName);
        for (int i = 0; i < fileName.Length; i++)
        {
            if (fileName[i] == '1' || fileName[i] == '2' || fileName[i] == '3' || fileName[i] == '4')
            {
                print("index is " + i);
                for (int j = 0; j < i; j++)
                {
                    user += fileName[j];
                }
                caseNumber = "" + fileName[i];
                for (int j = i + 1; j < fileName.Length; j++)
                {
                    method += fileName[j];
                }
                break;
            }
        }
    }
}
