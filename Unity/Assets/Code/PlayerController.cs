using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public List<Animator> anims;		//lista på animationerna
	public float speed;					//hur snabbt karaktären rör sig
	public List<SpriteRenderer> hearts;	//lista på hjärtan
	public List<Sprite> HeartSprites;	//bilder för två hjärtan bilder
	private int hp = 3;					//hur mycket hp man har
	public BoxCollider2D atkZone;		//collidern som bestämmer när och hur man träffar
	private bool rainbowColored = false;//special color ftw

	private Vector2 direction = new Vector2(0,0);		//vilken riktning man är påväg
	private Vector2 newDirection = new Vector2(0,0);	//den nya riktningen man är påväg just nu
	private List<ComboBar> combo = new List<ComboBar>();	//sparar alla comboBar så vi kan hålla reda på dem
	public GameObject comboPref;		//vår kombobar som vi instantierar		
	public TextMesh scoreText;			//den här är en textruta som just nu används för att räkna Stocks
	public int playerID;				//håller koll på vilket id vi har, så när någon dör så vet den vem som gjorde det
	public int stock = 3;				//hur många stocks man har
	private bool charge;				//håller koll på om man just nu laddar sitt vapen
	public Transform playerObj;			//denna container skalas från 1 till -1 när man vänder sig om

	//GetComponent Cache
	private NetworkView thisNetworkView;
	
	void Start()
	{
		scoreText.text = stock.ToString();
		playerID = GetInstanceID();
		atkZone.GetComponent<AtkScript>().playerID = playerID;
		SetHP();
		thisNetworkView = GetComponent<NetworkView>();
		
		if ( thisNetworkView.isMine )
		{
			//skapa combobar
			for (int i = 0; i < 3; i++) 
			{
				GameObject GO = (GameObject)Instantiate(comboPref,new Vector3(-2.5f+(i*2.5f),-2.25f,0),Quaternion.identity);
				GO.GetComponent<ComboBar>().gameRef = this;
				combo.Add( GO.GetComponent<ComboBar>());
			}
			combo[0].SetGlow();
			combo[0].ready = true;
		}
	}

	//den kollar bara hur mycket hp man har och aktiverar rätt hjärtan
	private void SetHP()
	{
		for (int i = 0; i < hearts.Count; i++) 
		{
			if ( hp > i )
				hearts[i].sprite = HeartSprites[1];
			else 
				hearts[i].sprite = HeartSprites[0];
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if ( thisNetworkView.isMine  )
		{
			//vi nollställer direction, så vi kan se ifall vi har bytt riktning sen senast med NewDirection
			direction = new Vector2(0,0);
	
			//move 
			direction.x = Input.GetAxis( "Horizontal" );
			direction.y = Input.GetAxis( "Vertical" );
			direction.Normalize();
			
			//special för att resetta Stocks
			if ( Input.GetKeyDown(KeyCode.P))
				Reset();

			//testar att göra combos
			if(combo[0].ready && stock > 0)
			{
				if ( Input.GetButtonDown("Y"))
				{
					TestCombo(0);
				}
				else if ( Input.GetButtonDown("B"))
				{
					TestCombo(1);
				}
				else if ( Input.GetButtonDown("A"))
				{
					TestCombo(2);
				}
				else if ( Input.GetButtonDown("X"))
				{
					TestCombo(3);
				}
			}

			//sätter färg på spelaren
			if ( Input.GetKeyDown(KeyCode.Alpha1) )
				SetColor(231,100,15,191,36,0);
			if ( Input.GetKeyDown(KeyCode.Alpha2))
				SetColor(69,209,229,33,182,176);
			if ( Input.GetKeyDown(KeyCode.Alpha3))
				SetColor(255,244,158,165,251,255);
			if ( Input.GetKeyDown(KeyCode.Alpha4))
				SetColor(139,140,249,247,101,255);
			if ( Input.GetKeyDown(KeyCode.Alpha5))
				SetColor(41,41,41,142,142,142);
			if( Input.GetKeyDown( KeyCode.F9 ) ) //SHSHSHHSHH, SECRET DONT LOOK
			{
				rainbowColored = !rainbowColored;
				if( rainbowColored )
					StartCoroutine( "RainbowColor" );
				else
					StopCoroutine( "RainbowColor" );
			}

			//om vi går i en ny riktning så måste det uppdateras på alla spelare
			if ( direction != newDirection && anims[0].GetCurrentAnimatorStateInfo(0).IsName("Damaged") == false )
			{
				Walk(direction.x,direction.y);
				newDirection = direction;
			}
		}

		//om vi kan så ska vi flytta på oss
		if ( anims[0].GetCurrentAnimatorStateInfo(0).IsName("Atk") == false && anims[0].GetCurrentAnimatorStateInfo(0).IsName("Damaged") == false )
		{
			float chargeMod = 1;
			if ( charge ) 
				chargeMod = 0.4f;

			transform.position += new Vector3(Time.deltaTime*speed*direction.x*chargeMod,Time.deltaTime*speed*direction.y*chargeMod,0);
			
			//boundaries på skärmen
			if ( transform.position.y > 0.83f )
				transform.position = new Vector3( transform.position.x,0.83f,0);
			if ( transform.position.x > 4.78f )
				transform.position = new Vector3( 4.78f,transform.position.y,0);
			if ( transform.position.y < -2.4f )
				transform.position = new Vector3( transform.position.x,-2.4f,0);
			if ( transform.position.x < -4.78f )
				transform.position = new Vector3( -4.78f,transform.position.y,0);

			//sätter rätt sortingorder så de ritas ut i rätt ordning
			foreach (Animator item in anims) {
				item.GetComponent<SpriteRenderer>().sortingOrder = 1000 - Mathf.FloorToInt((transform.position.y*100));
			}

			for (int i = 0; i < hearts.Count; i++) 
			{
				hearts[i].sortingOrder = 1000 - Mathf.FloorToInt((transform.position.y*100));
			}
			
		}
	}

	IEnumerator RainbowColor()
	{
		while( true )
		{
			SetColor( Random.Range( 0, 255 ), Random.Range( 0, 255 ), Random.Range( 0, 255 ), Random.Range( 0, 255 ), Random.Range( 0, 255 ), Random.Range( 0, 255 ) );
			yield return new WaitForSeconds( 0.5f );
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
		//ta bort den gamla
		combo[0].bRemove = true;
		combo.RemoveAt(0);
		
		//skapa en ny längst till höger
		GameObject GO = (GameObject)Instantiate(comboPref,new Vector3(5f,-2.25f,0),Quaternion.identity);
		GO.GetComponent<ComboBar>().gameRef = this;
		combo.Add( GO.GetComponent<ComboBar>());
		
		//få alla att röra sig åt vänster
		for (int i = 0; i < combo.Count; i++) 
		{
			combo[i].bMove++;
		}
	}

	//denna körs ifrån comboBar om tiden tagit slut
	public void FailedCombo()
	{
		EndCharge();
	}

	//SERVER
	void OnPlayerConnected( NetworkPlayer player )
	{
		//Se till att uppdatera vilken riktning som alla spelare står i
		if( playerObj.localScale.x < 0 ) thisNetworkView.RPC( "IsMirrored", player );
	}

	[RPC] public void IsMirrored()
	{
		playerObj.localScale = new Vector3( -1, 1, 1 );
	}

	//säger åt alla spelare att vi håller på att går, sätter animation och direction
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
		
		if ( dir.x != 0 )
			playerObj.localScale = new Vector3( (dir.x < 0 ? -1 : 1 ),1,1);

		if ( thisNetworkView.isMine )
			thisNetworkView.RPC("Walk",RPCMode.Others,dir.x,dir.y);
	}

	//resettar stocks till 3, dock fungerar bara på 1 spelare
	[RPC] public void Reset()
	{
		stock = 3;
		scoreText.text = stock.ToString();

		if ( thisNetworkView.isMine )
			thisNetworkView.RPC("Reset",RPCMode.Others);
	}

	//om en spelare börjar chargea
	[RPC] public void Charge()
	{
		charge = true;

		foreach (Animator item in anims )
			item.SetBool("Charge",true);

		if ( thisNetworkView.isMine )
			thisNetworkView.RPC("Charge",RPCMode.Others);
	}
	
	//när spelaren slutar chargea
	[RPC] public void EndCharge()
	{
		charge = false;

		foreach (Animator item in anims )
			item.SetBool("Charge",false);

		if ( thisNetworkView.isMine )
			thisNetworkView.RPC("EndCharge",RPCMode.Others);
	}
	
	//när den attackerar så aktiveras boxcollider
	[RPC] public void Atk()
	{
		atkZone.enabled = true;

		foreach (Animator item in anims )
			item.SetTrigger("Atk");

		if ( thisNetworkView.isMine )
			thisNetworkView.RPC("Atk",RPCMode.Others);
	}

	//om man får skada så ska man veta vart skadan kom ifrån
	//ENDAST ANIMATION
	[RPC] public void RecieveDamage( int amount , float dirX, int p0) 
	{
		foreach (Animator item in anims )
			item.SetTrigger("Damage");

		//knockback
		if ( dirX == 1 )
			transform.position += new Vector3(0.5f,0,0);
		else
			transform.position -= new Vector3(0.5f,0,0);
		
		//om man dör
		if ( hp - amount <= 0 )
		{
			if ( thisNetworkView.isMine )
			{
				SyncHp( 3, true );
			}
		}else
		{
			if ( thisNetworkView.isMine )
			{
				SyncHp( hp - amount, false );
			}
		}
		
		if ( thisNetworkView.isMine )
			thisNetworkView.RPC("RecieveDamage",RPCMode.Others,amount,dirX,p0);
	}

	//Syncar hp
	[RPC] public void SyncHp( int hp, bool loseStock )
	{
		this.hp = hp;
		SetHP();

		if( loseStock )
		{
			stock--;
			scoreText.text = stock.ToString();
			transform.position = new Vector3(4.18f,0.67f,0);
			transform.rotation = Quaternion.identity;
		}

		if ( thisNetworkView.isMine )
			thisNetworkView.RPC( "SyncHp", RPCMode.OthersBuffered, hp, loseStock );
	}

	[RPC] public void SetColor( int R , int G ,int B, int R2 , int G2 ,int B2 ) 
	{
		anims[1].GetComponent<SpriteRenderer>().color = new Color(R/255f,G/255f,B/255f);
		anims[2].GetComponent<SpriteRenderer>().color = new Color(R2/255f,G2/255f,B2/255f);

		if ( thisNetworkView.isMine )
			thisNetworkView.RPC("SetColor",RPCMode.OthersBuffered,R,G,B,R2,G2,B2);
	}
}
