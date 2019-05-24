using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class StartExperience : MonoBehaviour
{
    void Start()
    {
        UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        SteamVR_LoadLevel.Begin("FS_START");
    }
}
