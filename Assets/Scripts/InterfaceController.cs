using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour {
	
	public Text gameOverText;
	private LevelController levelController;
	public GameObject gameOverPanel;
	public GameObject[] GoPauseButton;



	void Start () 
	{
		levelController = GetComponent <LevelController>();
		gameOverText.text="";
		levelController.MaJNbText();
		gameOverPanel.SetActive(false);
	}


	public void ReloadLevel()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}

	//affiche un message différent suivant le type de fin: 0 pour réussite, 1 pour une mort
	public void GameOver(int endType, int nbPan, int nbTry, float timer, int score)
	{
		if (endType == 0)
		{
			gameOverText.text="You win!\n"+"Unused Panels: "+nbPan+"\nAttemps: "+nbTry+"\nTime: "+timer+"s\nTotal Score: "+score;
		}
		else if (endType == 1)
			gameOverText.text="You are dead...";
		else
			gameOverText.text="This end is not the good one...";

		Button button;

		foreach(GameObject GPbutton in GoPauseButton)
		{
			button=GPbutton.GetComponent<Button>();
			button.interactable=false;
		}

		StartCoroutine(DisplayEnd());
	}

	public void ReturnMenu()
	{
		Application.LoadLevel("MenuPrincipal");
	}

	IEnumerator DisplayEnd()
	{
		yield return new WaitForSeconds(1f);
		gameOverPanel.SetActive(true);
	}

}
