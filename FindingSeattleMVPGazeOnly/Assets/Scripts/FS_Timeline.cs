using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Valve.VR;

public class FS_Timeline : MonoBehaviour
{
    // Safety margin to allow video to start buffering before fading back in.
    // Note that video will actually have BUFFER + fadeDuration (default: 0.5) seconds to buffer
    // before it needs to be loaded because the request to the VideoPlayer is made as the video starts fading out
    // (with the reasoning that even if the video freezes it will hardly be noticeable while everything is fading and
    // in most cases it saves us an extra 0.5 seconds of lag).
    public const float BUFFER = 1.0f;

    // After fading into a new video, the number of seconds before the corresponding audio starts playing.
    public const float AUDIO_DELAY = 0.2f;

    public GameObject instructionsContainer;
    public AnimatedText[] experienceIntroductionTexts;
    public AnimatedText[] choiceSceneObjects;
    public AnimatedText[] museumInstructionTexts;
    public GameObject museumSceneContainer;

    void SetupInstructionsScene(float t)
    {
        // TODO: Still working on this one!
        ScriptShowInstructions(t);

        // No return -- give the user time to situate themselves before choosing to go on
    }

    float SetupStartScene(float t)
    {
        ScriptPlayAudioClip(t + 8, "music/intro_piano_music", 24);
        t = ScriptPlaySegment(t, 16.75f, "morning/small-room", "narration/24");
        t = ScriptShowStartText(t);

        return t;
    }

    float SetupEvictionScene(float t)
    {
        t = ScriptPlaySegment(t, 21, "eviction/inside-apartment", "other_audio/bank-knocking-shortened");
        t = ScriptPlaySegment(t, 10, "eviction/hall-short", "narration/short1");
        t = ScriptPlaySegment(t, 6, "eviction/zillow-scroll", "narration/short3");
        t = ScriptPlaySegment(t, 12, "eviction/scroll-shelter", "narration/4");
        ScriptPlayAudioClip(t + 1, "other_audio/doorknock", 5);
        t = ScriptPlaySegment(t, 9, "eviction/eviction-notice", "narration/short5");

        return t;
    }

    float SetupMorningScene(float t)
    {
        t = ScriptPlaySegment(t, 8, "morning/outside", "narration/6");
        t = ScriptPlaySegment(t, 13, "morning/front-desk", "narration/short7");
        t = ScriptPlaySegment(t, 10, "morning/bathroom", "narration/8alternate");
        t = ScriptPlaySegment(t, 10, "morning/big-room-long", "narration/9");

        return t;
    }

    void SetupParkScene(float t)
    {
        t = ScriptPlaySegment(t, 8, "morning/on-bed", "narration/short11");
        t = ScriptPlaySegment(t, 11, "park/park1", "narration/short12");
        t = ScriptPlaySegment(t, 21, "park/park2_Trim", "narration/13");
        ScriptShowChoiceScene(t);

        // No return -- give the user time to make a choice
    }

    float SetupChoseShelterScene(float t)
    {
        t = ScriptPlaySegment(t, 16, "morning/hallway", "narration/22alternate");

        return t;
    }

    float SetupChoseWorkScene(float t)
    {
        t = ScriptPlaySegment(t, 14, "morning/hallway", "narration/23alternate");

        return t;
    }

    float SetupEveningScene(float t)
    {
        t = ScriptPlaySegment(t, 12, "evening/entrance", "narration/short14");
        t = ScriptPlaySegment(t, 16, "evening/bunkbed", "narration/15");
        t = ScriptPlaySegment(t, 7, "evening/lockers", "narration/16");
        t = ScriptPlaySegment(t, 6, "evening/laundry_trim", "narration/short17-18");
        ScriptPlayAudioClip(t + 7, "music/end_piano_music", 28);
        t = ScriptPlaySegment(t, 12, "evening/shower", "narration/19");
        t = ScriptPlaySegment(t, 24, "evening/success", "narration/25");

        return t;
    }

    void SetupFinalScene(float t)
    {
        // TODO: Still working on the instructions part of this! (currently has no effect)
        t = ScriptShowMuseumInstructions(t);

        ScriptShowMuseumScene(t);

        // No return -- give the user time to explore the museum
    }

    /**
     * ====================================================================================================
     *  Below this point are utility classes and infrastructure to make scene setup work
     * ====================================================================================================
     */

    private AudioSource audioSource;
    private VideoPlayer videoPlayer;

    private float currentTime;
    private List<Event> timeline;
    private bool timelinePlaying;

    private float ScriptPlaySegment(float startTime, float duration, string videoName, string audioName, float audioVolume = 1.0f, float fadeDuration = 0.5f)
    {
        timeline.Add(new FadeEvent()
        {
            time = startTime,
            fadeDuration = fadeDuration,
            fadeIn = false
        });
        timeline.Add(new VideoEvent()
        {
            time = startTime,
            videoName = videoName,
            videoPlayer = videoPlayer
        });
        timeline.Add(new FadeEvent()
        {
            // + BUFFER to let the video load and start playing! It's hacky but it works. We are willing to take the low road here on team 6.
            time = startTime + fadeDuration + BUFFER,
            fadeDuration = fadeDuration,
            fadeIn = true
        });
        timeline.Add(new AudioEvent()
        {
            time = startTime + fadeDuration + BUFFER + fadeDuration + AUDIO_DELAY,
            audioName = audioName,
            audioVolume = audioVolume,
            audioSource = audioSource
        });

        return startTime + fadeDuration + BUFFER + duration;
    }

    private float ScriptPlayAudioClip(float startTime, string audioName, float audioDuration, float audioVolume = 1.0f)
    {
        timeline.Add(new AudioEvent()
        {
            time = startTime,
            audioName = audioName,
            audioVolume = audioVolume,
            audioSource = audioSource
        });

        return startTime + audioDuration;
    }

    private float ScriptShowInstructions(float startTime)
    {
        // TODO: Still working on this one!



        return startTime;
    }

    private float ScriptShowStartText(float startTime)
    {
        foreach (AnimatedText obj in experienceIntroductionTexts)
        {
            timeline.Add(new LoadObjectEvent()
            {
                time = startTime,
                obj = obj.gameObject,
                load = true
            });
        }

        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 0.01f,
            text = experienceIntroductionTexts[0],
            animateDuration = 1.5f,
            animateIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 2.5f,
            text = experienceIntroductionTexts[1],
            animateDuration = 1.5f,
            animateIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 5f,
            text = experienceIntroductionTexts[2],
            animateDuration = 1.5f,
            animateIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 8.5f,
            text = experienceIntroductionTexts[3],
            animateDuration = 1.5f,
            animateIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 8.5f,
            text = experienceIntroductionTexts[0],
            animateDuration = 1f,
            animateIn = false
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 9f,
            text = experienceIntroductionTexts[1],
            animateDuration = 1f,
            animateIn = false
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 9.5f,
            text = experienceIntroductionTexts[2],
            animateDuration = 1f,
            animateIn = false
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 11.5f,
            text = experienceIntroductionTexts[3],
            animateDuration = 1f,
            animateIn = false
        });

        return startTime + 12f;
    }

    private void ScriptShowChoiceScene(float startTime)
    {
        foreach (AnimatedText obj in choiceSceneObjects)
        {
            timeline.Add(new LoadObjectEvent()
            {
                time = startTime,
                obj = obj.gameObject,
                load = true
            });
        }

        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 0.01f,
            text = choiceSceneObjects[0],
            animateDuration = 3.0f,
            animateIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 0.5f,
            text = choiceSceneObjects[1],
            animateDuration = 3.0f,
            animateIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 0.5f,
            text = choiceSceneObjects[2],
            animateDuration = 3.0f,
            animateIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 6.0f,
            text = choiceSceneObjects[3],
            animateDuration = 2.0f,
            animateIn = true
        });
    }

    private float ScriptShowMuseumInstructions(float startTime)
    {
        // TODO: Still working on this one!

        return startTime;
    }

    private void ScriptShowMuseumScene(float startTime)
    {
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime,
            obj = museumSceneContainer,
            load = true
        });
    }

    void ScriptUnloadAllObjects(float startTime)
    {
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime,
            obj = instructionsContainer,
            load = false
        });
        foreach (AnimatedText obj in experienceIntroductionTexts)
        {
            timeline.Add(new LoadObjectEvent()
            {
                time = startTime,
                obj = obj.gameObject,
                load = false
            });
        }
        foreach (AnimatedText obj in choiceSceneObjects)
        {
            timeline.Add(new LoadObjectEvent()
            {
                time = startTime,
                obj = obj.gameObject,
                load = false
            });
        }
        foreach (AnimatedText obj in museumInstructionTexts)
        {
            timeline.Add(new LoadObjectEvent()
            {
                time = startTime,
                obj = obj.gameObject,
                load = false
            });
        }
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime,
            obj = museumSceneContainer,
            load = false
        });
    }

    public void SetupSceneAndOnward(string sceneName)
    {
        timeline.Clear();
        currentTime = 0;
        ScriptUnloadAllObjects(0.5f);
        audioSource.Stop();

        float t = 0.01f;

        switch(sceneName)
        {
            case "instructions":
                SetupInstructionsScene(t);
                break; // Give user time to get situated and choose when to start the experience
            case "start":
                t = SetupStartScene(t);
                goto case "eviction";
            case "eviction":
                t = SetupEvictionScene(t);
                goto case "morning";
            case "morning":
                t = SetupMorningScene(t);
                goto case "park";
            case "park":
                SetupParkScene(t);
                break; // Give user time to make a selection between the two choices
            case "chose_shelter":
                t = SetupChoseShelterScene(t);
                goto case "evening";
            case "chose_work":
                t = SetupChoseWorkScene(t);
                goto case "evening";
            case "evening":
                t = SetupEveningScene(t);
                goto case "final";
            case "final":
                SetupFinalScene(t);
                break; // Let user explore museum as long as they want
        }
    }

    abstract class Event
    {
        public float time;

        public abstract void Start();
    }

    class VideoEvent : Event
    {
        public string videoName;
        public VideoPlayer videoPlayer;

        public override void Start()
        {
            VideoClip video = Resources.Load<VideoClip>(videoName) as VideoClip;
            videoPlayer.clip = video;
            videoPlayer.frame = 0;
            // videoPlayer.SetDirectAudioVolume(0, 0.0f);
            videoPlayer.Play();
        }
    }

    class AudioEvent : Event
    {
        public string audioName;
        public float audioVolume;
        public AudioSource audioSource;

        public override void Start()
        {
            AudioClip audio = Resources.Load<AudioClip>(audioName) as AudioClip;
            audioSource.PlayOneShot(audio, audioVolume);
        }
    }

    class FadeEvent : Event
    {
        public float fadeDuration;
        public bool fadeIn;

        public override void Start()
        {
            Color toColor = (fadeIn) ? Color.clear : Color.black;
            SteamVR_Fade.Start(toColor, fadeDuration);
        }
    }

    class AnimateTextEvent : Event
    {
        public AnimatedText text;
        public float animateDuration;
        public bool animateIn;

        public override void Start()
        {
            if (animateIn)
            {
                text.Enter(animateDuration);
            } else
            {
                text.Exit(animateDuration);
            }
        }
    }

    class LoadObjectEvent : Event
    {
        // The object to be loaded or unloaded.
        public GameObject obj;

        // If true, load the object. If false, unload the object. (Also recursively applies to children)
        public bool load;

        public override void Start()
        {
            obj.SetActive(load);
        }
    }

    // Set up the experience
    void Start()
    {
        currentTime = 0;
        timeline = new List<Event>();

        // Initialize the video and audio player objects
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();

        // Initialize all scenes, using a helper function to start from the first scene
        // and initialize events for all scenes until the choice
        SetupSceneAndOnward("start");
    }

    // Update the user's current time in the experience, and
    // check if any new events need to be triggered based on the current time.
    void Update()
    {
        currentTime += Time.deltaTime;

        for (int i = 0; i < timeline.Count; i++)
        {
            Event ev = timeline[i];
            if (ev.time < currentTime)
            {
                ev.Start();
                timeline.RemoveAt(i);
                i--;
            }
        }
    }
}
