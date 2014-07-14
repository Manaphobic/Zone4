using UnityEngine;
using System.Collections;

public class girlController : MonoBehaviour {

	public Animator pixelGirlAnim;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown(KeyCode.Space))
		{
			pixelGirlAnim.SetTrigger("Atk");
		}
	}
}
