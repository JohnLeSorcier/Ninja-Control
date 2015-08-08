using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;

public class MenuScript : MonoBehaviour {

	public Text cheatText;
	public Button soundOn;
	public Button soundOff;
	public Button fxOn;
	public Button fxOff;
	private GameController gameController;
	AudioSource audioS;

	public Toggle englishToggle;
	public Toggle frenchToggle;
	
	public Toggle cheatToggle;
	
	public bool cheatEnable;
	
	LanguageManager languageManager;
	
	public Slider volumeControl;
	
	public Text volumeText;

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
		{
			if (Application.systemLanguage == SystemLanguage.French)
				language="fr";
			else
				language="en";
		}
		
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
		
		string musicState;
		if (PlayerPrefs.HasKey("Music"))
			musicState=PlayerPrefs.GetString("Music");
		else
			musicState="On";

		audioS=gameController.GetComponent<AudioSource>();
		
		if (musicState == "Off")
			audioS.mute=true;
		else
			audioS.mute=false;
		
		//activer ou desactiver les boutons de son
		soundOff.gameObject.SetActive(audioS.mute);
		soundOn.gameObject.SetActive(!audioS.mute);
		
		string FXState;
		if (PlayerPrefs.HasKey("FXState"))
			FXState=PlayerPrefs.GetString("FXState");
		else
			FXState="On";
		
		bool FxStateBool;
		if (FXState == "Off")
			FxStateBool=false;
		else
			FxStateBool=true;	
			
		fxOff.gameObject.SetActive(!FxStateBool);
		fxOn.gameObject.SetActive(FxStateBool);
		
			
		if (gameController.lastLevel > 0) 
		{
			int menuindex=Mathf.FloorToInt((gameController.lastLevel-1)/6);
			GameObject MenuLevel = GameObject.Find("LevelMenus");
			MenuLevel.GetComponent<RectTransform>().GetChild(menuindex).gameObject.SetActive(true);
		}
		
		float volume;
		if (PlayerPrefs.HasKey("Volume"))
			volume=PlayerPrefs.GetFloat("Volume");
		else
			volume=100f;
		
		volumeText.text=""+volume+"%";
		volumeControl.value=volume;
		audioS.volume=gameController.volMax*volume/100;
			
		volumeControl.onValueChanged.AddListener (delegate {changeVolume();});
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
		}
	}

	public void SoundOn()
	{
		audioS.mute = false;
		PlayerPrefs.SetString("Music", "On");
		PlayerPrefs.Save();
	}
	
	public void SoundOff()
	{
		audioS.mute = true;
		PlayerPrefs.SetString("Music", "Off");
		PlayerPrefs.Save();
	}

	public void ChangeLang (string lang)
	{
		PlayerPrefs.SetString("Language", lang);
		PlayerPrefs.Save ();
	}
	
	public void ValidOptions()
	{
		PlayerPrefs.SetFloat("Volume", volumeControl.value);
		PlayerPrefs.Save ();
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void changeVolume ()
	{
		volumeText.text=""+volumeControl.value+"%";
		audioS.volume=gameController.volMax*volumeControl.value/100;
	}
	
	public void FXon()
	{
		PlayerPrefs.SetString("FXState", "On");
		PlayerPrefs.Save ();		
	}
	
	public void FXOff()
	{
		PlayerPrefs.SetString("FXState", "Off");
		PlayerPrefs.Save ();		
	}

}
