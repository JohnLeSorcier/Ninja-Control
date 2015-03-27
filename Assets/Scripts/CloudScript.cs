using UnityEngine;
using System.Collections;

public class CloudScript : MonoBehaviour {

	Vector3 startPos;
	float endPos;
	public float vitesse=5f;
	
	void Start()
	{
		startPos=transform.position;
		endPos=-startPos.x;
	}
	
	void Update () 
	{
		transform.position=new Vector3(transform.position.x-vitesse*Time.deltaTime,transform.position.y,transform.position.z);
		
		if(transform.position.x<endPos)
			transform.position=startPos;
		
	}
}
