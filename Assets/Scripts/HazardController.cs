using UnityEngine;
using System.Collections;

public class HazardController : MonoBehaviour {

	
	private LevelController levelController;
	private bool hazarded=false;//éviter les bugs de double entrée dans le collider.
	private Vector3 startPosition;
	Rigidbody2D body;
	SliderJoint2D slideJoint;

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
		
			StartCoroutine(waitForHazard());
		}

	}

	IEnumerator waitForHazard()
	{
		yield return new WaitForSeconds(0.3f);
		hazarded=false;
	}



	public void resetPosition()
	{
		if(body != null && slideJoint == null)
			body.isKinematic=true;
		else if (slideJoint !=null)
			slideJoint.enabled=false;

		transform.position=startPosition;
	}

	public void nonGraviticMove()
	{
		if (slideJoint !=null)
			slideJoint.enabled=true;
	}


}

