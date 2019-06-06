using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Captured Enemy Char

public class Score_Update : MonoBehaviour
{
    Text scoreLabel_C;

    // Start is called before the first frame update
    void Awake()
    {
        scoreLabel_C = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreLabel_C.text = Score_Manager.score_C.ToString();
    }
}
