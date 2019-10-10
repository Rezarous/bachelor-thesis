using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioChanger : MonoBehaviour {

    public MultiMarkerManager manager;
    public GameObject sphere;
    public GameObject[] scenarioModels;
    public TextMesh scenarioName;
    public Material[] sphereMaterials;
    public GameObject floatingStuff;
    public GameObject dynamicStuff;
    public GameObject staticStuff;
    

    private Transformation_Loader transformationLoader;

    void Awake()
    {
        floatingStuff.SetActive(false);
        dynamicStuff.SetActive(false);
        staticStuff.SetActive(false);
        if (manager.adjust == MultiMarkerManager.adjustmentMotheds.Jump)
        {
            SetThisOneToActiveAndRestToFalse(0);
            sphere.GetComponent<Renderer>().material = sphereMaterials[0];
            scenarioName.text = "Bunny";
        }
        else if (manager.adjust == MultiMarkerManager.adjustmentMotheds.Lerp)
        {
            SetThisOneToActiveAndRestToFalse(1);
            sphere.GetComponent<Renderer>().material = sphereMaterials[1];
            scenarioName.text = "Shark";
        }
        else if (manager.adjust == MultiMarkerManager.adjustmentMotheds.BehindTheBack)
        {
            SetThisOneToActiveAndRestToFalse(2);
            sphere.GetComponent<Renderer>().material = sphereMaterials[2];
            scenarioName.text = "Dagger";
        }
        else
        {
            SetThisOneToActiveAndRestToFalse(3);
            sphere.GetComponent<Renderer>().material = sphereMaterials[3];
            scenarioName.text = "Hololens";
        }
    }

    void SetThisOneToActiveAndRestToFalse(int index)
    {
        for(int i=0; i<4; i++)
        {
            if (i == index)
            {
                scenarioModels[i].SetActive(true);
            }
            else
            {
                scenarioModels[i].SetActive(false);
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
