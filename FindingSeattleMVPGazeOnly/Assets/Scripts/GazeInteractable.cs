using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInteractable : MonoBehaviour
{
    public GameObject[] showOnGaze;
    public GameObject[] hideOnGaze;
    public GameObject[] showOnSelect;
    public GameObject[] hideOnSelect;

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
}
