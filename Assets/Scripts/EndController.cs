using UnityEngine;
using System.Collections;

public class EndController : MonoBehaviour {

	private LevelController levelController;
	AudioSource endSound;
	GameController gameController;
	

	void Start () 
	{
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'GameController' script");
		
		
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent <GameController>();
		else
			Debug.Log ("Cannot find 'GameController' script");
		
		endSound=GetComponent<AudioSource>();
	}


	

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			levelController.GameOver(0);
			endSound.Play ();
			gameController.suspendMusic();
		}

	}
}
