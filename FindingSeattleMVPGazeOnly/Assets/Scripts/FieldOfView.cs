using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Camera camera;
    private float windowaspect;
    private float targetaspect;
    private float scaleheight;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("entered start");
        camera = GetComponent<Camera>();
        windowaspect = (float)Screen.width / (float)Screen.height;
        targetaspect = 12.0f / 9.0f;
        scaleheight = windowaspect / targetaspect;
        scale();
   
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    void scale()
    {
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        } 
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
        Debug.Log("scaled fine");

    }
}
