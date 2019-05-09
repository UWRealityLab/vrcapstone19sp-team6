using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInButtons : MonoBehaviour
{
    private GameObject[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        buttons = GameObject.FindGameObjectsWithTag("Choice Button");
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
        Invoke("ActiveButtons", 5.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ActiveButtons()
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }
    }
}
