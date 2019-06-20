using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_btn : MonoBehaviour
{
    public GameObject PauseUI;
    public GameObject ClearUI;
    //public GameObject mamualUI;
    public bool paused = false;
    public bool ClearPanel = false;
   // private bool mamualPanel = true;
    // Start is called before the first frame update
    void Start()
    {
        PauseUI.SetActive(false);
        ClearUI.SetActive(false);
        // mamualUI.SetActive(true);
  }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause")) {
            paused = !paused;
        }
        if (paused == true)
        {
            PauseUI.SetActive(true);
            Time.timeScale = 0;
        }
        if (paused == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PauseUI.SetActive(false);
            Time.timeScale = 1f;
        }

        if (Time_count.LimitTime <= 0)
        {
            ClearPanel = true;
            paused = false;
            PauseUI.SetActive(false);
        }
		
        if (ClearPanel == true)
        {
            ClearUI.SetActive(true);
            Time.timeScale = 0;
        }
        
    }
    public void Resume()
    {
        paused = false;
    }
    public void Restart()
    {
        SceneManager.LoadScene("GameScenes");
    }

    public void TitleMenu()
    {
        SceneManager.LoadScene("Title");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OK()
    {
        //mamualPanel = false;
    }
}
