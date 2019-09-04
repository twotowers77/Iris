using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_B : MonoBehaviour
{
    protected GameObject[] goal_;        //二つのゴールの位置情報
    protected Transform[] goalTransform; //ゴールの位置
    protected float[] traceDist;         //モンスターとゴールの距離

    protected Transform monsterTrans;      //モンスター位置
    protected Transform targetTrans;       //ターゲットの位置（ゴール）
    protected int targetint = 0;

    public AudioClip Col_SE;
    AudioSource collision_SE;

    Animator Anim_;
    public GameObject enemy_2;         //次に出すオブジェクト
    protected Vector3 enPos;           //このオブジェクトの位置
    protected Vector3 hitPos;          //あたった位置

    protected NavMeshAgent nvAgent;

    void Start()
    {
        Anim_ = GetComponent<Animator>();
        collision_SE = GetComponent<AudioSource>();
        nvAgent = GetComponent<NavMeshAgent>();
        goal_ = GameObject.FindGameObjectsWithTag("goal");
        goalTransform = new Transform[goal_.Length];
        traceDist = new float[goal_.Length];

        for (int i = 0; i < goal_.Length; i++)
        {
            goalTransform[i] = goal_[i].GetComponent<Transform>();
        }

        targetTrans = goalTransform[0];
        StartCoroutine(SearchTarget());

    }

    void Update()
    {
        collision_SE.PlayOneShot(Col_SE);
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
            Score_Manager.score_C += 1;
            Score_Manager.score_E -= 1;
            Destroy(this.gameObject);
        }
    }
    protected virtual void Change()
    {
        Instantiate(this.enemy_2, enPos, transform.rotation);

        Destroy(this.gameObject);
    }
    IEnumerator SearchTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < goal_.Length; ++i)
            {
                traceDist[i] = Mathf.Abs(Vector3.Distance(goalTransform[i].position, transform.position));
            }
            targetTrans = goalTransform[0];
            if (goal_.Length == 1)
            {

            }
            else
            {
                for (int i = 0; i < goal_.Length - 1; i++)
                {
                    if (traceDist[targetint] <= traceDist[i + 1])
                    {
                        targetTrans = goalTransform[targetint];
                    }
                    else
                    {
                        targetint = i + 1;
                        targetTrans = goalTransform[targetint];
                    }
                }
            }
            nvAgent.destination = targetTrans.position;
            targetint = 0;
        }
    }

}

