using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject obj;
    public GameObject en;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Instantiate(obj);
        GameObject.Instantiate(en);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
