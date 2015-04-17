﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

	public Text JumpRText;
	public Text JumpLText;
	public Text GoRText;
	public Text GoLText;
	public Text SGText;
	public Text DWText;



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
	int nbPlatform;
	int nbHazard;

	private int scoreBefore;

	[HideInInspector] public bool playerCanMove=false;
	[HideInInspector] public bool panelCanMove=true;
	

	void Start ()
	{
		nbPlatform=0;
		nbHazard=0;

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

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent <GameController>();
		else
			Debug.Log ("Cannot find 'GameController' script");

		if (PlayerPrefs.HasKey(Application.loadedLevel+"_score"))
			scoreBefore=PlayerPrefs.GetInt(Application.loadedLevel+"_score");
		else
		    scoreBefore=0;

		bestScore.text=""+scoreBefore;

		nbTry=0;
		tryText.text="Attempts: "+nbTry;

		timer=timeAllowed;//eviter une bétise de vérif.
		timeText.text="Time: "+timeAllowed+"\'\'00";//redondant avec l'update, mais par sécurité...

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
		pieceText.text="Coins: "+nbPieces;

		MaJNbText ();
	}

	void Update()
	{
		if (!playerCanMove)
			timeText.text="Time: "+timeAllowed+"\'\'00";

		int sec;
		int milli;
		if (playerCanMove && !end)
		{
			float timerTemp=Time.time-startTime;
			timer=timeAllowed-Mathf.Floor(timerTemp*100)/100;
			sec=Mathf.FloorToInt(timer);
			milli=Mathf.FloorToInt((timer-sec)*100);
			timeText.text="Time: "+sec+"\'\'"+milli;
		}

		if(timer<0 && !end)
		{
			end=true;
			timeText.text="Time: 0\'\'00" ;
			GameOver(3);
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

		if (nbJumpR>0)
			jrTxt="Jump Right x" + nbJumpR;
		if (nbJumpL>0)
			jlTxt="Jump Left x" + nbJumpL;
		if (nbGoR>0)
			grTxt="Turn Right x" + nbGoR;
		if (nbGoL>0)
			glTxt="Turn Left x" + nbGoL;
		if (nbSG>0)
			sgTxt="Wait x" + nbSG;
		if (nbDW>0)
			dwTxt="Slide x" + nbDW;

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

	//0 pour réussite, 1 pour une mort, 2 pour une sortie de niveau, 3 pour manque de temps, 4 pour noyade
	public void GameOver(int endType)
	{
		int nbPan=0;
		int total=0;
		int nbStars=0;
		bool passed=false;

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
				PlayerPrefs.SetInt(Application.loadedLevel+"_score", total);
				PlayerPrefs.SetInt(Application.loadedLevel+"_stars", nbStars);
				PlayerPrefs.Save();
			}
			nextButton.interactable=true;
		}
		else if (endType==1 || endType==2)
			playerController.Dead(0);
		else if (endType==4)
			playerController.Dead(1);
		else if (endType==3)
			playerController.End();

		end=true;
		interfaceController.GameOver(endType, nbPan, nbTry, nbPieces, timer, total, nbStars, passed);
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
		nbTry+=1;
		tryText.text="Attempts: "+nbTry;
		startTime=Time.time;
	}

	public void ResetPositions()
	{
		timer=timeAllowed;//éviter une bétise de vérif
		playerController.ReturnToPosition();
		end=false;
		ResetInstruct();
		nbPieces=0;
		pieceText.text="Coins: "+nbPieces;

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
		pieceText.text="Coins: "+nbPieces;
	}

	public void SoundGest()
	{
		AudioSource audioS=gameController.GetComponent<AudioSource>();
		audioS.mute=!audioS.mute;
	}
}
