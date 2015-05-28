﻿using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	int finishScore=500;
	int scorePanRest=250;
	int scoreTry=100;
	int scoreTime=200;
	int scorePiece=100;

	public int lastLevel;

	public static GameController control;

	void Awake()
	{
		if (control == null)
		{
			DontDestroyOnLoad(gameObject);
			control=this;
			lastLevel=0;
		}
		else if (control != this)
		{
			Destroy(gameObject);
		}

	}

	public int Score(int nbPanel, int nbTry, int nbPieces, float timer)
	{
		float score=finishScore + timer*scoreTime + nbPanel*scorePanRest + nbPieces*scorePiece - nbTry *scoreTry;
		return Mathf.FloorToInt(score);
	}

	public void retourMenu()
	{
		lastLevel = Application.loadedLevel;
		Application.LoadLevel("MenuPrincipal");
	}
}
