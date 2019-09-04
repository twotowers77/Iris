using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_count : MonoBehaviour
{
    public static float LimitTime;
   // public Slider _TimeGage;
    public Text _textTimer;
    Game_btn GB;

    //float _tempTime;

    // Start is called before the first frame update
    void Start()
    {
        GB = GameObject.Find("Main Camera").GetComponent<Game_btn>();
     //   _tempTime = 0;
        LimitTime = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (GB.cntDown == false) {
            if (Time.timeScale == 1f) {
                LimitTime -= Time.deltaTime;
                _textTimer.text = " " + Mathf.Round(LimitTime);
            }
        }
    }
    /*public void SetLimitTime() {
        _tempTime += 1;
        if (_tempTime == LimitTime)
        {
            _TimeGage.value -= LimitTime;
        }
    }*/
}
