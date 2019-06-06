using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Btn : MonoBehaviour
{
    public AudioClip SE;
    AudioSource buttonSE;

    void Start()
    {
        buttonSE = GetComponent<AudioSource>();
        
    }
    public void Button(){
        buttonSE.PlayOneShot(SE);
        Invoke("GS", 1f);
    }
    
    public void GS(){ SceneManager.LoadScene("GameScenes");  }

    void Update()
    {
        //if (Input.anyKey) SceneManager.LoadScene("Title");

    }
}
