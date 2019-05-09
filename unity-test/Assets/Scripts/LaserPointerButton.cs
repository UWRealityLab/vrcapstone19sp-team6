using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Valve.VR;
using Valve.VR.Extras;

public class LaserPointerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // If set, selecting this button will transition to the specified scene.
    // Make sure the scene is included in the Asset Bundle.
    public string targetScene;

    // When true, this button only appears when the global configuration is
    // used to enter debug mode.
    public bool showInDebugOnly;

    // These can be overridden with other materials
    // to change the look of the button
    public Material normalMaterial;
    public Material selectedMaterial;

    private Texture2D blankTexture;

    // Start is called before the first frame update
    void Start()
    {
        if (showInDebugOnly && !GameObject.FindWithTag("Player").GetComponent<GlobalConfig>().debugEnabled)
        {
            gameObject.SetActive(false);
        }
        else
        {
            GetComponent<Renderer>().material = normalMaterial;
        }
        blankTexture = (Texture2D)Resources.Load("BlankTexture");
    }

    void Update()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Renderer>().material = selectedMaterial;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Renderer>().material = normalMaterial;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetScene != null)
        {
            FadeToScene(targetScene);
        }
    }

    private void FadeToScene(string target)
    {
        var loader = new GameObject("loader").AddComponent<SteamVR_LoadLevel>();
        loader.levelName = target;
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