using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

	public Text JumpRText;
	public Text JumpLText;
	public Text GoRText;
	public Text GoLText;



	public Text tryText;
	public Text timeText;

	public Text bestScore;

	public Button nextButton;

	public string[] tagTab={"JumpLeft","JumpRight", "GoLeft", "GoRight"};

	public int scoreStars1;
	public int scoreStars2;
	public int scoreStars3;

	public int nbJumpR=10;
	public int nbJumpL=10;
	public int nbGoR=10;
	public int nbGoL=10;

	public GameObject jumpR;
	public GameObject jumpL;
	public GameObject goR;
	public GameObject goL;


	bool end=false;
	int nbPlatform;
	int nbTry;
	float timer;
	float startTime;


	private GameController gameController;
	private InterfaceController interfaceController;
	private CharacterControllerAuto playerController;
	private PlatformController[] platformControllers;

	private int scoreBefore;

	[HideInInspector] public bool playerCanMove=false;
	[HideInInspector] public bool panelCanMove=true;



	void Start ()
	{
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

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent <GameController>();
		else
			Debug.Log ("Cannot find 'GameController' script");

		if (PlayerPrefs.HasKey(Application.loadedLevelName))
		    scoreBefore=PlayerPrefs.GetInt(Application.loadedLevelName);
		else
		    scoreBefore=0;

		bestScore.text=""+scoreBefore;

		nbTry=0;
		tryText.text="Attemps: "+nbTry;

		timer=0;
		timeText.text="Time: 0.00";

		if (nbJumpR==0)
			jumpR.SetActive(false);
		if (nbJumpL==0)
			jumpL.SetActive(false);
		if (nbGoR==0)
			goR.SetActive(false);
		if (nbGoL==0)
			goL.SetActive(false);


		MaJNbText ();
	}

	void Update()
	{
		if (playerCanMove && !end)
		{
			float timerTemp=Time.time-startTime;
			timer=Mathf.Floor(timerTemp*100)/100;
			timeText.text="Time: "+timer;
		}
	}

	void MaJNbText()
	{
		string jrTxt="";
		string jlTxt="";
		string grTxt="";
		string glTxt="";

		if (nbJumpR>0)
			jrTxt="Jump Right x" + nbJumpR;
		if (nbJumpL>0)
			jlTxt="Jump Left x" + nbJumpL;
		if (nbGoR>0)
			grTxt="Turn Right x" + nbGoR;
		if (nbGoL>0)
			glTxt="Turn Left x" + nbGoL;

		JumpRText.text=jrTxt;
		JumpLText.text=jlTxt;
		GoRText.text=grTxt;
		GoLText.text=glTxt;
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

		return true;

	}

	public int PanRest()
	{
		int nbPan=nbJumpR+nbJumpL+nbGoR+nbGoL;
		return nbPan;
	}


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
			total = gameController.Score(nbPan,nbTry,timer);

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
		else if (endType==1)
			playerController.Dead();

		end=true;
		interfaceController.GameOver(endType, nbPan, nbTry, timer, total, nbStars, passed);
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
		tryText.text="Attemps: "+nbTry;
		startTime=Time.time;
	}

	void ResetPositions()
	{
		StartCoroutine(waitAMoment());
		panelCanMove=true;
		playerController.ReturnToPosition();

		timer=0;
		timeText.text="Time: 0.00";

		ResetInstruct();

		if (nbPlatform>0)
		{
			foreach (PlatformController platForm in platformControllers)
				platForm.Desactive();
		}
	}

	//Permet de laisser le temps que totu se remette en place
	IEnumerator waitAMoment()
	{
		yield return new WaitForSeconds(0.2f);
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
				int nbJump = InstructObject.Length;
				PlaceController[] PlaceControllers=new PlaceController[nbJump];
				for (int i=0; i<nbPlatform;i++)
					PlaceControllers.SetValue(InstructObject[i].GetComponent<PlaceController>(),i);
				foreach (PlaceController PlaceC in PlaceControllers)
				{
					if (PlaceC != null)
						PlaceC.Replace();
				}
			}
		}
	}
}
