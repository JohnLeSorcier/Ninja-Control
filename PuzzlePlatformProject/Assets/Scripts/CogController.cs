using UnityEngine;
using System.Collections;

public class CogController : MonoBehaviour {

	bool isTurning;
	Vector3 direction;
	[HideInInspector] public bool StartLeft;

	void Start()
	{
		isTurning=false;
		if (StartLeft)
			direction=Vector3.forward;
		else
			direction=Vector3.back;
	}

	void Update () 
	{
		if (isTurning)
			transform.Rotate (direction * 100 * Time.deltaTime);
	}

	public void Change()
	{
		direction.z*=-1;
	}

	public void Active()
	{
		isTurning=true;
	}

	public void Desactive()
	{
		isTurning=false;
	}

}
