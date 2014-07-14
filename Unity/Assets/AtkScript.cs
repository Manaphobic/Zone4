﻿using UnityEngine;
using System.Collections;

public class AtkScript : MonoBehaviour {
	
	private float timer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( timer > 0 )
			timer -= Time.deltaTime;
		else
			GetComponent<BoxCollider2D>().enabled = false;
	}

	public void Atk()
	{
		GetComponent<BoxCollider2D>().enabled = true;
		timer = 0.1f;
	}

	void OnCollisionEnter2D(Collision2D coll) 
	{
		if (coll.gameObject.tag == "Player")
		{
			GetComponent<BoxCollider2D>().enabled = false;
			coll.transform.GetComponent<LazerController>().RecieveDamage(1,transform.parent.transform.localScale.x);
		}
	}
}
