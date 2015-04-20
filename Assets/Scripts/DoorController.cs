using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

	private SliderJoint2D slide;
	private Vector3 origin;
	private Quaternion rotation;

	void Start () 
	{
		slide=GetComponent<SliderJoint2D>();
		origin=transform.position;
		rotation=transform.rotation;
		rigidbody2D.isKinematic=true;
	}

	public void Active()
	{
		slide.enabled=true;
		rigidbody2D.isKinematic=false;
	}

	public void Desactive()
	{
		slide.enabled=false;
		transform.position=origin;
		transform.rotation=rotation;
		rigidbody2D.isKinematic=true;

	}

}
