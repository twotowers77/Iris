using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_count : MonoBehaviour
{
    public static float LimitTime;
    public Text text_Timer;

    // Start is called before the first frame update
    void Start()
    {
        LimitTime = 120;
    }

    // Update is called once per frame
    void Update()
    {
        LimitTime -= Time.deltaTime;
        text_Timer.text = " " + Mathf.Round(LimitTime);
    }
}
