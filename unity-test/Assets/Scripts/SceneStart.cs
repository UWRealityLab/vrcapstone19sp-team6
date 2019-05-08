using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStart : MonoBehaviour
{
    public string firstScene;

    // Start is called before the first frame update
    void Start()
    {
        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        Valve.VR.OpenVR.Compositor.SetTrackingSpace(
        Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
        SceneManager.LoadScene(firstScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
