using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LazerController : MonoBehaviour {

	public Animator pixelGirlAnim;
//	private bool bRight = true;
	public float speed;
	public List<SpriteRenderer> hearts;
	public List<Sprite> sprites;
	private int hp = 3;
	public BoxCollider2D atkZone;

	private Vector2 direction = new Vector2(0,0);
	private Vector2 newDirection = new Vector2(0,0);

	// Use this for initialization
	void Start () 
	{
		SetHP();
	}

	private void SetHP()
	{
		for (int i = 0; i < hearts.Count; i++) 
		{
			if ( hp > i )
				hearts[i].sprite = sprites[1];
			else 
				hearts[i].sprite = sprites[0];
		}
	}


	// Update is called once per frame
	void Update () 
	{
		if ( GetComponent<NetworkView>().isMine  )
		{
			if ( Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1") )
			{
				Atk();
			}

			direction = new Vector2(0,0);


			if ( Input.GetAxis( "Horizontal" ) > 0.2f )
				direction.x = 1;

			if ( Input.GetAxis( "Horizontal" ) < -0.2f )
				direction.x = -1;

			if ( Input.GetAxis( "Vertical" ) > 0.2f )
				direction.y = 1;
			
			if ( Input.GetAxis( "Vertical" ) < -0.2f )
				direction.y = -1;



			if ( Input.GetKey(KeyCode.W))
			{
				direction.y = 1;
			}
			if ( Input.GetKey(KeyCode.S))
			{
				direction.y = -1;
			}
			if ( Input.GetKey(KeyCode.D))
			{
				direction.x = 1;
			}
			if ( Input.GetKey(KeyCode.A))
			{
				direction.x = -1;
			}

			if ( direction != newDirection )
			{
				Walk(direction.x,direction.y);
				newDirection = direction;
			}
		}

		if ( pixelGirlAnim.GetCurrentAnimatorStateInfo(0).IsName("Atk") == false )
		{
			pixelGirlAnim.transform.position += new Vector3(Time.deltaTime*speed*direction.x,Time.deltaTime*speed*direction.y,0);
			if ( pixelGirlAnim.transform.position.y > 0 )
				pixelGirlAnim.transform.position = new Vector3( pixelGirlAnim.transform.position.x,0,0);
		}
	}

	[RPC] public void Walk( float x0, float y0 )
	{

		Vector2 dir = new Vector2(x0,y0);
		direction = dir;
		if ( dir.magnitude > 0 )
			pixelGirlAnim.SetBool("Walking",true);
		else
			pixelGirlAnim.SetBool("Walking",false);

		if ( dir.x != 0 )
			pixelGirlAnim.transform.localScale = new Vector3(dir.x,1,1);

		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("Walk",RPCMode.Others,dir.x,dir.y);
	}

	[RPC] public void Atk()
	{
		atkZone.enabled = true;

		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("Atk",RPCMode.Others);

		pixelGirlAnim.SetTrigger("Atk");
	}

	[RPC] public void RecieveDamage( int amount , float dirX) 
	{
		hp -= amount;
		SetHP();
		if ( dirX == 1 )
			transform.position += new Vector3(0.1f,0,0);
		else
			transform.position -= new Vector3(0.1f,0,0);
		
		if ( hp <= 0 )
		{
			transform.position = new Vector3(-0.89f,transform.position.y,0);
			hp = 3;
			SetHP();
			transform.rotation = Quaternion.identity;
		}
		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("RecieveDamage",RPCMode.Others,amount,dirX);
	}

}
