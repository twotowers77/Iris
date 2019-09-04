using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBall : MonoBehaviour
{
    public AudioClip Col_SE;
    AudioSource colision_SE;

    private float speed = 2f;
    private float DestroyTime = 2.0f;
    private Transform _transform;

    GameObject _player;
    Rigidbody _rigdbody;
    Vector3 _forceDirection;

    public GameObject effect_hit;    //hit（インク）エフェクト

    // Start is called before the first frame update
    void Awake()
    {
        colision_SE = GetComponent<AudioSource>();
        _transform = GetComponent<Transform>();
        _rigdbody = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player");
        Destroy(gameObject, DestroyTime);
    }

    // Update is called once per frame
    void Update()
    {    }

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject == _player)
        {
            _forceDirection = transform.position;

            _forceDirection.x = _player.transform.position.x > _forceDirection.x ? -3f : 3f;
            _forceDirection.y = 0;
            _forceDirection.z = _player.transform.position.z > _forceDirection.z ? -3f : 3f;

            _rigdbody.AddForce(_forceDirection * speed, ForceMode.Impulse);   
        }
    }

       void OnCollisionEnter(Collision other)
    {    
        if (other.gameObject.tag == "obj" || other.gameObject.tag == "Ground" || other.gameObject.tag == "Enemy")
        {
            colision_SE.PlayOneShot(Col_SE);
            //エフェクトをあたった位置に出す
            foreach (ContactPoint point in other.contacts)
            {
                Vector3 hitPos;
                hitPos = point.point;
                Instantiate(this.effect_hit, point.point, transform.rotation);
            }
        }
		if (!(other.gameObject.tag == "CBP"))
        {
            Destroy(this.gameObject);
        }
    }

}
