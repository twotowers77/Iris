using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeManager : MonoBehaviour
{
    public Image fade;
    float fades = 1.0f;
    float time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (fades > 0.0f && time >= 0.1f)
        {
            fades -= 0.2f;
            fade.color = new Color(0, 0, 0, fades);
            time = 0.0f;
        }
        else if (fades <= 0.0f)
        {
            time = 0.0f;
        }      
    }
}