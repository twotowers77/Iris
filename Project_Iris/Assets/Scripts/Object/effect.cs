using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effect : MonoBehaviour
{
    int cnt;
    // Start is called before the first frame update
    void Start()
    {
        cnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        cnt++;
        if (cnt >= 100)
        {
            Destroy(this.gameObject);
        }
    }
}
