using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_A : MonoBehaviour
{
    Animator Anim_;
    public GameObject enemy_1;       //次に出すオブジェクト 
    public GameObject effect_smoke;  //煙エフェクト
    private int AnimCnt;
    Vector3 enPos;                   //このオブジェクトの位置

    void Start()
    {
        Anim_ = GetComponent<Animator>();
        AnimCnt = 0;
    }

    void Update()
    {
        //AnimCnt++;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Anim_.SetBool("isPlay", true);
        }
        else
        {
            Anim_.SetBool("isPlay", false);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CBP")  //ボールにあたったら
        {
            enPos = this.gameObject.transform.position;                 // オブジェクトの位置を取得
            Instantiate(this.effect_smoke, enPos, transform.rotation);  //煙エフェクト
            Invoke("Change", 1);
        }
    }
    //モンスターを出現させる
    protected virtual void Change()
    {
        Instantiate(this.enemy_1, enPos, transform.rotation);
        Destroy(this.gameObject);
    }
}
