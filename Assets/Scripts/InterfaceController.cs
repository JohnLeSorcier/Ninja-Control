using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;


public class InterfaceController : MonoBehaviour {
	
	public Text gameOverText;
	public GameObject gameOverPanel;
	public Button[] GoPauseButton;

	public Button nextLevel;

	public Image[] stars;
	public Image[] nStars;

	private GameController gameController;

	public Button soundMusicOn;
	public Button soundMusicOff;
	
	public Button soundFXOn;
	public Button soundFXOff;

	LanguageManager languageManager;
	
	string baseCoin;
	string baseAttempt;
	string baseLeftTime;
	string baseUnusedPanel;
	string baseTotalScore;
	
	AudioSource audioS;

	void Start () 
	{
	
		string language;
		if(PlayerPrefs.HasKey("Language"))
			language=PlayerPrefs.GetString("Language");
		else
			language = "en";
		
		languageManager= LanguageManager.Instance;
		languageManager.ChangeLanguage(language);	
		
		baseCoin=languageManager.GetTextValue("Interface.Coin");
		baseAttempt=languageManager.GetTextValue("Interface.Attempt");
		baseLeftTime=languageManager.GetTextValue("Interface.LeftTime");
		baseUnusedPanel=languageManager.GetTextValue("Interface.UnusedPanel");
		baseTotalScore=languageManager.GetTextValue("Interface.TotalScore");
	
		
		gameOverText.text="";
		gameOverPanel.SetActive(false);

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent <GameController>();
		else
			Debug.Log ("Cannot find 'GameController' script");
		

		AudioSource audioMusic=gameController.GetComponent<AudioSource>();
	
		string musicState;
		if (PlayerPrefs.HasKey("Music"))
			musicState=PlayerPrefs.GetString("Music");
		else
			musicState="On";
					
		if (musicState == "Off")
			audioMusic.mute=true;
		else
			audioMusic.mute=false;
			
		soundMusicOff.gameObject.SetActive(audioMusic.mute);
		soundMusicOn.gameObject.SetActive(!audioMusic.mute);
		
		
		
		audioS=GetComponent<AudioSource>();
		
		string FXState;
		
		if(PlayerPrefs.HasKey("FXState"))
			FXState=PlayerPrefs.GetString("FXState");
		else
			FXState= "On";
		
		bool FXStateBool;
			
		if (FXState == "On")
			FXStateBool = true;
		else
			FXStateBool = false;
			
		audioS.mute=!FXStateBool;
		
		soundFXOn.gameObject.SetActive(FXStateBool);
		soundFXOff.gameObject.SetActive(!FXStateBool);
	}


	public void ReloadLevel()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}

	//affiche un message différent suivant le type de fin: 0 pour réussite, 1 pour une mort, 2 pour une sortie de niveau, 3 pour manque de temps, 4 pour noyade, 5 pour assomé, 6 pour écrasé (comme mort normale)
	public void GameOver(int endType, int nbPan, int nbTry, int nbPieces, float timer, int score, int nbStars, bool passed)
	{
		nextLevel.interactable=false;
		string textAffich;

		//désactivez les étoiles 'noirs ou jaunes) si pas fini
		foreach (Image nstar in nStars)
			nstar.enabled=false;
		foreach (Image star in stars)
			star.enabled=false;



		if (endType == 0)
		{
			textAffich=baseUnusedPanel+": "+nbPan+"\n"+baseAttempt+": "+nbTry+"\n"+baseCoin+": "+nbPieces+"\n"+baseLeftTime+": "+timer+"s\n"+baseTotalScore+": "+score;

			if(passed && !(Application.loadedLevel == Application.levelCount-1))
				nextLevel.interactable=true;

			if (passed)
				textAffich+="\n\n"+languageManager.GetTextValue("Interface.Win");
			else
				textAffich+="\n\n"+languageManager.GetTextValue("Interface.ScoreLow");

			foreach (Image nstar in nStars)
				nstar.enabled=true;

			if (nbStars>0)
				stars[0].enabled=true;
			if (nbStars>1)
				stars[1].enabled=true;
			if (nbStars>2)
				stars[2].enabled=true;

		}
		else if (endType == 1 || endType == 6) 
			textAffich=languageManager.GetTextValue("Interface.Dead");
		else if (endType == 4)
			textAffich=languageManager.GetTextValue("Interface.Drowned");
		else if (endType == 5)
			textAffich=languageManager.GetTextValue("Interface.Knocked");
		else if (endType == 2)
			textAffich=languageManager.GetTextValue("Interface.OutofLevel");
		else if (endType == 3)
			textAffich=languageManager.GetTextValue("Interface.TimeOver");
		else
			textAffich=languageManager.GetTextValue("Interface.BadEnd");

		gameOverText.text=textAffich;

		DesactivButton();

		StartCoroutine(DisplayEnd());
	}

	public void ReturnMenu()
	{
		gameController.retourMenu ();
	}

	public void NextLevel()
	{
		Application.LoadLevel(Application.loadedLevel+1);
	}

	public void DesactivButton()
	{
		foreach(Button button in GoPauseButton)
		{
			button.interactable=false;
		}
	}

	public void ActivButton()
	{
		foreach(Button button in GoPauseButton)
		{
			button.interactable=true;
		}
		GoPauseButton[0].gameObject.SetActive(true);
		GoPauseButton[1].gameObject.SetActive(false);
	}


	IEnumerator DisplayEnd()
	{
		yield return new WaitForSeconds(1f);
		gameOverPanel.SetActive(true);
	}

	public void waitForGoButton()
	{
		DesactivButton();
		StartCoroutine(timerButton());

	}

	IEnumerator timerButton()
	{
		yield return new WaitForSeconds(0.1f);
		ActivButton();
	}



}
