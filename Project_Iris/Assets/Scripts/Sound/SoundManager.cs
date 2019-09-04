using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip Col_SE;
    AudioSource collision_SE;

    // Start is called before the first frame update
    void Start()
    {
        collision_SE = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
                
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "CBP")
        {
            collision_SE.PlayOneShot(Col_SE);
        }
    }
}
