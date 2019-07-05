using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_C : Enemy_B
{
	Animator Anim_;
	public GameObject enemy_3;
    //protected Transform goalTransform;
    // Start is called before the first frame update
    void Start()
    {
		Anim_ = GetComponent<Animator>();
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
       base.OnCollisionEnter(collision);
    }
    protected override void Change()
    {
        Destroy(this.gameObject);

        Instantiate(this.enemy_3, enPos, transform.rotation);
    }
}
