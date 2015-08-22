using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class GameController : MonoBehaviour {

	int finishScore=500;
	int scorePanRest=150;
	int scoreTry=100;
	int scoreTime=100;
	int scorePiece=150;

	public int lastLevel;

	public static GameController control;
	
	AudioSource music;
	bool downMusic;
	bool upMusic;
	public float volMax=1f;
	bool initTime=false;
	float startTime;
	float deltaTime;
	[HideInInspector] public bool adReady=false;
	

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
		StartCoroutine(waitForLaunchMusic());
		
		
		#if UNITY_ANDROID
			Advertisement.Initialize("61148",true);
		#elif UNITY_IOS
			Advertisement.Initialize("61149");
		#endif
		
		StartCoroutine(waitForAd());
		
		
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
	
	IEnumerator waitForAd()
	{
		yield return new WaitForSeconds(300f);
		adReady=true;
	}
	
	IEnumerator waitForLaunchMusic()
	{
		yield return new WaitForSeconds(1f);
		music.Play();
	}
	
	public void affichAd()
	{
		if(adReady && Advertisement.IsReady())
		{
			Advertisement.Show();
			adReady=false;
			StartCoroutine(waitForAd());
		}
			
	}

}
