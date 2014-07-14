using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ComboBar : MonoBehaviour {

	public Sprite glow;
	public List<Sprite> button_off;
	public List<Sprite> button_on;
	public List<SpriteRenderer> buttons;
	private List<int> commands = new List<int>();
	private int completed;
	private float timer;
	public SpriteRenderer bar;
	public LazerController gameRef;
	public bool ready = false;

	private float comboSpeedDuration = 2.0f;

	// Use this for initialization
	void Start () 
	{
		int length = Mathf.FloorToInt(Random.Range(2,4));
		for (int i = 0; i < length; i++) 
		{
			commands.Add(Mathf.FloorToInt(Random.Range(0,4)));
			buttons[i].sprite = button_off[commands[i]];
		}
	}
	
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
			
		if ( completed > 0 && bRemove == false )
		{
			timer+=Time.deltaTime;
			bar.transform.localScale += new Vector3(Time.deltaTime/comboSpeedDuration,0,0);
			bar.transform.position += new Vector3(Time.deltaTime/comboSpeedDuration,0,0);
			
			if ( timer >= comboSpeedDuration )
			{
				timer = 0;
				gameRef.FailedCombo();
				gameRef.GetNextComboReady();
			}
		}
	}
	
	public bool bRemove;
	private void FadeOut()
	{
		timer+= Time.deltaTime*3;
		GetComponent<SpriteRenderer>().color = new Color(1,1,1,1-timer);
		for (int i = 0; i < buttons.Count; i++) 
		{
			buttons[i].color = new Color(1,1,1,1-timer); 	
		}
		bar.color = new Color(0,0,0,1-timer); 
		if ( timer >= 1 )
			Destroy(this.gameObject);
	}
	
	public int bMove;
	private float moveMod = 3;
	private void MoveNext()
	{
		timer += Time.deltaTime*moveMod;
		transform.position -= new Vector3(Time.deltaTime*moveMod,0,0);
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
