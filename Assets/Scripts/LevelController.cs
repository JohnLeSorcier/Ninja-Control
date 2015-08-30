using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;

public class LevelController : MonoBehaviour {

	public Text JumpRText;
	public Text JumpLText;
	public Text GoRText;
	public Text GoLText;
	public Text SGText;
	public Text DWText;
	
	public Text JumpRDesc;
	public Text JumpLDesc;
	public Text GoRDesc;
	public Text GoLDesc;
	public Text SGDesc;
	public Text DWDesc;

	public Text levelName;
	public string levelNameIndex;

	public Text tryText;
	public Text timeText;
	public Text pieceText;

	public Text bestScore;

	public Button nextButton;

	private string[] tagTab={"JumpLeft","JumpRight", "GoLeft", "GoRight", "StopGo","Slide"};

	public int timeAllowed;

	public int scoreStars1;
	public int scoreStars2;
	public int scoreStars3;

	public int nbJumpR=10;
	public int nbJumpL=10;
	public int nbGoR=10;
	public int nbGoL=10;
	public int nbSG=10;
	public int nbDW=10;

	public GameObject jumpR;
	public GameObject jumpL;
	public GameObject goR;
	public GameObject goL;
	public GameObject SG;
	public GameObject DW;


	bool end=false;
	int nbTry;
	float timer;
	float startTime;
	int nbPieces;


	private GameController gameController;
	private InterfaceController interfaceController;
	private CharacterControllerAuto playerController;

	private PlatformController[] platformControllers;
	private HazardController[] hazardControllers;
	private AutoDoorController[] AutoDoorControllers;
	private DoorController[] DoorControllers;
	int nbPlatform;
	int nbHazard;
	int nbAutoDoor;
	int nbDoor;

	private int scoreBefore;

	[HideInInspector] public bool playerCanMove=false;
	[HideInInspector] public bool panelCanMove=true;

	bool one_click = false;
	bool timer_running;
	float timer_for_double_click;
	
	float delay=0.20f;
	
	LanguageManager languageManager;
	
	string baseTime;
	string baseCoin;
	string baseAttempt;
	
	AudioSource gameOverSound;
	
	[HideInInspector] public bool StateFX;
	float volMax=0.5f;
	
	bool alreadyDead;
	
	void Start ()
	{
	
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent <GameController>();
		else
			Debug.Log ("Cannot find 'GameController' script");
			
		gameController.affichAd();	
	
		string language;
		if(PlayerPrefs.HasKey("Language"))
			language=PlayerPrefs.GetString("Language");
		else
			language = "en";
		
		languageManager= LanguageManager.Instance;
		languageManager.ChangeLanguage(language);	
		
		string levelTxt=languageManager.GetTextValue("Menu.Lvl");
				
		levelName.text=levelTxt+" "+Application.loadedLevel+":\n"+languageManager.GetTextValue("Level."+levelNameIndex);
	
		baseTime=languageManager.GetTextValue("Interface.Time");
		baseCoin=languageManager.GetTextValue("Interface.Coin");
		baseAttempt=languageManager.GetTextValue("Interface.Attempt");
	
		nbPlatform=0;
		nbHazard=0;
		nbAutoDoor=0;

		Time.timeScale=1.0f;

		interfaceController = GetComponent<InterfaceController>();

		GameObject playerObject = GameObject.FindWithTag ("Player");
		if (playerObject != null)
			playerController = playerObject.GetComponent <CharacterControllerAuto>();
		else
			Debug.Log ("Cannot find 'Player' script");

		GameObject[] platformObject = GameObject.FindGameObjectsWithTag("Platform");
		if (platformObject !=null)
		{
			nbPlatform = platformObject.Length;
			platformControllers=new PlatformController[nbPlatform];
			for (int i=0; i<nbPlatform;i++)
				platformControllers.SetValue(platformObject[i].GetComponent<PlatformController>(),i);
		}

		GameObject[] hazardObject = GameObject.FindGameObjectsWithTag("Hazard");
		if (hazardObject !=null)
		{
			nbHazard = hazardObject.Length;
			hazardControllers=new HazardController[nbHazard];
			for (int i=0; i<nbHazard;i++)
				hazardControllers.SetValue(hazardObject[i].GetComponent<HazardController>(),i);
		}

		GameObject[] AutoDoorObject = GameObject.FindGameObjectsWithTag("AutoDoor");
		if (AutoDoorObject !=null)
		{
			nbAutoDoor = AutoDoorObject.Length;
			AutoDoorControllers=new AutoDoorController[nbAutoDoor];
			for (int i=0; i<nbAutoDoor;i++)
				AutoDoorControllers.SetValue(AutoDoorObject[i].GetComponent<AutoDoorController>(),i);
		}

		GameObject[] DoorObject = GameObject.FindGameObjectsWithTag("Door");
		if (DoorObject !=null)
		{
			nbDoor = DoorObject.Length;
			DoorControllers=new DoorController[nbDoor];
			for (int i=0; i<nbDoor;i++)
				DoorControllers.SetValue(DoorObject[i].GetComponent<DoorController>(),i);
		}


		if (PlayerPrefs.HasKey(Application.loadedLevel+"_score"))
			scoreBefore=PlayerPrefs.GetInt(Application.loadedLevel+"_score");
		else
		    scoreBefore=0;

		bestScore.text=""+scoreBefore;

		nbTry=0;
		tryText.text=baseAttempt+": "+nbTry;

		timer=timeAllowed;//eviter une bétise de vérif.
		timeText.text=baseTime+": "+timeAllowed+"\'\'00";//redondant avec l'update, mais par sécurité...

		if (nbJumpR==0)
			jumpR.SetActive(false);
		if (nbJumpL==0)
			jumpL.SetActive(false);
		if (nbGoR==0)
			goR.SetActive(false);
		if (nbGoL==0)
			goL.SetActive(false);
		if (nbSG==0)
			SG.SetActive(false);
		if (nbDW==0)
			DW.SetActive(false);

		nbPieces=0;
		pieceText.text=baseCoin+": "+nbPieces;

		MaJNbText ();
		
		gameOverSound=GetComponent<AudioSource>();
		
		string FXState;
		
		if(PlayerPrefs.HasKey("FXState"))
			FXState=PlayerPrefs.GetString("FXState");
		else
			FXState= "On";
		
	
		if (FXState == "On")
			StateFX = true;
		else
			StateFX = false;
			
		float volume;
		if (PlayerPrefs.HasKey("Volume"))
			volume=PlayerPrefs.GetFloat("Volume");
		else
			volume=100f;
		gameOverSound.volume=volMax*volume/100;
		
		alreadyDead=false;
			
	}

	void Update()
	{
		if (!playerCanMove)
			timeText.text=baseTime+": "+timeAllowed+"\'\'00";

		int sec;
		int milli;
		if (playerCanMove && !end)
		{
			float timerTemp=Time.time-startTime;
			timer=timeAllowed-Mathf.Floor(timerTemp*100)/100;
			sec=Mathf.FloorToInt(timer);
			milli=Mathf.FloorToInt((timer-sec)*100);
			timeText.text=baseTime+": "+sec+"\'\'"+milli;
		}

		if(timer<0 && !end)
		{
			timeText.text=baseTime+": 0\'\'00" ;
			GameOver(3);
		}

		if(Input.GetMouseButtonDown(0))
		{
			if(!one_click) 
			{
				one_click = true;
				timer_for_double_click = Time.time; 
			} 
			else
			{
				one_click = false; 				
				if (playerCanMove)
					ChangeMove();
			}
		}
		if(one_click)
		{
			if((Time. time - timer_for_double_click) > delay)
				one_click = false;				
		}

	}

	void MaJNbText()
	{
		string jrTxt="";
		string jlTxt="";
		string grTxt="";
		string glTxt="";
		string sgTxt="";
		string dwTxt="";
		JumpRDesc.enabled=false;
		JumpLDesc.enabled=false;
		GoRDesc.enabled=false;
		GoLDesc.enabled=false;
		SGDesc.enabled=false;
		DWDesc.enabled=false;

		if (nbJumpR>0)
		{
			jrTxt="x" + nbJumpR;
			JumpRDesc.enabled=true;
		}
		if (nbJumpL>0)
		{
			jlTxt="x" + nbJumpL;
			JumpLDesc.enabled=true;
		}
		if (nbGoR>0)
		{
			grTxt="x" + nbGoR;
			GoRDesc.enabled=true;
		}
		if (nbGoL>0)
		{
			glTxt="x" + nbGoL;
			GoLDesc.enabled=true;
		}
		if (nbSG>0)
		{
			sgTxt="x" + nbSG;
			SGDesc.enabled=true;
		}
		if (nbDW>0)
		{
			dwTxt="x" + nbDW;
			DWDesc.enabled=true;
		}

		JumpRText.text=jrTxt;
		JumpLText.text=jlTxt;
		GoRText.text=grTxt;
		GoLText.text=glTxt;
		SGText.text=sgTxt;
		DWText.text=dwTxt;
	}

	//on passe les tag des objets
	public void TakePanel(string tagPanel)
	{
		if (tagPanel == "JumpRight")
			nbJumpR -=1;
		if (tagPanel == "JumpLeft")
			nbJumpL -=1;
		if (tagPanel == "GoRight")
			nbGoR -=1;
		if (tagPanel == "GoLeft")
			nbGoL-=1;
		if (tagPanel == "StopGo")
			nbSG-=1;
		if (tagPanel == "Slide")
			nbDW-=1;

		MaJNbText();
	}
	
	public void AddPanel(string tagPanel)
	{
		if (tagPanel == "JumpRight")
			nbJumpR +=1;
		if (tagPanel == "JumpLeft")
			nbJumpL +=1;
		if (tagPanel == "GoRight")
			nbGoR +=1;
		if (tagPanel == "GoLeft")
			nbGoL+=1;
		if (tagPanel == "StopGo")
			nbSG+=1;
		if (tagPanel == "Slide")
			nbDW+=1;
		
		MaJNbText();
	}

	public bool VerifPanel (string tagPanel)
	{
		if (tagPanel == "JumpRight" && nbJumpR==0)
			return false;
		if (tagPanel == "JumpLeft"&& nbJumpL==0)
			return false;
		if (tagPanel == "GoRight"&& nbGoR==0)
			return false;
		if (tagPanel == "GoLeft"&& nbGoL==0)
			return false;
		if (tagPanel == "StopGo"&& nbSG==0)
			return false;
		if (tagPanel == "Slide"&& nbDW==0)
			return false;

		return true;

	}

	public int PanRest()
	{
		int nbPan=nbJumpR+nbJumpL+nbGoR+nbGoL+nbSG+nbDW;
		return nbPan;
	}

	//0 pour réussite, 1 pour une mort, 2 pour une sortie de niveau, 3 pour manque de temps, 4 pour noyade, 5 pour assomé, 6 pour écrasé
	public void GameOver(int endType)
	{
		int nbPan=0;
		int total=0;
		int nbStars=0;
		bool passed=false;
		if (!end)
		{
			if (endType==0)
			{
				passed=true;
				playerController.End();
				nbPan=PanRest();
				total = gameController.Score(nbPan,nbTry,nbPieces,timer);
	
				if (total>scoreStars3)
					nbStars=3;
				else if (total>scoreStars2)
					nbStars=2;
				else if (total>scoreStars1)
					nbStars=1;
				else
					passed=false;
	
				if (total>scoreBefore)
				{
					bestScore.text=""+total;
					PlayerPrefs.SetFloat(Application.loadedLevel+"_time", timeAllowed-timer);
					PlayerPrefs.SetInt(Application.loadedLevel+"_score", total);
					PlayerPrefs.SetInt(Application.loadedLevel+"_stars", nbStars);
					PlayerPrefs.Save();
				}
				nextButton.interactable=true;
			}
			else if (endType==1 || endType==2 || endType==5)
				playerController.Dead(0);
			else if (endType==4)
				playerController.Dead(1);
			else if (endType==6)
				playerController.Dead(2);
			else if (endType==3)
				playerController.End();
				
			if (!alreadyDead)
				interfaceController.GameOver(endType, nbPan, nbTry, nbPieces, timer, total, nbStars, passed);
				
			if (endType!=0 && !alreadyDead)
			{
				sonGameOver();
				alreadyDead=true;
			}
	
			end=true;
		}
	}

	public void ChangeMove()
	{
		if (!playerCanMove)
		{
			GoPlayer ();
		}
		else if (!end)
		{
			ResetPositions ();
			interfaceController.waitForGoButton();
		}
	}

	void GoPlayer()
	{
		playerController.PlayerMove();
		playerCanMove=true;
		panelCanMove=false;
		if (nbPlatform>0)
		{
			foreach (PlatformController platForm in platformControllers)
				platForm.Active();
		}

		if (nbAutoDoor>0)
		{
			foreach (AutoDoorController autoDoor in AutoDoorControllers)
				autoDoor.Active();
		}

		nbTry+=1;
		tryText.text=baseAttempt+": "+nbTry;
		startTime=Time.time;
	}

	public void ResetPositions()
	{
		timer=timeAllowed;//éviter une bétise de vérif
		playerController.ReturnToPosition();
		end=false;
		alreadyDead=false;
		ResetInstruct();
		nbPieces=0;
		pieceText.text=baseCoin+": "+nbPieces;

		if (nbPlatform>0)
		{
			foreach (PlatformController platForm in platformControllers)
				platForm.Desactive();
		}

		if (nbHazard>0)
		{
			foreach (HazardController hazard in hazardControllers)
				hazard.resetPosition();
		}

		if (nbAutoDoor>0)
		{
			foreach (AutoDoorController autoDoor in AutoDoorControllers)
				autoDoor.Desactive();
		}

		if (nbDoor>0)
		{
			foreach (DoorController Door in DoorControllers)
				Door.Desactive();
		}

		panelCanMove=true;
		playerCanMove=false;

	}
	

	void ResetInstruct()
	{
		GameObject[] InstructObject;
		foreach (string tag in tagTab)
		{
			InstructObject = GameObject.FindGameObjectsWithTag(tag);
			if (InstructObject !=null)
			{
				int nbInstruct = InstructObject.Length;
				PlaceController[] PlaceControllers=new PlaceController[nbInstruct];
				for (int i=0; i<nbInstruct;i++)
					PlaceControllers.SetValue(InstructObject[i].GetComponent<PlaceController>(),i);
				foreach (PlaceController PlaceC in PlaceControllers)
				{
					if (PlaceC != null)
						PlaceC.Replace();
				}
			}
		}
	}

	public void Pause()
	{
		Time.timeScale=0.0f;
	}

	public void Relauch()
	{
		Time.timeScale=1.0f;
	}

	public void RamassePiece()
	{
		nbPieces++;
		pieceText.text=baseCoin+": "+nbPieces;
	}

	public void SoundGest()
	{
		gameController.GestMusic();
	}
	
	public void FXGestOn()
	{
		StateFX=true;
		PlayerPrefs.SetString("FXState", "On");
		PlayerPrefs.Save ();		
	}
	
	public void FXGestOff()
	{
		StateFX=false;
		PlayerPrefs.SetString("FXState", "Off");
		PlayerPrefs.Save ();
	}
		
		
	public void sonGameOver()
	{
		gameOverSound.mute=!StateFX;
		if (!gameOverSound.mute)
			gameController.suspendMusic();
			
		StartCoroutine(waitforGameOverSound());
	}
	
	IEnumerator waitforGameOverSound()
	{
		yield return new WaitForSeconds(0.5f);
		gameOverSound.Play ();
	}
}
