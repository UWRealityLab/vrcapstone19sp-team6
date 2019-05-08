using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Valve.VR;

public class GoToNextScene : MonoBehaviour
{
    public string targetScene;

    public bool triggerOnVideoEnd = false;

    // Only used if triggerOnVideoEnd is false.
    public float delayBeforeTransition = 2.0f;

    private Texture2D blankTexture;
    private bool isCoroutineExecuting = false;

    void Start()
    {
        if (triggerOnVideoEnd)
        {
            GetComponent<VideoPlayer>().loopPointReached += EndReached;
        }
        else
        {
            Invoke("FadeToScene", delayBeforeTransition);
        }

        blankTexture = (Texture2D)Resources.Load("BlankTexture");
    }

    void EndReached(VideoPlayer vp)
    {
        FadeToScene();
    }

    private void FadeToScene()
    {
        var loader = new GameObject("loader").AddComponent<SteamVR_LoadLevel>();
        loader.levelName = targetScene;
        loader.showGrid = false;
        loader.fadeInTime = 0.3f;
        loader.fadeOutTime = 0.3f;
        loader.postLoadSettleTime = 1.5f;

        // All of this code is just to remove the default SteamVR loading screen
        // (has a loading bar and floating windows) because it is more jarring than a simple black fade
        loader.loadingScreen = blankTexture;
        loader.progressBarEmpty = blankTexture;
        loader.progressBarFull = blankTexture;
        loader.loadingScreenWidthInMeters = 0f;
        loader.progressBarWidthInMeters = 0f;
        //loader.front = blankTexture;
        //loader.back = blankTexture;
        //loader.left = blankTexture;
        //loader.right = blankTexture;
        //loader.top = blankTexture;
        //loader.bottom = blankTexture;
        loader.loadingScreenFadeInTime = 0.1f;
        loader.loadingScreenFadeOutTime = 0.1f;

        loader.backgroundColor = new Color(0f, 0f, 0f, 1f);
        loader.Trigger();
    }
}
