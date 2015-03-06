using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

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
		float score=nbPanel*scorePanRest - nbTry *scoreTry-timer*scoreTime;
		return Mathf.FloorToInt(score);
	}

	public void NewGame()
	{
		//FAIRE UN MESSAGE: TOUT VA ETRE SUPPRIMER
		PlayerPrefs.DeleteAll();
	}
}
