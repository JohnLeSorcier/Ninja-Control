using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;

public class MenuScript : MonoBehaviour {

	public Text cheatText;
	public Button soundOn;
	public Button soundOff;
	private GameController gameController;
	AudioSource audioS;

	public Toggle englishToggle;
	public Toggle frenchToggle;
	
	public Toggle cheatToggle;
	
	public bool cheatEnable;
	
	LanguageManager languageManager;

	void Start()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent <GameController>();
		else
			Debug.Log ("Cannot find 'GameController' script");

		Time.timeScale=1.0f;
		string CheatC = "No";
		
		if (PlayerPrefs.HasKey("Cheats_Enabled"))
			CheatC=PlayerPrefs.GetString("Cheats_Enabled");

		string language;
		if(PlayerPrefs.HasKey("Language"))
		   language=PlayerPrefs.GetString("Language");
		else
			language = "en";
		
		if (language == "en")
		{
			englishToggle.isOn = true;
			frenchToggle.isOn = false;
		}
		if (language == "fr")
		{
			frenchToggle.isOn = true;
			englishToggle.isOn = false;
		}
		
		languageManager= LanguageManager.Instance;
		languageManager.ChangeLanguage(language);
		
		if(CheatC == "Yes")
		{
			cheatEnable=true;
			cheatText.enabled=true;
			cheatToggle.isOn=true;
		}
		else
		{
			cheatEnable=false;
			cheatText.enabled=false;
			cheatToggle.isOn=false;
		}
			
	

		audioS=gameController.GetComponent<AudioSource>();
		
		//activer ou desactiver les boutons de son
		soundOff.gameObject.SetActive(audioS.mute);
		soundOn.gameObject.SetActive(!audioS.mute);
			
		if (gameController.lastLevel > 0) 
		{
			int menuindex=Mathf.FloorToInt((gameController.lastLevel-1)/6);
			GameObject MenuLevel = GameObject.Find("LevelMenus");
			MenuLevel.GetComponent<RectTransform>().GetChild(menuindex).gameObject.SetActive(true);
		}
	}


	public void DeletSave()
	{
		PlayerPrefs.DeleteAll();

		Application.LoadLevel(Application.loadedLevel);
	}

	public void CheatEnabled()
	{

		if (cheatEnable == cheatToggle.isOn)
			return;			
		else 
		{
			string CheatO;
			if (cheatToggle.isOn)
				CheatO="Yes";
			else
				CheatO="No";

		PlayerPrefs.SetString("Cheats_Enabled",CheatO);
		PlayerPrefs.Save ();
		}
	}

	public void SoundOn()
	{
		audioS.mute = false;
	}
	
	public void SoundOff()
	{
		audioS.mute = true;
	}

	public void ChangeLang (string lang)
	{
		PlayerPrefs.SetString("Language", lang);
	}
	
	public void ReloadLevel()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
}
