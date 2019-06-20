using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_D : Enemy_A
{
	Animator Anim3_;
    protected Transform goalTransform;
    // Start is called before the first frame update
    void Start()
    {
		Anim3_ = GetComponent<Animator>();
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
            Score_Manager.score_C += 1;
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag.Equals("goal"))
        {
            Score_Manager.score_E += 1;
            Destroy(this.gameObject);
        }
    }
}
