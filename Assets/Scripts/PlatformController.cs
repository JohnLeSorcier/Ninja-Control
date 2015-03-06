using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {

	private SliderJoint2D[] slide;
	private Vector3 origin;
	private Quaternion rotation;
	private CogController cog;
	public bool isStartLeft;

	void Start () 
	{
		slide=GetComponents<SliderJoint2D>();
		slide[0].enabled=false;
		slide[1].enabled=false;

		cog=GetComponentInChildren<CogController>();
		cog.StartLeft=isStartLeft;

		origin=transform.position;
		rotation=transform.rotation;
		rigidbody2D.isKinematic=true;
	}
	

	void Update ()
	{
		if (slide[0].limitState == JointLimitState2D.UpperLimit)
		{
			slide[0].enabled=false;
			slide[1].enabled=true;
			cog.Change();
		}
		if (slide[1].limitState == JointLimitState2D.LowerLimit)
		{
			slide[1].enabled=false;
			slide[0].enabled=true;
			cog.Change();
		}
	}

	public void Active()
	{
		if (isStartLeft)
			slide[0].enabled=true;
		else
			slide[1].enabled=true;
		rigidbody2D.isKinematic=false;
		cog.Active();
	}

	public void Desactive()
	{
		slide[1].enabled=false;
		slide[0].enabled=false;
		transform.position=origin;
		transform.rotation=rotation;
		rigidbody2D.isKinematic=true;
		cog.Desactive();

	}
}
