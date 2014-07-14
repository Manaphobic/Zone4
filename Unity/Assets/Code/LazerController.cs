using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LazerController : MonoBehaviour {

	public List<Animator> anims;
//	public Animator pixelGirlAnim;
//	private bool bRight = true;
	public float speed;
	public List<SpriteRenderer> hearts;
	public List<Sprite> sprites;
	private int hp = 3;
	public BoxCollider2D atkZone;

	private Vector2 direction = new Vector2(0,0);
	private Vector2 newDirection = new Vector2(0,0);
	private List<ComboBar> combo = new List<ComboBar>();
	public GameObject pref;
	public Transform heartContainer;
	public TextMesh scoreText;
	private int score;
	public int player;
	public int stock = 3;
	private bool charge;
	public Transform playerObj;
	// Use this for initialization
	void Start () 
	{
		scoreText.text = stock.ToString();
		player = GetInstanceID();
		atkZone.GetComponent<AtkScript>().player = player;
		SetHP();
		if ( GetComponent<NetworkView>().isMine  )
		{
			for (int i = 0; i < 3; i++) 
			{
				GameObject GO = (GameObject)Instantiate(pref,new Vector3(-2.5f+(i*2.5f),-2.25f,0),Quaternion.identity);
				GO.GetComponent<ComboBar>().gameRef = this;
				combo.Add( GO.GetComponent<ComboBar>());
			}
			combo[0].SetGlow();
			combo[0].ready = true;
		}
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
//			if ( Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1") )
//			{
//				Atk();
//			}

			direction = new Vector2(0,0);


			if ( Input.GetAxis( "Horizontal" ) > 0.2f )
				direction.x = 1;

			if ( Input.GetAxis( "Horizontal" ) < -0.2f )
				direction.x = -1;

			if ( Input.GetAxis( "Vertical" ) > 0.2f )
				direction.y = 1;
			
			if ( Input.GetAxis( "Vertical" ) < -0.2f )
				direction.y = -1;


			if ( Input.GetKeyDown(KeyCode.P))
				Reset();
//				RecieveDamage(1,-1,player);

			if(combo[0].ready && stock > 0)
			{
				if ( Input.GetKeyDown(KeyCode.I) || Input.GetButtonDown("Jump"))
				{
					TestCombo(0);
				}
				else if ( Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Fire2"))
				{
					TestCombo(1);
				}
				else if ( Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Fire1"))
				{
					TestCombo(2);
				}
				else if ( Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire3"))
				{
					TestCombo(3);
				}
			}
	
			if ( Input.GetKey(KeyCode.Alpha1))
				SetColor(231,100,15,191,36,0);
			if ( Input.GetKey(KeyCode.Alpha2))
				SetColor(69,209,229,33,182,176);
			if ( Input.GetKey(KeyCode.Alpha3))
				SetColor(255,244,158,165,251,255);
			if ( Input.GetKey(KeyCode.Alpha4))
				SetColor(139,140,249,247,101,255);
			if ( Input.GetKey(KeyCode.Alpha5))
				SetColor(41,41,41,142,142,142);



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

			if ( direction != newDirection && anims[0].GetCurrentAnimatorStateInfo(0).IsName("Damaged") == false )
			{
				Walk(direction.x,direction.y);
				newDirection = direction;
			}
		}

		if ( anims[0].GetCurrentAnimatorStateInfo(0).IsName("Atk") == false && anims[0].GetCurrentAnimatorStateInfo(0).IsName("Damaged") == false )
		{
			float chargeMod = 1;
			if ( charge ) 
				chargeMod = 0.4f;

			transform.position += new Vector3(Time.deltaTime*speed*direction.x*chargeMod,Time.deltaTime*speed*direction.y*chargeMod,0);
			if ( transform.position.y > 0.83f )
				transform.position = new Vector3( transform.position.x,0.83f,0);
			if ( transform.position.x > 4.78f )
				transform.position = new Vector3( 4.78f,transform.position.y,0);
			if ( transform.position.y < -2.4f )
				transform.position = new Vector3( transform.position.x,-2.4f,0);
			if ( transform.position.x < -4.78f )
				transform.position = new Vector3( -4.78f,transform.position.y,0);

			foreach (Animator item in anims) {
				item.GetComponent<SpriteRenderer>().sortingOrder = 1000 - Mathf.FloorToInt((transform.position.y*100));
			}

			for (int i = 0; i < hearts.Count; i++) 
			{
				hearts[i].sortingOrder = 1000 - Mathf.FloorToInt((transform.position.y*100));
			}
			
		}
	}

	private void TestCombo( int button )
	{
		if ( combo[0].ButtonPressed(button) )
		{
			Charge();
			if ( combo[0].CheckIfFinished() )
			{
				GetNextComboReady();
				EndCharge();
				Atk();
			}
		}
		else
		{
			GetNextComboReady();
			FailedCombo();
		}
	}	
	public void GetNextComboReady()
	{
		combo[0].bRemove = true;
//		Destroy(combo[0].gameObject);
		combo.RemoveAt(0);
		
		GameObject GO = (GameObject)Instantiate(pref,new Vector3(5f,-2.25f,0),Quaternion.identity);
		GO.GetComponent<ComboBar>().gameRef = this;
		combo.Add( GO.GetComponent<ComboBar>());
		
		for (int i = 0; i < combo.Count; i++) 
		{
			combo[i].bMove++;
		}
//		combo[0].SetGlow();
	}

	public void FailedCombo()
	{
		EndCharge();
	}
	
	[RPC] public void Walk( float x0, float y0 )
	{

		Vector2 dir = new Vector2(x0,y0);
		direction = dir;
		if ( dir.magnitude > 0 )
		{
			foreach (Animator item in anims )
				item.SetBool("Walking",true);
		}
		else
		{
			foreach (Animator item in anims )
				item.SetBool("Walking",false);
		}

		if ( transform.localScale.x != dir.x && dir.x != 0)
		{
//			if ( dir.x == 1 )
//			{
//				transform.position += new Vector3(0.25f,0,0);
////				heartContainer.position += new Vector3(0.25f,0,0);
//			}
//			else
//			{
//				transform.position += new Vector3(-0.25f,0,0);
////				heartContainer.position += new Vector3(-0.25f,0,0);
//			}
		}
		
		if ( dir.x != 0 )
			playerObj.localScale = new Vector3(dir.x,1,1);

		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("Walk",RPCMode.Others,dir.x,dir.y);
	}

	[RPC] public void Reset()
	{
		stock = 3;
		scoreText.text = stock.ToString();

		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("Reset",RPCMode.Others);
	}

	[RPC] public void Charge()
	{
		charge = true;
		
		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("Charge",RPCMode.Others);

		foreach (Animator item in anims )
			item.SetBool("Charge",true);
	}
	
	[RPC] public void EndCharge()
	{
		charge = false;
		
		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("EndCharge",RPCMode.Others);

		foreach (Animator item in anims )
			item.SetBool("Charge",false);
	}
	
	[RPC] public void Atk()
	{
		atkZone.enabled = true;

		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("Atk",RPCMode.Others);

		foreach (Animator item in anims )
			item.SetTrigger("Atk");
	}

	[RPC] public void GiveScore( int p0) 
	{
		if ( p0 == player )
		{
			score++;
		}

//		scoreText.text = score.ToString();

		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("GiveScore",RPCMode.Others,p0);
	}

	[RPC] public void RecieveDamage( int amount , float dirX, int p0) 
	{
		foreach (Animator item in anims )
			item.SetTrigger("Damage");

		hp -= amount;
		SetHP();
		if ( dirX == 1 )
			transform.position += new Vector3(0.5f,0,0);
		else
			transform.position -= new Vector3(0.5f,0,0);
		
		if ( hp <= 0 )
		{
			stock--;
			scoreText.text = stock.ToString();
			GiveScore(p0);
			transform.position = new Vector3(4.18f,0.67f,0);
			hp = 3;
			SetHP();
			transform.rotation = Quaternion.identity;
		}
		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("RecieveDamage",RPCMode.Others,amount,dirX,p0);
	}

	[RPC] public void SetColor( int R , int G ,int B, int R2 , int G2 ,int B2 ) 
	{
		anims[1].GetComponent<SpriteRenderer>().color = new Color(R/255f,G/255f,B/255f);
		anims[2].GetComponent<SpriteRenderer>().color = new Color(R2/255f,G2/255f,B2/255f);

		if ( GetComponent<NetworkView>().isMine )
			GetComponent<NetworkView>().RPC("SetColor",RPCMode.Others,R,G,B,R2,G2,B2);
	}
}
