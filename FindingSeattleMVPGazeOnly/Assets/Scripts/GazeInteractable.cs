using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class GazeInteractable : MonoBehaviour
{
    public GameObject[] showOnGaze;
    public GameObject[] hideOnGaze;
    public GameObject[] showOnSelect;
    public GameObject[] hideOnSelect;

    public enum Action { CHANGE_SCENE, NO_ACTION };
    public Action onSelectAction;
    public string onSelectNextScene;
    public GazeMenu gazeMenu;

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
