using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStart : MonoBehaviour
{
    public string firstScene;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(firstScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
