using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

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
		if(other is BoxCollider2D && other.CompareTag("Player") && levelController.playerCanMove)
		{
			if (gameObject.tag == "JumpRight" && !playerController.facingRight)
				playerController.Flip();
			if (gameObject.tag == "JumpLeft" && playerController.facingRight)
				playerController.Flip();

			playerController.Jump();
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other is BoxCollider2D && other.CompareTag("Player") && levelController.playerCanMove)
		{
			if (gameObject.tag == "JumpRight" && !playerController.facingRight)
				playerController.Flip();
			if (gameObject.tag == "JumpLeft" && playerController.facingRight)
				playerController.Flip();
			
			playerController.Jump();
		}
	}



}
