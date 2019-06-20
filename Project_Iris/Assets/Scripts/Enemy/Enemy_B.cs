using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_B : Enemy_A
{
    Animator Anim_;
    public GameObject enemy_2;
    protected Transform goalTransform;

    void Start()
    {
        Anim_ = GetComponent<Animator>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        goalTransform = GameObject.FindWithTag("goal").GetComponent<Transform>();
        agent.destination = goalTransform.position;
    }

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
		Instantiate(this.enemy_2, enV, Quaternion.identity);
    }
}

