using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;

public class LevelButtonController : MonoBehaviour {

	public int levelIndex;

	public Image[] starsImage;
	public Image[] nStarsImage;
	public Image cash;
	public GameObject ErrorPanel;
	public Text supportText;
	string cheatC;
	LanguageManager languageManager;
	

	int stars=0;
	int score=0;

	void Start() 
	{
		if (PlayerPrefs.HasKey ("Cheats_Enabled"))
			cheatC=PlayerPrefs.GetString("Cheats_Enabled");
		else
			cheatC="No";
		
		string language;
		if(PlayerPrefs.HasKey("Language"))
			language=PlayerPrefs.GetString("Language");
		else
			language = "en";
		
		languageManager= LanguageManager.Instance;
		languageManager.ChangeLanguage(language);	
		

		DisplayButton ();
	}

	public void DisplayButton()
	{
		Button button=GetComponent<Button>();
		Text buttonText=GetComponentInChildren<Text>();

		
		for(int i=0;i<3;i++)
		{
			starsImage[i].enabled=false;
		}

		int prevLevelIndex=levelIndex-1;

		if(levelIndex==1 || PlayerPrefs.HasKey(prevLevelIndex+"_stars") || cheatC == "Yes")
		{
			button.interactable=true;
			cash.enabled=false;
		}
		else
			button.interactable=false;
			
		string levelTxt=languageManager.GetTextValue("Menu.Lvl");
			
		string bTxt=levelTxt+" "+levelIndex+"\n";
		
		if(PlayerPrefs.HasKey(levelIndex+"_stars"))
		{
			stars=PlayerPrefs.GetInt(levelIndex+"_stars");
			score=PlayerPrefs.GetInt(levelIndex+"_score");
			bTxt+=score+" pts";
		}
		
		buttonText.text=bTxt;
		
		if(stars>0)
		{
			for(int i=0;i<stars;i++)
			{
				starsImage[i].enabled=true;
				nStarsImage[i].enabled=false;
			}
		}
		
		supportText.enabled=false;
		if (levelIndex>Application.levelCount-2)
		{
			supportText.enabled=true;
			cash.enabled=true;
			button.enabled=false;
		}	
	}

	public void LoadLevel()
	{
		if (levelIndex<Application.levelCount)
			Application.LoadLevel(levelIndex);
		else
			ErrorPanel.SetActive(true);
	}

}
