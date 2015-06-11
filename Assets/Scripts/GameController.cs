using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	int finishScore=500;
	int scorePanRest=250;
	int scoreTry=100;
	int scoreTime=200;
	int scorePiece=100;

	public int lastLevel;

	public static GameController control;
	
	AudioSource music;
	bool downMusic;
	bool upMusic;
	public float volMax=0.1f;
	bool initTime=false;
	float startTime;
	float deltaTime;
	

	void Awake()
	{
		if (control == null)
		{
			DontDestroyOnLoad(gameObject);
			control=this;
			lastLevel=0;
		}
		else if (control != this)
		{
			Destroy(gameObject);
		}
	}
	
	void Start()
	{
		music=GetComponent<AudioSource>(); 
		music.volume=volMax;
		downMusic=false;
		upMusic=false;
	}
	
	void Update()
	{	
		if (downMusic)
		{
				if (!initTime)
			{
				startTime=Time.time;
				initTime=true;				
			}
			deltaTime=Time.time-startTime;
			music.volume=Mathf.Max(volMax-(deltaTime/5),0);
			if (music.volume == 0)
			{
				downMusic=false;
				StartCoroutine(waitforMusic());
				initTime=false;
			}			
		}
		if (upMusic)
		{
			if (!initTime)
			{
				startTime=Time.time;
				initTime=true;				
			}
			deltaTime=Time.time-startTime;
			music.volume=Mathf.Min((deltaTime/5),volMax);
			if (music.volume == volMax)
			{
				upMusic=false;
				initTime=false;
			}			
		}
	}

	public int Score(int nbPanel, int nbTry, int nbPieces, float timer)
	{
		float score=finishScore + timer*scoreTime + nbPanel*scorePanRest + nbPieces*scorePiece - nbTry *scoreTry;
		return Mathf.FloorToInt(score);
	}

	public void retourMenu()
	{
		lastLevel = Application.loadedLevel;
		Application.LoadLevel("MenuPrincipal");
	}
	
	public void suspendMusic()
	{	
		downMusic=true;
	}
	
	IEnumerator waitforMusic()
	{
		yield return new WaitForSeconds(4f);
		upMusic=true;
	}
	
	public void GestMusic()
	{
		AudioSource audioS=GetComponent<AudioSource>();
		
		audioS.mute=!audioS.mute;
		if (audioS.mute)
			PlayerPrefs.SetString("Music", "Off");
		else
			PlayerPrefs.SetString("Music", "On");
		PlayerPrefs.Save();
	}

}
