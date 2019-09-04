using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    public Slider _TimeGage;
    private float timeRemaining = 0;
    private float timerMax = 150f;
    
    Game_btn gb;

    void Start()
    {
        gb = GameObject.Find("Main Camera").GetComponent<Game_btn>();
        
    }
    void Update()
    {
        _TimeGage.value = CalculateSliderValue();

        if (gb.cntDown == false) {
            if(Time.timeScale == 1f)
            {
                if (timeRemaining >= 0)
                {
                    timeRemaining += Time.deltaTime;
                }
            }
        }
    }

    float CalculateSliderValue()
    {
        return timeRemaining;
    }
}
