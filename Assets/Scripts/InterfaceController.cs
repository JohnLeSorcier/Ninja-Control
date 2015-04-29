using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour {
	
	public Text gameOverText;
	public GameObject gameOverPanel;
	public Button[] GoPauseButton;

	public Button nextLevel;

	public Image[] stars;
	public Image[] nStars;

	private GameController gameController;

	public Button soundOn;
	public Button soundOff;



	void Start () 
	{
		gameOverText.text="";
		gameOverPanel.SetActive(false);

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent <GameController>();
		else
			Debug.Log ("Cannot find 'GameController' script");

		AudioSource audioS=gameController.GetComponent<AudioSource>();

		//activer ou desactiver les boutons de son
		soundOff.gameObject.SetActive(audioS.mute);
		soundOn.gameObject.SetActive(!audioS.mute);


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
			textAffich="Unused Panels: "+nbPan+"\nAttempts: "+nbTry+"\nCoins: "+nbPieces+"\nTime left: "+timer+"s\nTotal Score: "+score;

			if(passed && !(Application.loadedLevel == Application.levelCount-1))
				nextLevel.interactable=true;

			if (passed)
				textAffich+="\n\nYou win!";
			else
				textAffich+="\n\nScore too low...";

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
			textAffich="You are dead...";
		else if (endType == 4)
			textAffich="You are drowned...";
		else if (endType == 5)
			textAffich="You are knocked...";
		else if (endType == 2)
			textAffich="You're out of the level";
		else if (endType == 3)
			textAffich="Time is over!";
		else
			textAffich="This end is not the good one...";

		gameOverText.text=textAffich;

		DesactivButton();

		StartCoroutine(DisplayEnd());
	}

	public void ReturnMenu()
	{
		Application.LoadLevel("MenuPrincipal");
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
