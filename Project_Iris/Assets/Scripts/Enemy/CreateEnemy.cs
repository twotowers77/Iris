using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    public Transform[] points;
    public GameObject MonsterPrefabs;
    public float CreateTime = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        points = GameObject.Find("spawnManager").GetComponentsInChildren<Transform>();
        StartCoroutine(this.CreateMonster());
    }
    IEnumerator CreateMonster()
    {
        while (true)
        {
            int idx = Random.Range(1, points.Length);
            Instantiate(MonsterPrefabs, points[idx].position, Quaternion.identity);

            yield return new WaitForSeconds(CreateTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CreateMonster();
    }
}
