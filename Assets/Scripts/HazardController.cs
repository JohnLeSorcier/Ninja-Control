using UnityEngine;
using System.Collections;

public class HazardController : MonoBehaviour {

	
	private LevelController levelController;
	private bool hazarded=false;//éviter les bugs de double entrée dans le collider.
	// Use this for initialization
	void Start () {
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'GameController' script");
	}
	
		
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player") && !hazarded)
		{
			hazarded=true;
			levelController.GameOver(1);
			StartCoroutine(waitForHazard());
		}

	}

	IEnumerator waitForHazard()
	{
		yield return new WaitForSeconds(0.5f);
		hazarded=false;
	}
}
