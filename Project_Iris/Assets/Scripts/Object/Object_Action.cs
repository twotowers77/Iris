using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Action : MonoBehaviour {

    public float speed = 30f;
    private Transform _transform;
    GameObject player;
    Rigidbody rigdbody;
    Vector3 forceDirection;

    ////////////////////[Override function]/////////////////////////
    void Awake()
    {
        _transform = GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player");
        rigdbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        rigdbody.velocity = Vector3.zero;
        rigdbody.angularVelocity = Vector3.zero;

    }
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
           
    }
    void OnTriggerStay(Collider other) //衝突体が重なっているときに発生する
    {   //Occurs when the collider continues to overlap.
        if (other.gameObject == player) {
            forceDirection = transform.position; // player의 x,y,z값을 forceDirection에 넣는다.

            forceDirection.x = player.transform.position.x > forceDirection.x ? -3f : 3f;// 기본 형식   (조건) ? (조건이 참일 때 수행) : (조건이 거짓일 때 수행) 구분은 ?와 : 을 사용함.
            forceDirection.y = 0;
            forceDirection.z = player.transform.position.z > forceDirection.z ? -3f : 3f;

            //rigdbody.AddForce(forceDirection * speed, ForceMode.Impulse);
        }
    }
}
