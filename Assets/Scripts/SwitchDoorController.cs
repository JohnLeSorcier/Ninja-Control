using UnityEngine;
using System.Collections;

public class SwitchDoorController : MonoBehaviour {

	DoorController door;
	Animator anim;
	public GameObject doorObject;

	void Start () 
	{
		door=doorObject.GetComponent<DoorController>();
		anim=GetComponent<Animator>();
	}



	void OnTriggerEnter2D(Collider2D other)
	{
		if(other is BoxCollider2D && other.CompareTag("Player"))
		{
			anim.SetTrigger("SwitchOn");
			door.Active();
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other is BoxCollider2D && other.CompareTag("Player"))
		{
			anim.SetTrigger("SwitchOff");
		}

	}
	
}
