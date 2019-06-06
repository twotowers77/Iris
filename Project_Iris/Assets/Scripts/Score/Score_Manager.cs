using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_Manager : MonoBehaviour
{
    public static int score_C;
    public static int score_E;

    void Awake()
    {
        score_C = 0; score_E = 0;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
