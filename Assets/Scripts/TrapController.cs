using UnityEngine;
using System.Collections;

public class TrapController : MonoBehaviour {

	Rigidbody2D rigidParent;
	public GameObject hazardG;
	HazardController hazard;
	Animator anim;
	public bool gravitic;

	void Start ()
	{
		rigidParent=hazardG.GetComponent<Rigidbody2D>();
		hazard=hazardG.GetComponent<HazardController>();
		anim=GetComponent<Animator>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other is BoxCollider2D && other.CompareTag("Player"))
		{
			anim.SetTrigger("SwitchOn");
			if (gravitic)
				rigidParent.isKinematic=false;
			else
				hazard.nonGraviticMove();
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
