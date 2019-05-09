using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EvictionApartmentText : MonoBehaviour
{
    public Text planeText;
    // Start is called before the first frame update
    void Start()
    {
        planeText.text = "Working two jobs while caring for your elderly mother who is diagnosed with Lyme disease." + 
            "Because of the extra time you have had to spend at the hospital you lost one of your jobs today when rent was already hard to make as is";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
