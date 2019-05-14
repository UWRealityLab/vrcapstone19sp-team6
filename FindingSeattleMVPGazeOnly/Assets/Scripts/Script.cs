using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Valve.VR;

public class Script : MonoBehaviour
{
    public enum Direction { IN, OUT };

    [System.Serializable]
    public class ScriptAudio { public float time; public AudioClip source; }
    public List<ScriptAudio> scriptAudioEvents;

    [System.Serializable]
    public class ScriptVideo { public float time; public string source; }
    public List<ScriptVideo> scriptVideoEvents;

    [System.Serializable]
    public class ScriptFade { public float time; public Direction direction; public float duration; }
    public List<ScriptFade> scriptFadeEvents;

    [System.Serializable]
    public class ScriptAnimateText { public float time; public Direction direction; public float duration; public AnimatedText text; }
    public List<ScriptAnimateText> scriptAnimateTextEvents;

    public Material VRVideoMaterial;

    private VideoPlayer VRVideo;
    private AudioSource VRAudio;
    private double t;
    private bool playing;

    private void Start()
    {
        VRVideo = GetComponent<VideoPlayer>();
        VRAudio = GetComponent<AudioSource>();
        t = 0;
        playing = true;
        
        SteamVR_Fade.Start(Color.clear, 0f);
        VRVideo.Stop();
    }

    public void Pause()
    {
        playing = false;
    }

    public void Play()
    {
        playing = true;
    }

    private void Update()
    {
        if (playing)
        {
            t += Time.deltaTime;
        }

        for (int i = 0; i < scriptAudioEvents.Count; i++)
        {
            if (t > scriptAudioEvents[i].time)
            {
                HandleScriptAudio(scriptAudioEvents[i]);
                scriptAudioEvents.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < scriptVideoEvents.Count; i++)
        {
            if (t > scriptVideoEvents[i].time)
            {
                HandleScriptVideo(scriptVideoEvents[i]);
                scriptVideoEvents.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < scriptFadeEvents.Count; i++)
        {
            if (t > scriptFadeEvents[i].time)
            {
                HandleScriptFade(scriptFadeEvents[i]);
                scriptFadeEvents.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < scriptAnimateTextEvents.Count; i++)
        {
            if (t > scriptAnimateTextEvents[i].time)
            {
                HandleScriptAnimateText(scriptAnimateTextEvents[i]);
                scriptAnimateTextEvents.RemoveAt(i);
                i--;
            }
        }
    }

    private void HandleScriptAudio(ScriptAudio scriptAudio)
    {
        VRAudio.PlayOneShot(scriptAudio.source);
    }

    private void HandleScriptVideo(ScriptVideo scriptVideo)
    {
        RenderSettings.skybox = VRVideoMaterial;
        VRVideo.url = scriptVideo.source;
        VRVideo.frame = 0;
        VRVideo.Play();
    }

    private void HandleScriptFade(ScriptFade scriptFade)
    {
        Color toColor = (scriptFade.direction == Direction.IN) ? Color.clear : Color.black;
        SteamVR_Fade.Start(toColor, scriptFade.duration);
    }

    private void HandleScriptAnimateText(ScriptAnimateText scriptAnimateText)
    {
        if (scriptAnimateText.direction == Direction.IN)
        {
            scriptAnimateText.text.Enter(scriptAnimateText.duration);
        }
        else
        {
            scriptAnimateText.text.Exit(scriptAnimateText.duration);
        }
    }
}
