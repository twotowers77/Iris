using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    public enum CurrentState { idle, runAway, attack, dead }; //列挙型"状態"変数
    public CurrentState curState = CurrentState.idle;

    private Transform _transform;
    private Transform targetTransform;
    //private NavMeshAgent nvAgent;

    public float traceDist = 15.0f;//追跡射程
    public bool isDead = false;//死亡有無
    private int LifeCount = 0; 
    // Start is called before the first frame update
    void Start()
    {
        _transform = this.gameObject.GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //nvAgent = this.gameObject.GetComponent<NavMeshAgent>();



        //アニメーション適用後に使用することそれぞれの状態をアニメーション設定する必要がある。
        //StartCoroutine(this.CheckState());
        //StartCoroutine(this.CheckStateForAction());
    }
    void FixedUpdate()
    {
        //追跡対象の位置を設定するとすぐ追跡スタート
        //nvAgent.destination = targetTransform.position;
    }
    /*
    IEnumerator CheckState()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.5f);

            //Vector3.Distance(Vector3 a, Vector3 b);
            //a, bの間の距離を測定して返す関数で私たちが作成したコードと
            //プレーヤの現在の位置とモンスターの現在の位置は同じ位置を持っている
            float dist = Vector3.Distance(targetTransform.position, _transform.position);


            if (dist <= traceDist)
            {
                curState = CurrentState.runAway;
            }
            else
            {
                curState = CurrentState.idle;
            }
        }
    }
    /*IEnumerator CheckStateForAction()
    {
        while (!isDead)
        {
            switch (curState)
            {
                case CurrentState.idle:
                    nvAgent.Stop(); //NavMeshAgent Component Stop 逃亡中止
                    break;
                case CurrentState.runAway:
                    nvAgent.destination = targetTransform.position;
                    nvAgent.Resume(); //NavMeshAgent Component Resume 初位置に戻る.
                    break;
                case CurrentState.attack:
                    break;
            }
            yield return null;
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "ColorBallPrefabs")
        {
            LifeCount++;
        }
        if (LifeCount == 3 || col.gameObject.tag.Equals("Player"))
        {
            Destroy(this.gameObject);
            LifeCount = 0;
        }
    }
}
