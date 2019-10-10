using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

public class Transformation_Saver : MonoBehaviour
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

    public HL_Receive hLReceive;

    private void Start()
    {
        StartCoroutine(SaveAndPrint());
    }

    IEnumerator SaveAndPrint()
    {
        yield return new WaitForSeconds(10);
        SaveAll();
        print("Data:" + hLReceive.lastReceivedUDPPacket);
        EditorApplication.isPaused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveAll();
        }
    }

    void SaveAll()
    {
        SaveTransformationDataAsJSON("artHololens", artHololens);
        SaveTransformationDataAsJSON("artMarker", artMarker);
        SaveTransformationDataAsJSON("vuforiaHololens", vuforiaHololens);
        SaveTransformationDataAsJSON("vuforiaMarker", vuforiaMarker);
    }

    void SaveTransformationDataAsJSON(string name, Transform transform)
    {
        TransformationData obj = new TransformationData();
        obj.position = transform.position;
        obj.rotation = transform.rotation;
        CreateFile(name, obj);
    }

    void CreateFile(string name, TransformationData obj)
    {
        string path = Application.dataPath + "/Data/" + name + ".json";
        string data = JsonUtility.ToJson(obj);
        File.WriteAllText(path, data);
    }
}
