using UnityEngine;
using System.Collections;

public class InvisibleWallController : MonoBehaviour {

	private LevelController levelController;
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
		if(other.CompareTag("Player"))
		{
			levelController.GameOver(2);
		}
		
	}
}
