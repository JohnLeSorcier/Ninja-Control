using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AffichScoreController : MonoBehaviour {

	int nbLevel;
	public Text ScoreText;
	float totalTime=0f;
	int totalScore=0;

	void Start () 
	{
		nbLevel = Application.levelCount - 1;
		ScoreText.text = "Level Time Score";
		float timeL;
		int score;
		int i;
		for (i = 1; i<= nbLevel; i++) 
		{
			if (PlayerPrefs.HasKey(i+"_stars"))
			{
				timeL=PlayerPrefs.GetFloat(i+"_time");
				score =PlayerPrefs.GetInt(i+"_score");
				ScoreText.text+="\n1-"+i+"  "+timeL+"s  "+score;
				totalTime+=timeL;
				totalScore+=score;
			}
		}
		ScoreText.text += "\n Total time = " + totalTime + "s\n Total Score = " + totalScore;

		ScoreText.resizeTextMaxSize = 11;

	}

}
