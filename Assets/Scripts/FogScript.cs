using UnityEngine;
using System.Collections;

public class FogScript : MonoBehaviour {

	Vector3 startPos;
	float endPos=41f;

	void Start()
	{
		startPos=transform.position;
	}

	void Update () 
	{
		transform.position=new Vector3(transform.position.x+5*Time.deltaTime,transform.position.y,transform.position.z);

		if(transform.position.x>endPos)
			transform.position=startPos;

	}
}
