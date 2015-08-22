using UnityEngine;
using System.Collections;

public class EndController : MonoBehaviour {

	private LevelController levelController;
	AudioSource endSound;
	GameController gameController;
	float volMax=0.3f;

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
		
		float volume;
		if (PlayerPrefs.HasKey("Volume"))
			volume=PlayerPrefs.GetFloat("Volume");
		else
			volume=100f;
		endSound.volume=volMax*volume/100;

		
		endSound.mute=!levelController.StateFX;
	}


	

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player") && other is BoxCollider2D)
		{
			levelController.GameOver(0);
			endSound.mute=!levelController.StateFX;
			endSound.Play ();
			if (!endSound.mute)
				gameController.suspendMusic();
		}

	}
}
