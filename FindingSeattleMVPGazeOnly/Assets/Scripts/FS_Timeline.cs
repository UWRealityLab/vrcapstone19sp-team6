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
    public MuseumExhibit[] museumExhibits;
    public GameObject museumSceneContainer;
    public GameObject menu;
    public GameObject camera;
    public GameObject player;

    void SetupInstructionsScene(float t)
    {
        ScriptShowInstructions(t);

        // No return -- give the user time to situate themselves before choosing to go on
    }

    float SetupStartScene(float t)
    {
        ScriptPlayAudioClip(t + 8, "music/intro_piano_music", 24, 0.7f);
        t = ScriptPlaySegment(t, 16.75f, "morning/small-room-converted", "narration/24", 0.7f);
        t = ScriptShowStartText(t);

        return t;
    }

    float SetupEvictionScene(float t)
    {
        t = ScriptPlaySegment(t, 21, "eviction/outside-apartment-converted", "other_audio/bank-knocking-shortened", videoVolume: 1.0f);
        t = ScriptPlaySegment(t, 11, "eviction/hall-short-converted", "narration/short1");
        t = ScriptPlaySegment(t, 6, "eviction/zillow-scroll-converted", "narration/short3");
        t = ScriptPlaySegment(t, 12, "eviction/scroll-shelter-converted", "narration/4");
        ScriptPlayAudioClip(t + 1, "other_audio/doorknock", 5, 0.5f);
        t = ScriptPlaySegment(t, 9, "eviction/eviction-notice-injected-converted", "narration/short5");

        return t;
    }

    float SetupMorningScene(float t)
    {
        t = ScriptPlaySegment(t, 8, "morning/outside-converted", "narration/6");
        t = ScriptPlaySegment(t, 13, "morning/front-desk-converted", "narration/short7");
        t = ScriptPlaySegment(t, 10, "morning/bathroom-converted", "narration/8alternate");
        ScriptPlayAudioClip(t, "other_audio/background-talking", 10, 0.9f);
        t = ScriptPlaySegment(t, 10, "morning/big-room-long-converted", "narration/9");

        return t;
    }

    void SetupParkScene(float t)
    {
        t = ScriptPlaySegment(t, 8, "park/on-bed-converted", "narration/short11");
        ScriptPlayAudioClip(t, "other_audio/birds", 45, 0.5f);
        t = ScriptPlaySegment(t, 9, "park/park1-converted", "narration/short12");
        t = ScriptPlaySegment(t, 21, "park/park2-converted", "narration/13");
        t = ScriptPlaySegment(t, 16, "park/park_choice-injected-converted", "narration/21alternate-with-text");
        ScriptShowChoiceScene(t);

        // No return -- give the user time to make a choice
    }

    float SetupChoseShelterScene(float t)
    {
        ScriptPlayAudioClip(t, "other_audio/text-sent", 1, 0.5f);
        t = ScriptPlaySegment(t + 1, 16, "morning/hallway", "narration/23alternate");

        return t;
    }

    float SetupChoseWorkScene(float t)
    {
        ScriptPlayAudioClip(t, "other_audio/text-sent", 1, 0.5f);
        t = ScriptPlaySegment(t + 1, 14, "park/chose-work-converted", "narration/22alternate");

        return t;
    }

    float SetupEveningScene(float t)
    {
        t = ScriptPlaySegment(t, 12, "evening/entrance-converted", "narration/short14");
        t = ScriptPlaySegment(t, 16, "evening/bunkbed-converted", "narration/15");
        t = ScriptPlaySegment(t, 7, "evening/lockers-converted", "narration/16");
        t = ScriptPlaySegment(t, 6, "evening/laundry-converted", "narration/short17-18");
        ScriptPlayAudioClip(t + 7, "music/end_piano_music", 28);
        t = ScriptPlaySegment(t, 12, "evening/shower-converted", "narration/19");
        t = ScriptPlaySegment(t, 24, "evening/success-converted", "narration/25new");

        return t;
    }

    void SetupFinalScene(float t)
    {
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

    private float ScriptPlaySegment(float startTime, float duration, string videoName, string audioName, float videoVolume = 0.0f, float audioVolume = 1.0f, float fadeDuration = 0.5f)
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
            videoPlayer = videoPlayer,
            videoVolume = videoVolume
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

    private void ScriptShowInstructions(float startTime)
    {
        timeline.Add(new FadeEvent()
        {
            time = startTime,
            fadeDuration = 0.5f,
            fadeIn = true
        });
        timeline.Add(new VideoEvent()
        {
            time = startTime,
            videoName = "instructions/stairs",
            videoPlayer = videoPlayer,
            videoVolume = 0.0f
        });
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime + 0.5f,
            obj = instructionsContainer,
            load = true
        });
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
        timeline.Add(new FadeEvent()
        {
            time = startTime,
            fadeDuration = 0.5f,
            fadeIn = false
        });
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime + 0.8f,
            obj = museumSceneContainer,
            load = true
        });
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime + 0.8f,
            obj = museumInstructionTexts[0].gameObject,
            load = true
        });
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime + 0.8f,
            obj = museumInstructionTexts[1].gameObject,
            load = true
        });
        timeline.Add(new FadeEvent()
        {
            // + BUFFER to let the video load and start playing! It's hacky but it works. We are willing to take the low road here on team 6.
            time = startTime + 0.8f + BUFFER,
            fadeDuration = 0.5f,
            fadeIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 1.0f + BUFFER,
            text = museumInstructionTexts[0],
            animateDuration = 2.5f,
            animateIn = true
        });
        timeline.Add(new AnimateTextEvent()
        {
            time = startTime + 1.0f + BUFFER + 4.5f,
            text = museumInstructionTexts[1],
            animateDuration = 2.5f,
            animateIn = true
        });

        return startTime + 1.0f + BUFFER + 7.0f;
    }

    private void ScriptShowMuseumScene(float startTime)
    {
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime + 0.5f,
            obj = menu,
            load = true
        });
        for (int i = 0; i < museumExhibits.Length; i++)
        {
            timeline.Add(new MuseumExhibitEvent()
            {
                time = startTime + 0.1f + (i * 0.2f),
                exhibit = museumExhibits[i],
                animateDuration = 0.7f,
                animateIn = true
            });
        }
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
            obj = museumInstructionTexts[0].gameObject,
            load = false
        });
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime,
            obj = museumInstructionTexts[1].gameObject,
            load = false
        });
        timeline.Add(new LoadObjectEvent()
        {
            time = startTime + 0.02f,
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
        public float videoVolume;

        public override void Start()
        {
            VideoClip video = Resources.Load<VideoClip>(videoName) as VideoClip;
            videoPlayer.clip = video;
            videoPlayer.frame = 0;

            if (videoVolume == 0.0f)
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            }
            else
            {
                videoPlayer.SetDirectAudioVolume(0, videoVolume);
                videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            }

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

    class MuseumExhibitEvent : Event
    {
        public MuseumExhibit exhibit;
        public float animateDuration;
        public bool animateIn;

        public override void Start()
        {
            if (animateIn)
            {
                exhibit.Enter(animateDuration);
            }
            else
            {
                exhibit.Exit(animateDuration);
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

        UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        ResetCamera();

        menu.SetActive(false);

        // Initialize all scenes, using a helper function to start from the first scene
        // and initialize events for all scenes until the choice
        SetupSceneAndOnward("instructions");
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

        CheckKeypress();
    }

    void CheckKeypress()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            menu.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            menu.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetCamera();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetupSceneAndOnward("instructions");
        }
    }

    void ResetCamera()
    {
        OpenVR.System.ResetSeatedZeroPose();
        OpenVR.Compositor.SetTrackingSpace(ETrackingUniverseOrigin.TrackingUniverseSeated);
        Vector3 cameraPosition = new Vector3(player.transform.position.x, player.transform.position.y + 2.1f, player.transform.position.z);
        camera.transform.SetPositionAndRotation(cameraPosition, player.transform.rotation);
    }
}
