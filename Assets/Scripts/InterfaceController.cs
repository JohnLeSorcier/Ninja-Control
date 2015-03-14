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



	void Start () 
	{
		gameOverText.text="";
		gameOverPanel.SetActive(false);
	}


	public void ReloadLevel()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}

	//affiche un message différent suivant le type de fin: 0 pour réussite, 1 pour une mort
	public void GameOver(int endType, int nbPan, int nbTry, float timer, int score, int nbStars, bool passed)
	{
		nextLevel.interactable=false;
		string textAffich;

		if (endType == 0)
		{
			textAffich="Unused Panels: "+nbPan+"\nAttemps: "+nbTry+"\nTime: "+timer+"s\nTotal Score: "+score;

			if(passed && !(Application.loadedLevel == Application.levelCount-1))
				nextLevel.interactable=true;

			if (passed)
				textAffich+="\n\nYou win!";
			else
				textAffich+="\n\nScore too low";


			if (nbStars>0)
				stars[0].enabled=true;
			if (nbStars>1)
				stars[1].enabled=true;
			if (nbStars>2)
				stars[2].enabled=true;

		}
		else if (endType == 1)
		{
			textAffich="You are dead...";
			foreach (Image nstar in nStars)
				nstar.enabled=false;
		}
		else
			textAffich="This end is not the good one...";

		gameOverText.text=textAffich;

		foreach(Button button in GoPauseButton)
		{
			button.interactable=false;
		}

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


	IEnumerator DisplayEnd()
	{
		yield return new WaitForSeconds(1f);
		gameOverPanel.SetActive(true);
	}

}
