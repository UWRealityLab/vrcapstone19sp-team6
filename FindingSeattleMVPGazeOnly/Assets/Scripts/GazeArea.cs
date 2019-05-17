using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeArea : MonoBehaviour
{
    public GazeInteractable gazeInteractable;

    private void Start()
    {
        GetComponent<Renderer>().material.color = Color.clear;
    }

    public GazeInteractable getGazeInteractable()
    {
        if (gazeInteractable == null)
        {
            Debug.Log("WARNING GAZE INTERACTABLE IS NULL");
            Debug.Log(this);
        }
        return gazeInteractable;
    }
}
