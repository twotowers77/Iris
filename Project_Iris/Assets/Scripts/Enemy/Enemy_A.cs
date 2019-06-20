using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_A : MonoBehaviour
{
	Animator Anim_;
    public GameObject enemy_1;
	private int AnimCnt;

	void Start()
	{
		Anim_ = GetComponent<Animator>();
		//AnimCnt = 0;
	}

	/*void Update()
	{
		AnimCnt++;
		if(AnimCnt <= 60)
		{
			AnimCnt = 0;
		}
		if(AnimCnt == 60)
		{

		}
	}*/

	protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CBP")
        {
				Change();
        }
    }
	protected virtual void Change()
	{

		Destroy(this.gameObject);
	
		Vector3 enV = this.gameObject.transform.position;
		Instantiate(this.enemy_1, enV, Quaternion.identity);
	}

}
