using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject controls;
    bool showControls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
             SceneManager.LoadScene(0);
        }

        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showControls = !showControls;

            if (showControls)
            {
                controls.SetActive(true);
            }
            else
            {
                controls.SetActive(false);
            }
        }
    }
}
