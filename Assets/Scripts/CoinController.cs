using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour {

	private LevelController levelController;
	private bool invisible;
	SpriteRenderer sprite;
	
	AudioSource coinSound;
	float volMax=0.5f;

	void Start()
	{
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'LevelController' script");
			
		coinSound=GetComponent<AudioSource>();

		sprite=GetComponent<SpriteRenderer>();

		invisible=false;
		
		float volume;
		if (PlayerPrefs.HasKey("Volume"))
			volume=PlayerPrefs.GetFloat("Volume");
		else
			volume=100f;
		coinSound.volume=volMax*volume/100;
		
		
		coinSound.mute=!levelController.StateFX;
	}

	void Update()
	{
		if (invisible && !levelController.playerCanMove)
			invisible=false;

		if(invisible)
			sprite.enabled=false;
		else
			sprite.enabled=true;

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player") && levelController.playerCanMove && !invisible)
		{
			levelController.RamassePiece();
			invisible=true;
			coinSound.mute=!levelController.StateFX;
			if(!coinSound.isPlaying)
				coinSound.Play();
		}
	}
}
