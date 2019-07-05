using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Btn : MonoBehaviour
{
    public AudioClip SE;
    AudioSource buttonSE;
    public Video_Manager VM;
    void Start()
    {
        buttonSE = GetComponent<AudioSource>();
        VM = GameObject.Find("Video Player").GetComponent<Video_Manager>();
        
    }
    public void Gamescenes(){
        buttonSE.PlayOneShot(SE);
        Invoke("GS", 1f);
    }
    
    public void GS(){ SceneManager.LoadScene("GameScenes");  }

    void Update()
    {
        //if (Input.anyKey) SceneManager.LoadScene("Title");
        if (Input.GetButtonDown("Pause"))
        {
            Gamescenes();
        }
    }
}
