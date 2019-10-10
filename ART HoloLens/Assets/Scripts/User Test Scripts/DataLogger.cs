using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLogger : MonoBehaviour {

    [Serializable]
    public class Data
    {
        public float timeStamp;
        public int slamUpdate;
        public int userReportOfSlamUpdate;
        public int numberOfShownHolograms;
        public Vector3 headsetPosition;
        public Quaternion headsetRotaion;
        public Vector3 basePosition;
        public Quaternion baseRotation;
        public Vector3 geometricPosition;
        public Quaternion geometricRotation;
        public Vector3 penPosition;
        public Quaternion penRotation;
        public Vector3 boatPosition;
        public Quaternion boatRotation;
        public Vector3 carPosition;
        public Quaternion carRotation;
    }

    //public GameObject transformedHeadsetMarker;
    public GameObject haedset;
    public GameObject baseMarker; //base marker

    public string userName;
    public MultiMarkerManager manager;
    public MultiMarkerReceiver receiver;
    public int logFrequency;

    private DeltaTransformationTracker dtt;
    private Transformation_Loader transformationLoader;
    private string path;
    private string updateMothod;
    private int logCongroller;
    private string scenarioName;
    private int jsonIndex;

    private void Start()
    {
        jsonIndex = 0;
        dtt = GetComponent<DeltaTransformationTracker>();
        transformationLoader = GetComponent<Transformation_Loader>();

        if (manager.adjust == MultiMarkerManager.adjustmentMotheds.Jump)
        {
            updateMothod = "Jump";
        }
        else if (manager.adjust == MultiMarkerManager.adjustmentMotheds.Lerp)
        {
            updateMothod = "Lerp";
        }
        else if (manager.adjust == MultiMarkerManager.adjustmentMotheds.BehindTheBack)
        {
            updateMothod = "OnNotVisible";
        }
        else
        {
            updateMothod = "SLAM";
        }
        scenarioName = userName + transformationLoader.caseName + updateMothod;
        path = Application.dataPath + "/Data/" + scenarioName + ".json";
        CreateFile();

        logCongroller = 0;
    }

    void LateUpdate () {
        if (logCongroller % logFrequency == 0)
        {
            SaveTransformationDataAsJSON();
        }
        logCongroller++;
    }

    void SaveTransformationDataAsJSON()
    {
        Data dataObj = new Data();
        dataObj.timeStamp = Time.time;
        dataObj.slamUpdate = manager.numberOfCoordinateUpdates;
        dataObj.userReportOfSlamUpdate = manager.numberOfReports;
        dataObj.numberOfShownHolograms = manager.numberOfShownHolograms;

        dataObj.headsetPosition = haedset.transform.localPosition;
        dataObj.headsetRotaion = haedset.transform.localRotation;

        dataObj.basePosition = baseMarker.transform.localPosition;
        dataObj.baseRotation = baseMarker.transform.localRotation;

        //dataObj.geometricPosition = dtt.deltaGeometry.transform.localPosition;
        //dataObj.geometricRotation = dtt.deltaGeometry.transform.localRotation;

        //dataObj.penPosition = dtt.deltaPen.transform.localPosition;
        //dataObj.penRotation = dtt.deltaPen.transform.localRotation;

        //dataObj.boatPosition = dtt.deltaBoat.transform.localPosition;
        //dataObj.boatRotation = dtt.deltaBoat.transform.localRotation;

        //dataObj.carPosition = dtt.deltaCar.transform.localPosition;
        //dataObj.carRotation = dtt.deltaCar.transform.localRotation;

        dataObj.geometricPosition = receiver.markers[2].transform.position;
        dataObj.geometricRotation = receiver.markers[2].transform.rotation;

        dataObj.penPosition = receiver.markers[3].transform.position;
        dataObj.penRotation = receiver.markers[3].transform.rotation;

        dataObj.boatPosition = receiver.markers[4].transform.position;
        dataObj.boatRotation = receiver.markers[4].transform.rotation;

        dataObj.carPosition = receiver.markers[5].transform.position;
        dataObj.carRotation = receiver.markers[5].transform.rotation;

        SaveToFile(dataObj);
    }

    void CreateFile()
    {
        //File.WriteAllText(path, "{");
        File.WriteAllText(path, "{\"DataArray\":[");
    }

    void SaveToFile(Data obj)
    {
        if (File.Exists(path))
        {
            //string data = "\"" + jsonIndex + "\"" + ":" + JsonUtility.ToJson(obj) +",";
            //File.AppendAllText(path, data);
            string data = JsonUtility.ToJson(obj) + ",";
            File.AppendAllText(path, data);
        }
        jsonIndex++;
    }

    private void OnApplicationQuit()
    {
        File.AppendAllText(path, "]}");
    }

}
