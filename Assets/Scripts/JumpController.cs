﻿using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

	private LevelController levelController;
	private CharacterControllerAuto playerController;
	private PlaceController place;
	
			
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
			
		place=GetComponent<PlaceController>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		
		if (!place.isOnGround())
			return;
		if(other is BoxCollider2D && other.CompareTag("Player") && levelController.playerCanMove)
		{
			playerController.Jump();

			if (gameObject.tag == "JumpRight" && !playerController.facingRight)
				playerController.Flip();
			if (gameObject.tag == "JumpLeft" && playerController.facingRight)
				playerController.Flip();
		}
	}

/*	void OnTriggerExit2D(Collider2D other)
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
*/

}
