using UnityEngine;
using System.Collections;

public class TrapController : MonoBehaviour {

	Rigidbody2D rigidParent;
	public GameObject hazard;

	void Start ()
	{
		rigidParent=hazard.GetComponent<Rigidbody2D>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			rigidParent.isKinematic=false;
		}
	}
}
