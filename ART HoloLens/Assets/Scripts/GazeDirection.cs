using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeDirection : MonoBehaviour {

    //public GameObject gazeSphere;
    //public GameObject gazeCollisionIndicator;
    public Image gazeCollisionIndicator;
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public bool gazeHit = false;
    public bool gazeInit = false;
    RaycastHit hit;
    Ray ray;
	
	// Update is called once per frame
	void Update () {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        ray = new Ray(transform.position, forward);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Init")
            {
                gazeInit = true;
                gazeHit = false;
                //gazeCollisionIndicator.GetComponent<Renderer>().material = greenMaterial;
                gazeCollisionIndicator.color = Color.green;
            } else if (hit.collider.gameObject.tag == "GazeSphere")
            {
                gazeInit = false;
                gazeHit = true;
                gazeCollisionIndicator.color = Color.blue;
            }
            else
            {
                gazeInit = false;
                gazeHit = false;
                gazeCollisionIndicator.color = Color.red;
            }
        }
        else
        {
            gazeInit = false;
            gazeHit = false;
            gazeCollisionIndicator.color = Color.red;
        }

        Debug.DrawRay(transform.position, forward, Color.green);
    }
}
