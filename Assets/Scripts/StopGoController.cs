using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StopGoController : MonoBehaviour {

	private LevelController levelController;
	private CharacterControllerAuto playerController;
	PlaceController placeControl;

	public Text SGText;

	private float timeAllowed;
	private bool waitingPlayer;
	private float startTime;
	private float timerTime;
	
	private bool used;
	
	private Vector3 positionInit;

	Animator anim;
	
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

		anim=GetComponent<Animator>();

		placeControl=GetComponent<PlaceController>();

		SGText.text="";
		timeAllowed=1;
		waitingPlayer=false;
		startTime=0f;
		used=false;

	}

	void Update()
	{
		if (placeControl.isOnGround() && !levelController.playerCanMove)
		{
			waitingPlayer=false;
			SGText.text=""+Mathf.FloorToInt(timeAllowed);
			anim.SetBool("isGreen", false);
			used=false;
		}

		if(waitingPlayer)
		{
			float timerTemp=Time.time-startTime;
			float timer=timeAllowed-Mathf.Floor(timerTemp*100)/100;
			int sec=Mathf.FloorToInt(timer)+1;
			if(sec==0)
			{
				waitingPlayer=false;
				playerController.GoWait();
				anim.SetBool("isGreen", true);
			}
			SGText.text=""+sec;
		}

	}

	void OnMouseDown()
	{
		if (levelController.playerCanMove)
			return;	
		
		positionInit=transform.position;
	
	}
	

	
	void OnMouseUp()
	{
		if (levelController.playerCanMove)
			return;
			

		if (placeControl.isOnGround() && transform.position==positionInit)
		{
			timeAllowed++;
			if(timeAllowed>5)
				timeAllowed=1;
			SGText.text=""+Mathf.FloorToInt(timeAllowed);
		}

	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (!placeControl.isOnGround())
			return;
		if(other is BoxCollider2D && other.CompareTag("Player") && levelController.playerCanMove && timeAllowed>0 && !used)
		{
			playerController.StopWait();
			waitingPlayer=true;
			startTime=Time.time;
			used=true;
		}
	}
}
