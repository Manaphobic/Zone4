using UnityEngine;
using System.Collections;

public class SortThisObject : MonoBehaviour {

	public float offsetY;

	// Use this for initialization
	void Start () 
	{
		GetComponent<SpriteRenderer>().sortingOrder = 1000 - Mathf.FloorToInt(((transform.position.y+offsetY)*100));
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SpriteRenderer>().sortingOrder = 1000 - Mathf.FloorToInt(((transform.position.y+offsetY)*100));
	}
}
