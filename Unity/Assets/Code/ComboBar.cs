using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ComboBar : MonoBehaviour {

	public Sprite glow;					//sparar bilden för när den ska lysa
	public List<Sprite> button_off;		//sparar bilder
	public List<Sprite> button_on;		//sparar bilder
	public List<SpriteRenderer> buttons;
	private List<int> commands = new List<int>();
	private int completed;				//hur många knappar man har klarat
	private float timer;
	public SpriteRenderer timeBar;		
	public PlayerController gameRef;		//för att skicka tillbaka ifall den är failad eller klarad
	public bool ready = false;			//ifall den har nått sin slut position	
	public bool bRemove;				//true when removing
	public int bMove;					//true when moving, det är en int för att räkna hur många steg den ska åka
	private float moveSpeedMod = 3;		//hur snabbt den rör sig 
	private float comboSpeedDuration = 2.0f;	//hur många sekunder man har på sig på varje 


	void Start () 
	{
		//skapar X antal buttons
		int length = Mathf.FloorToInt(Random.Range(2,4));
		for (int i = 0; i < length; i++) 
		{
			commands.Add(Mathf.FloorToInt(Random.Range(0,4)));
			buttons[i].sprite = button_off[commands[i]];
		}
	}
	
	//vi testar ifall det är rätt knapp vi tryckte på
	public bool ButtonPressed(int i0)
	{
		if ( i0 == commands[completed] )
		{
			buttons[completed].sprite = button_on[commands[completed]];
			completed++;
			return true;
		}
		
		return false;
	}
	
	//kollar om hela kombon är klar 
	public bool CheckIfFinished()
	{
		if ( completed >= commands.Count )
			return true;
		else
			return false;
	}
	
	public void SetGlow()
	{
		GetComponent<SpriteRenderer>().sprite = glow;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( bMove > 0 )
			MoveNext();
			
		if ( bRemove )
			FadeOut();
			
		//flyttar timeBar så vi vet hur lång tid det är kvar
		if ( completed > 0 && bRemove == false )
		{
			timer+=Time.deltaTime;
			timeBar.transform.localScale += new Vector3(Time.deltaTime/comboSpeedDuration/5,0,0);
			timeBar.transform.position += new Vector3(Time.deltaTime/comboSpeedDuration/1.0f,0,0);
			
			if ( timer >= comboSpeedDuration )
			{
				timer = 0;
				gameRef.FailedCombo();
				gameRef.GetNextComboReady();
			}
		}
	}
	
	//fadear ut rutan
	private void FadeOut()
	{
		timer+= Time.deltaTime*3;
		GetComponent<SpriteRenderer>().color = new Color(1,1,1,1-timer);
		for (int i = 0; i < buttons.Count; i++) 
		{
			buttons[i].color = new Color(1,1,1,1-timer); 	
		}
		timeBar.color = new Color(0,0,0,1-timer); 
		if ( timer >= 1 )
			Destroy(this.gameObject);
	}
	
	//flyttar rutan till nästa posotion
	private void MoveNext()
	{
		timer += Time.deltaTime*moveSpeedMod;
		transform.position -= new Vector3(Time.deltaTime*moveSpeedMod,0,0);
		if ( timer >= 2.5f )
		{
			timer = 0;
			bMove--;
			if ( transform.position.x < -2 )
			{
				SetGlow();
				ready = true;
			}
		}	
	}
}
