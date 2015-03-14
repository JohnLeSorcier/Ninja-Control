using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	int finishScore=1000;
	int scorePanRest=150;
	int scoreTry=100;
	int scoreTime=50;

	public static GameController control;

	void Awake()
	{
		if (control == null)
		{
			DontDestroyOnLoad(gameObject);
			control=this;
		}
		else if (control != this)
		{
			Destroy(gameObject);
		}
	}

	public int Score(int nbPanel, int nbTry, float timer)
	{
		float score=finishScore + nbPanel*scorePanRest - nbTry *scoreTry-timer*scoreTime;
		return Mathf.FloorToInt(score);
	}

}
