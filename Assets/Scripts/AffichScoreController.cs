using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;

public class AffichScoreController : MonoBehaviour {

	int nbLevel;
	public Text ScoreText;
	float totalTime=0f;
	int totalScore=0;
	int totalStars=0;
	int totalLevel=0;
	LanguageManager languageManager;

	void Start () 
	{
		nbLevel = Application.levelCount - 1;
		ScoreText.text = "";
		float timeL;
		int score;
		int stars;
		int i;
		for (i = 1; i<= nbLevel; i++) 
		{
			if (PlayerPrefs.HasKey(i+"_stars"))
			{
				timeL=PlayerPrefs.GetFloat(i+"_time");
				score=PlayerPrefs.GetInt(i+"_score");
				stars= PlayerPrefs.GetInt(i+"_stars");
				//ScoreText.text+="\n1-"+i+"  "+timeL+"s  "+score;
				totalTime+=timeL;
				totalScore+=score;
				totalStars+=stars;
				totalLevel++;
			}
		}
		
		string language;
		if(PlayerPrefs.HasKey("Language"))
			language=PlayerPrefs.GetString("Language");
		else
			language = "en";
		
		languageManager= LanguageManager.Instance;
		languageManager.ChangeLanguage(language);	
		
		string levelCompTxt=languageManager.GetTextValue("AffSc.Lvl");
		string starsTxt=languageManager.GetTextValue("AffSc.Stars");
		string timeTxt=languageManager.GetTextValue("AffSc.Time");
		string scoreTxt=languageManager.GetTextValue("AffSc.Score");
		
		
		ScoreText.text += levelCompTxt+"\n"+totalLevel+"\n\n"+starsTxt+"\n"+totalStars+"\n\n"+timeTxt+"\n" + totalTime + "\"\n\n"+scoreTxt+"\n" + totalScore;

	}

}
