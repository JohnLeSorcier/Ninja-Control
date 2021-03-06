﻿using UnityEngine;
using System.Collections;

public class HazardController : MonoBehaviour {

	
	private LevelController levelController;
	private bool hazarded=false;//éviter les bugs de double entrée dans le collider.
	private Vector3 startPosition;
	Rigidbody2D body;
	SliderJoint2D slideJoint;
	public BoxCollider2D wallC;

	void Start () {
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'LevelController' script");

		startPosition=transform.position;
		body = GetComponent<Rigidbody2D>();
		slideJoint=GetComponent<SliderJoint2D>();
		if (slideJoint !=null)
			slideJoint.enabled=false;
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if(other is BoxCollider2D && other.CompareTag("Player") && !hazarded)
		{
			hazarded=true;
			//le layer 4, c'est l'eau
			if (gameObject.layer==4)
				levelController.GameOver(4);
			else if (body != null && slideJoint == null)
				levelController.GameOver(6);
			else
				levelController.GameOver(1);
			//en cas de noyade->gameOver 4, en cas d'écrasement par un truc qui tombe->gameOver 6, sinon gameOver 1
			
			if (wallC !=null)
				wallC.isTrigger=true;
		
			StartCoroutine(waitForHazard());
		}

	}

	IEnumerator waitForHazard()
	{
		yield return new WaitForSeconds(0.3f);
		hazarded=false;
	}

	IEnumerator waitForSlide()
	{
		yield return new WaitForSeconds(0.1f);
		slideJoint.enabled=false;
		transform.position=startPosition;
	}

	public void resetPosition()
	{
		if (wallC!=null)
			wallC.isTrigger=true;	
		if(body != null && slideJoint == null)
		{
			body.isKinematic=true;
			transform.position=startPosition;
		}
		else if (slideJoint !=null)
			StartCoroutine(waitForSlide()); //permet d'attendre que le slide ne bouge plus et éviter qu'il parte dans el vide		
	}

	public void nonGraviticMove()
	{
		if (slideJoint !=null)
			slideJoint.enabled=true;
	}


}

