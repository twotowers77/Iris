using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_fin : MonoBehaviour
{
    Text scoreLabel_C;

    void Awake()
    {
        scoreLabel_C = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreLabel_C.text = (Score_Manager.score_C * 1000).ToString();
    }
}
