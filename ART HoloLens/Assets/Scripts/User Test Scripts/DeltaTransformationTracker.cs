using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaTransformationTracker : MonoBehaviour {

    public MultiMarkerReceiver network;
    public GameObject deltaGeometry;
    public GameObject deltaPen;
    public GameObject deltaBoat;
    public GameObject deltaCar;

    // Use this for initialization
    void Start () {
        //tl = gameObject.GetComponent<Transformation_Loader>();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateTransformation(deltaGeometry, network.markers[2]);
        UpdateTransformation(deltaPen, network.markers[3]);
        UpdateTransformation(deltaBoat, network.markers[4]);
        UpdateTransformation(deltaCar, network.markers[5]);
    }

    void UpdateTransformation(GameObject obj, GameObject targetObj)
    {
        obj.transform.position = targetObj.transform.position;
        obj.transform.rotation = targetObj.transform.rotation;
    }
}
