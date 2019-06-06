using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_C : Enemy_A
{
    public GameObject enemy_3;
    protected Transform goalTransform;
    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        goalTransform = GameObject.FindWithTag("goal").GetComponent<Transform>();
        agent.destination = goalTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CBP")
        {
            Change();
        }

        if (collision.gameObject.tag.Equals("goal"))
        {
            Score_Manager.score_E += 1;
            Destroy(this.gameObject);
        }
    }
    protected override void Change()
    {
        Destroy(this.gameObject);

        Vector3 enV = this.gameObject.transform.position;
        Instantiate(this.enemy_3, enV, Quaternion.identity);
    }
}
