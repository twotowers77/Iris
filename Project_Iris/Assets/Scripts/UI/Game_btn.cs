using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_btn : MonoBehaviour
{
    [SerializeField]
    Score_Manager Sm;
    public static int scoreMax;

    public AudioSource BGM1;
    public AudioSource BGM2;
    public AudioClip ResultSE;

    public GameObject countDown;
    public GameObject PauseUI;
    public GameObject ClearUI;
    public GameObject GameUI;
    public bool paused = false;
    public bool ClearPanel = false;
    public bool cntDown = true;
    public bool SEtemp = false;

    void Start()
    {
        GameObject[] gesu = GameObject.FindGameObjectsWithTag("Enemy");
        Sm = GetComponent<Score_Manager>();
        //BGM1.PlayOneShot(GameBGM1);
        scoreMax = gesu.Length;
        countDown.SetActive(true);
        PauseUI.SetActive(false);
        ClearUI.SetActive(false);
        GameUI.SetActive(true);
    }
    public void SumScore() {
        int Sum = Score_Manager.score_C;
        if(Sum == scoreMax)
        {
            ClearPanel = true;
            paused = false;
            PauseUI.SetActive(false);
        }
    }
    void Update()
    {
        SumScore();
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause")) {
            paused = !paused;
        }
        if (ClearPanel == true) {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("A"))
            {
                Restart();
            }
            if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("B"))
            {
                TitleMenu();
            }
        }
        if (cntDown)
        {
            StartCoroutine("StartDelay");
            cntDown = false;
        }

        if (Input.GetButtonDown("A") && Input.GetButtonDown("B") && Input.GetButtonDown("X") && Input.GetButtonDown("Y"))
		{
			TitleMenu();
		}
		if (Input.GetButtonDown("Fire1") && Input.GetButtonDown("A") && Input.GetButton("B"))
		{
			Quit();
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
			Time.timeScale = 0;
            PauseUI.SetActive(false);
        }

        if (ClearPanel == true)
        {
            if(SEtemp == false)
            {
                SEtemp = true;
                BGM1.Stop();
                BGM2.PlayOneShot(ResultSE);
            }
            GameUI.SetActive(false);
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

    IEnumerator StartDelay()
    {
        Time.timeScale = 0;
        float pauseTime = Time.realtimeSinceStartup + 4f;
        while (Time.realtimeSinceStartup < pauseTime)
            yield return 0;
        countDown.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
