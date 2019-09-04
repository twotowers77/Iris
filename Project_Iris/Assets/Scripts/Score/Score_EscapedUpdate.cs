using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_EscapedUpdate : MonoBehaviour
{
    Text scoreLabel_E;
    
    void Awake()
    {
        scoreLabel_E = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreLabel_E.text = Score_Manager.score_E.ToString();
    }
}
