using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

	public Text cheatText;
	public Button soundOn;
	public Button soundOff;
	private GameController gameController;
	AudioSource audioS;

	void Start()
	{
		Time.timeScale=1.0f;
		string CheatC = "No";
		
		if (PlayerPrefs.HasKey("Cheats_Enabled"))
			CheatC=PlayerPrefs.GetString("Cheats_Enabled");

		if(CheatC == "Yes")
			cheatText.enabled=true;
		else
			cheatText.enabled=false;


		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent <GameController>();
		else
			Debug.Log ("Cannot find 'GameController' script");
		
		audioS=gameController.GetComponent<AudioSource>();
		
		//activer ou desactiver les boutons de son
		soundOff.gameObject.SetActive(audioS.mute);
		soundOn.gameObject.SetActive(!audioS.mute);

	}


	public void DeletSave()
	{
		PlayerPrefs.DeleteAll();

		Application.LoadLevel(Application.loadedLevel);
	}

	public void CheatEnabled()
	{
		string CheatC = "No";
		
		if (PlayerPrefs.HasKey("Cheats_Enabled"))
			CheatC=PlayerPrefs.GetString("Cheats_Enabled");
		
		string newCheatC="No";
		
		if (CheatC == "No")
			newCheatC="Yes";
		
		PlayerPrefs.SetString("Cheats_Enabled",newCheatC);
		PlayerPrefs.Save ();
		Application.LoadLevel(Application.loadedLevelName);
	}

	public void SoundOn()
	{
		audioS.mute = false;
	}
	
	public void SoundOff()
	{
		audioS.mute = true;
	}

}
