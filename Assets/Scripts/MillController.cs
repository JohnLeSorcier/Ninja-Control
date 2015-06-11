using UnityEngine;
using System.Collections;

public class MillController : MonoBehaviour {
	Vector3 initRot;
	LevelController levelController;
	bool init;
	float startTime;
	public bool sensHoraire=false;
	int sens;

	void Start () 
	{
		initRot=transform.eulerAngles;
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'LevelController' script");
			
		init=false;
		
		if (sensHoraire)
			sens=-1;
		else
			sens=1;
	}
	
	void Update () 
	{
		if (levelController.playerCanMove)
		{
			if (!init)
			{
				init=true;
				startTime=Time.time;
			}		
			transform.eulerAngles = new Vector3(transform.rotation.x,transform.rotation.y,sens*20*Mathf.Repeat(Time.time-startTime,36));
		}
		else
		{
			transform.eulerAngles=initRot;
			init=false;
		}
	}
}
