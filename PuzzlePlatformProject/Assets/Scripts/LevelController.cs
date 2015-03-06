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

	public string[] tagTab={"JumpLeft","JumpRight", "GoLeft", "GoRight"};

	public int nbJumpR=10;
	public int nbJumpL=10;
	public int nbGoR=10;
	public int nbGoL=10;
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

		bestScore.text="Best= "+scoreBefore;

		nbTry=0;
		tryText.text="Attemps: "+nbTry;

		timer=0;
		timeText.text="Time: 0.00";
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

	public void MaJNbText()
	{
		JumpRText.text="Jump Right x" + nbJumpR;
		JumpLText.text="Jump Left x" + nbJumpL;
		GoRText.text="Turn Right x" + nbGoR;
		GoLText.text="Turn Left x" + nbGoL;
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
		if (endType==0)
		{
			playerController.End();
			nbPan=PanRest();
			total =gameController.Score(nbPan,nbTry,timer);
			if (total>scoreBefore)
			{
				bestScore.text="Best= "+total;
				PlayerPrefs.SetInt(Application.loadedLevelName, total);
				PlayerPrefs.Save();
			}
		}
		else if (endType==1)
			playerController.Dead();

		end=true;

		interfaceController.GameOver(endType, nbPan, nbTry, timer, total);
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
		playerCanMove=false;
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
