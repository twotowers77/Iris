using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Btn : MonoBehaviour
{
    public AudioClip BGM;
    AudioSource TitleBGM;

    public Video_Manager VM;
    public Image fade;

    float fades = 0.0f;
    //float time = 0.0f;

    void Start()
    {
        TitleBGM = GetComponent<AudioSource>();
        VM = GameObject.Find("Video Player").GetComponent<Video_Manager>();
		Time.timeScale = 1f;
    }
    public void Gamescenes() {
        TitleBGM.PlayOneShot(BGM);
        SceneManager.LoadScene("GameScenes");
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        //time += Time.deltaTime;
        while (fades < 1.0f)
        {
            fades += 0.2f;
            fade.color = new Color(0, 0, 0, fades);
            yield return new WaitForSeconds(0.1f);
            //time = 0.0f;
        }
        Gamescenes();
        yield return null;
    }
}
