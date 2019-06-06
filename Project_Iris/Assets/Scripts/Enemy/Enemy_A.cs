using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_A : MonoBehaviour
{
    public GameObject enemy_1;


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
