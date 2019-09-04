using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_D : Enemy_B
{
    Animator Anim_;
    public GameObject effect_kieru;

    public AudioClip kieruSE;
    AudioSource audioSource;

    bool hit = false;

    // protected Transform goalTransform;
    // Start is called before the first frame update
    void Start()
    {
        Anim_ = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
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

    // Update is called once per frame
    void Update()
    {

    }
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CBP")
        {
            if (hit == false)
            {
                hit = true;
                enPos = this.gameObject.transform.position;
                Score_Manager.score_C += 1;
                Score_Manager.score_E -= 1;

                audioSource.PlayOneShot(kieruSE);
                Instantiate(this.effect_kieru, enPos, Quaternion.identity);


                Invoke("Destroy", 0.5f);
            }
        }
        if (collision.gameObject.tag.Equals("goal"))
        {
            Score_Manager.score_C += 1;
            Score_Manager.score_E -= 1;
            Destroy(this.gameObject);
        }
    }
    private void Destroy()
    {
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
