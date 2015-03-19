using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	int finishScore=500;
	int scorePanRest=250;
	int scoreTry=150;
	int scoreTime=200;

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
		float score=finishScore + timer*scoreTime + nbPanel*scorePanRest - nbTry *scoreTry;
		return Mathf.FloorToInt(score);
	}

}
