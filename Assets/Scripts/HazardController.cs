using UnityEngine;
using System.Collections;

public class HazardController : MonoBehaviour {

	
	private LevelController levelController;
	private bool hazarded=false;//éviter les bugs de double entrée dans le collider.
	private Vector2 startPosition;
	Rigidbody2D body;

	void Start () {
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'GameController' script");

		startPosition=transform.position;
		body = GetComponent<Rigidbody2D>();
	}
	
		
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other is BoxCollider2D && other.CompareTag("Player") && !hazarded)
		{
			hazarded=true;
			if (gameObject.layer==4)
				levelController.GameOver(4);
			else
				levelController.GameOver(1);
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
		if(body != null)
			body.isKinematic=true;
		transform.position=startPosition;
	}


}

