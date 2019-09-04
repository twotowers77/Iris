﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_A : MonoBehaviour
{
    public AudioClip Col_SE;
    AudioSource collision_SE;

    Animator Anim_;
    public GameObject enemy_1;       //次に出すオブジェクト 
    public GameObject effect_smoke;  //煙エフェクト
    private int AnimCnt;
    Vector3 enPos;                   //このオブジェクトの位置

    bool hit = false;

    void Start()
    {
        collision_SE = GetComponent<AudioSource>();
        Anim_ = GetComponent<Animator>();
        AnimCnt = 0;
    }

    void Update()
    {
        AnimCnt++;
        int no = 0;
        if (AnimCnt >= 100)
        {
            no = Random.Range(0, 5);
            AnimCnt = 0;
        }
        if (no == 1)
        {
            animPlay();
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CBP")  //ボールにあたったら
        {
            if (hit == false)
            {
                hit = true;
                enPos = this.gameObject.transform.position;                 // オブジェクトの位置を取得
                Instantiate(this.effect_smoke, enPos, transform.rotation);  //煙エフェクト
                Invoke("Change", 2.0f);

                collision_SE.PlayOneShot(Col_SE);
            }
        }
    }
    //モンスターを出現させる
    protected virtual void Change()
    {
        Instantiate(this.enemy_1, enPos, transform.rotation);
        Destroy(this.gameObject);
    }
    //アニメーション再生（がたがた）
    protected void animPlay()
    {
        Anim_.Play("play", 0, 0.0f);
    }
}
