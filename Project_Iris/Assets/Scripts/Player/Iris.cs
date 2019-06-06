using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iris : MonoBehaviour
{
    //Resource 
    public AudioClip _AttackSE;
    AudioSource _shotSE;

    //Object
    private Animator _animator;
    private Transform _transform;
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;
    Rigidbody _rigdbody;    

    [SerializeField]
    private MouseRotate m_MouseRotate;
    [SerializeField]
    private Camera m_Camera;
    //----------------------------------------
    //Check the isRun
    //----------------------------------------    
    private float moveSpeed;
    private float walkSpeed = 20.0f;
    private float runSpeed = 30.0f;
    //----------------------------------------
    private float rotateSpeed = 100.0f;
    private float range = 100.0f;
    private float jumpForce = 30f;
    private int jumpCount;
    //----------------------------------------
    //State Variable
    //----------------------------------------
    private bool isGround;
    private bool isRun;

    public GameObject _ColorBallPrefabs;
    public GameObject _player;
    public GameObject _AimPoint;
    //public GameObject _eyes;            //High probability of not using it

    // Start is called before the first frame update
    void Start()
    {
        _shotSE = GetComponent<AudioSource>();
        _transform = GetComponent<Transform>();
        _rigdbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>(); //Animatorコンポーネントはplayer objectの下位に存在するため、InChildren使用
        jumpCount = 1; //Jump Max 

        //Camera/Rotate
        m_Camera = Camera.main;
        m_MouseRotate.Init(transform, m_Camera.transform);


        _player = GameObject.FindGameObjectWithTag("Player");
        _AimPoint = GameObject.FindGameObjectWithTag("AimPoint");
        //_eyes = GameObject.FindGameObjectWithTag("eyes");

        isGround = true;
        isRun = false;
        moveSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(_AimPoint.transform.position, _AimPoint.transform.forward * range, Color.red);
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        Vector3 moveDirect = (Vector3.forward * _vertical) + (Vector3.right * _horizontal);

        _transform.Translate(moveDirect.normalized * Time.deltaTime * moveSpeed, Space.Self);
        _transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * Input.GetAxis("Mouse X"));

        if (_vertical >= 0.1f) { _animator.SetBool("isWalk", true); }
        else if (_vertical <= -0.1f) { _animator.SetBool("isWalk", true); }
        else if (_horizontal >= 0.1f) { _animator.SetBool("isWalk", true); }
        else if (_horizontal <= -0.1f) { _animator.SetBool("isWalk", true); }
        else { _animator.SetBool("isWalk", false); }

        if (Input.GetMouseButtonDown(0) && Time.timeScale == 1f)
        {
            _shotSE.PlayOneShot(_AttackSE);
            ColorBallShot();
        }

        if(isGround == true)
        {
            Run();
            jumpCount = 1;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(jumpCount == 1)
                {
                    _rigdbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                    isGround = false;
                    jumpCount = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        m_MouseRotate.UpdateCursorLock();
        RotateView();
    }
    private void ColorBallShot()
    {
        GameObject instantBall = (GameObject)Instantiate(_ColorBallPrefabs, _AimPoint.transform.position, _AimPoint.transform.rotation);
        Rigidbody rigidbody = instantBall.GetComponent<Rigidbody>();
        rigidbody.AddForce(_AimPoint.transform.forward * 50f, ForceMode.Impulse);
    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
            moveSpeed = runSpeed;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRun = false;
            moveSpeed = walkSpeed;
        }
    }

    private void RotateView()
    {
        m_MouseRotate.LookRotation(transform, m_Camera.transform);
    }
}
