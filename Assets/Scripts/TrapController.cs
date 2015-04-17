using UnityEngine;
using System.Collections;

public class TrapController : MonoBehaviour {

	Rigidbody2D rigidParent;
	public GameObject hazard;
	Animator anim;

	void Start ()
	{
		rigidParent=hazard.GetComponent<Rigidbody2D>();
		anim=GetComponent<Animator>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			rigidParent.isKinematic=false;
			anim.SetTrigger("SwitchOn");
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			anim.SetTrigger("SwitchOff");
		}
	}
}
