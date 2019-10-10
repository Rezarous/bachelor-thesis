using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayBack : MonoBehaviour
{

    [Serializable]
    public class DataWrapper
    {
        public Data[] DataArray;
    }

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

    public string fileName;
    public string userName;
    public int readingFrequency;
    public GameObject headset;
    public GameObject baseModel;
    public GameObject geometricModel;
    public GameObject penModel;
    public GameObject boatModel;
    public GameObject carModel;

    public DataWrapper myObj;
    private int frequencyController;
    public float timeStamp;
    public bool done;
    // private int index;

    //Slider
    public Slider slider;
    public Text buttonText;
    public bool play;
    private int scenarioLength;
    public int currentIndex;

    //Changing Colors
    public Material whiteMat;
    public Material redMat;
    public Material greenMat;
    public float colorThreshold;


    //UI
    public Text timeText;
    public Text numberOfShownHolograms;
    public Text systemUpdateText;
    public Text userReportText;

    public int numberOfSlamUpdates;
    public int numberOfUserReports;


    // Use this for initialization
    void Awake()
    {
        myObj = new DataWrapper();
        ReadFromFile();
        frequencyController = 0;
        UpdateAllTransformations(0);
        //index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentIndex == scenarioLength && !done)
        {
            print("Done");
            numberOfSlamUpdates = myObj.DataArray[currentIndex].slamUpdate;
            numberOfUserReports = myObj.DataArray[currentIndex].userReportOfSlamUpdate;
            done = true;
        }
        if (play && currentIndex < scenarioLength)
        {
            if (frequencyController % readingFrequency == 0)
            {
                currentIndex++;
                slider.value = currentIndex;
            }
            frequencyController++;
        }
        else
        {
            currentIndex = (int)slider.value;
        }

        if (myObj.DataArray[currentIndex].numberOfShownHolograms == 1)
        {
            GetComponent<Transformation_Loader>().baseModel.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (myObj.DataArray[currentIndex].numberOfShownHolograms == 2)
        {
            GetComponent<Transformation_Loader>().baseModel.transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (myObj.DataArray[currentIndex].numberOfShownHolograms == 3)
        {
            GetComponent<Transformation_Loader>().baseModel.transform.GetChild(3).gameObject.SetActive(true);
        }
        else if (myObj.DataArray[currentIndex].numberOfShownHolograms == 4)
        {
            GetComponent<Transformation_Loader>().baseModel.transform.GetChild(4).gameObject.SetActive(true);
        }

        UpdateAllTransformations(currentIndex);
        UpdateStates(currentIndex);
    }


    void UpdateStates(int index)
    {
        if (index < ChangeColorOnMovement.stateController)
        {
            geometricModel.GetComponent<ChangeColorOnMovement>().previousPosition =
                myObj.DataArray[0].geometricPosition;
            penModel.GetComponent<ChangeColorOnMovement>().previousPosition =
                myObj.DataArray[0].penPosition;
            boatModel.GetComponent<ChangeColorOnMovement>().previousPosition =
                myObj.DataArray[0].boatPosition;
            carModel.GetComponent<ChangeColorOnMovement>().previousPosition =
                myObj.DataArray[0].carPosition;
        }
        else
        {
            geometricModel.GetComponent<ChangeColorOnMovement>().previousPosition =
                myObj.DataArray[index - ChangeColorOnMovement.stateController].geometricPosition;
            penModel.GetComponent<ChangeColorOnMovement>().previousPosition =
                myObj.DataArray[index - ChangeColorOnMovement.stateController].penPosition;
            boatModel.GetComponent<ChangeColorOnMovement>().previousPosition =
                myObj.DataArray[index - ChangeColorOnMovement.stateController].boatPosition;
            carModel.GetComponent<ChangeColorOnMovement>().previousPosition =
                myObj.DataArray[index - ChangeColorOnMovement.stateController].carPosition;
        }
        timeStamp = myObj.DataArray[index].timeStamp;
    }

    public void playOrPause()
    {
        play = !play;
        buttonText.text = (play) ? "Pause" : "Play";
    }

    void UpdateAllTransformations(int index)
    {
        headset.transform.position = myObj.DataArray[index].headsetPosition;
        headset.transform.rotation = myObj.DataArray[index].headsetRotaion;

        baseModel.transform.position = myObj.DataArray[index].basePosition;
        baseModel.transform.rotation = myObj.DataArray[index].baseRotation;

        geometricModel.transform.position = myObj.DataArray[index].geometricPosition;
        geometricModel.transform.rotation = myObj.DataArray[index].geometricRotation;

        penModel.transform.position = myObj.DataArray[index].penPosition;
        penModel.transform.rotation = myObj.DataArray[index].penRotation;

        boatModel.transform.position = myObj.DataArray[index].boatPosition;
        boatModel.transform.rotation = myObj.DataArray[index].boatRotation;

        carModel.transform.position = myObj.DataArray[index].carPosition;
        carModel.transform.rotation = myObj.DataArray[index].carRotation;

        timeText.text = "Time: " + myObj.DataArray[index].timeStamp;
        systemUpdateText.text = "System Update: " + myObj.DataArray[index].slamUpdate;
        userReportText.text = "User Report: " + myObj.DataArray[index].userReportOfSlamUpdate;
        numberOfShownHolograms.text = "Number Of Holograms: " + myObj.DataArray[index].numberOfShownHolograms;
    }

    void SetSliderProperties()
    {
        slider.maxValue = scenarioLength;
        slider.wholeNumbers = true;
    }

    void ReadFromFile()
    {
        string path = Application.dataPath + "/Data/" + userName + "/" + fileName + ".json";
        string data = File.ReadAllText(path);
        for (int i = data.Length - 1; i > 0; i--)
        {
            if (data[i] == ',')
            {
                StringBuilder sb = new StringBuilder(data);
                sb.Remove(i, 1);
                data = sb.ToString();
                break;
            }
        }
        //print(data);
        myObj = JsonUtility.FromJson<DataWrapper>(data);
        //print(myObj.DataArray.Length);
        scenarioLength = myObj.DataArray.Length - 1;
        SetSliderProperties();
    }
}
