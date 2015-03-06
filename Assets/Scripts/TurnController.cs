using UnityEngine;
using System.Collections;

public class TurnController : MonoBehaviour {

	private LevelController levelController;
	private CharacterControllerAuto playerController;

	void Start()
	{
		//récupération de l'objet joueur
		GameObject playerObject = GameObject.FindWithTag ("Player");
		if (playerObject != null)
			playerController = playerObject.GetComponent <CharacterControllerAuto>();
		else
			Debug.Log ("Cannot find 'player' script");
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'LevelController' script");
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player") && levelController.playerCanMove)
		{
			if (gameObject.tag == "GoRight" && !playerController.facingRight)
				playerController.Flip();
			if (gameObject.tag == "GoLeft" && playerController.facingRight)
				playerController.Flip();
		}
	}


}