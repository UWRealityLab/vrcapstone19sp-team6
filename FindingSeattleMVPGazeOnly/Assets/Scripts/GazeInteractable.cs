using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Valve.VR;

public class GazeInteractable : MonoBehaviour
{
    public GameObject[] showOnGaze;
    public GameObject[] hideOnGaze;
    public GameObject[] showOnSelect;
    public GameObject[] hideOnSelect;
    public GameObject gazeArea;
    public Material material;

    public enum Action { CHANGE_SCENE, NO_ACTION, VIDEO_PLAYER, NARRATIVE_CHOICE };
    public Action onSelectAction;

    // If CHANGE_SCENE: happens immediately
    // If NARRATIVE_CHOICE: happens after onSelectNarrativeTimeBeforeTransition seconds;
    public string onSelectNextScene;
    public AudioClip onSelectNarrativeAudioClip;
    public GameObject[] onSelectNarrativeChoiceHide;
    public string onSelectNarrativeVideoClip;
    public float onSelectNarrativeTimeBeforeTransition;

    public GazeMenu gazeMenu;


    public VideoPlayer videoPlayer;
    public AudioSource audioPlayer;

    protected virtual void Start()
    {
        GazeStop();
        Deselect();
    }

    public virtual void Select()
    {
        foreach (GameObject obj in showOnSelect)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in hideOnSelect)
        {
            obj.SetActive(false);
        }
        if (onSelectAction == Action.CHANGE_SCENE)
        {
            StartSceneTransition();
            gazeMenu.Deselect();
        }
        if (onSelectAction == Action.VIDEO_PLAYER)
        {
            gazeArea.GetComponent<Renderer>().material = material;

            videoPlayer.Play();
        }
        if (onSelectAction == Action.NARRATIVE_CHOICE)
        {
            /**
             * To use, make each interactable in a narrative choice be responsible for hiding all of
             * the narrative choice GameObjects, and also give it an appropriate audio clip and video clip that it will play.
             * After the specified number of seconds, it will automatically transition to the next scene.
             */
            foreach (GameObject obj in onSelectNarrativeChoiceHide)
            {
                obj.SetActive(false);
            }

            SteamVR_Fade.Start(Color.black, 1f);
            Invoke("NChoicePlayVideo", 1f);
            Invoke("NChoicePlayAudio", 1f);
            Invoke("NChoiceFadeIn", 3f);
            Invoke("NChoiceFadeOut", onSelectNarrativeTimeBeforeTransition); // + 1f for time to start playing, - 1f to start fading before it ends
            Invoke("NChoiceAdvanceScene", onSelectNarrativeTimeBeforeTransition + 1f); // + 1f for time to start playing
        }
    }

    private void NChoicePlayAudio()
    {
        audioPlayer.PlayOneShot(onSelectNarrativeAudioClip);
    }

    private void NChoicePlayVideo()
    {
        videoPlayer.url = onSelectNarrativeVideoClip;
        videoPlayer.Play();
    }

    private void NChoiceAdvanceScene()
    {
        SteamVR_LoadLevel.Begin(onSelectNextScene);
    }

    private void NChoiceFadeIn()
    {
        SteamVR_Fade.Start(Color.clear, 1f);
    }

    private void NChoiceFadeOut()
    {
        SteamVR_Fade.Start(Color.black, 1f);
    }

    public virtual void Deselect()
    {
        foreach (GameObject obj in showOnSelect)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in hideOnSelect)
        {
            obj.SetActive(true);
        }
    }

    public virtual void GazeStart()
    {
        foreach (GameObject obj in showOnGaze)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in hideOnGaze)
        {
            obj.SetActive(false);
        }
    }

    public virtual void GazeStop()
    {
        foreach (GameObject obj in showOnGaze)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in hideOnGaze)
        {
            obj.SetActive(true);
        }
        if (onSelectAction == Action.VIDEO_PLAYER) {
            videoPlayer.Pause();
        }
    }

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    public virtual Quaternion GetRotation()
    {
        return transform.rotation * Quaternion.Euler(90, 0, 0);
    }

    private void StartSceneTransition()
    {
        SteamVR_LoadLevel.Begin(onSelectNextScene);
    }
}
