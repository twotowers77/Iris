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
        score_C = 0;
        GameObject[] gesu = GameObject.FindGameObjectsWithTag("Enemy");
        score_E = gesu.Length;
    }
    // Update is called once per frame
    void Update()
    {
    }
}
