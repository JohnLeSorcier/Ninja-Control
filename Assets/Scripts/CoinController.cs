using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour {

	private LevelController levelController;
	private bool invisible;
	SpriteRenderer sprite;
	
	AudioSource coinSound;

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
			coinSound.Play();
		}
	}
}
