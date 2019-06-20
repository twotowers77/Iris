using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Video_Manager : MonoBehaviour
{
    private VideoPlayer _video;
    private AudioSource _audio;
    //RawImage Image;
    // Start is called before the first frame update
    void Awake()
    {
        //Image = GetComponent<RawImage>();
        _video = gameObject.GetComponent<VideoPlayer>();
        _audio = gameObject.GetComponent<AudioSource>();
        _video.playOnAwake = false;
        _audio.playOnAwake = false;
        _audio.Pause();
        PlayVideo();
    }
    public void PlayVideo()
    {
        StartCoroutine(playVideo());
    }
    IEnumerator playVideo()
    {
        _video.audioOutputMode = VideoAudioOutputMode.AudioSource;

        _video.EnableAudioTrack(0, true);
        _video.SetTargetAudioSource(0, _audio);
        _video.Prepare();
        WaitForSeconds waitTime = new WaitForSeconds(10.0f);
        while (!_video.isPrepared)
        {
            Debug.Log("now Loading..");
            yield return waitTime;
        }

        _video.Play();
        _audio.Play();
        while (_video.isPlaying)
        {
            if (Input.anyKey) SceneManager.LoadScene("Title");
            Debug.Log(" Time : " + Mathf.FloorToInt((float)_video.time));
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
