using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_B : MonoBehaviour
{
    Animator Anim_;
    public GameObject enemy_2;         //次に出すオブジェクト
    protected Transform goalTransform; //ゴールの位置
	protected Vector3 enPos;           //このオブジェクトの位置
    protected Vector3 hitPos;          //あたった位置

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

    protected virtual void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.tag == "CBP")
		{
            enPos = this.gameObject.transform.position;
			Change();
		}
		if (collision.gameObject.tag.Equals("goal"))
		{
			Score_Manager.score_E += 1;
			Destroy(this.gameObject);
		}
	}
    protected virtual void Change()
    {		
		Instantiate(this.enemy_2, enPos, transform.rotation);

		Destroy(this.gameObject);
    }
}

