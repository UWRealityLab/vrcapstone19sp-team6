using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Valve.VR.Extras;

public class LaserPointerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string targetScene;

    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
    }

    void Update()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Renderer>().material.color = Color.grey;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Renderer>().material.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetScene != null)
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
