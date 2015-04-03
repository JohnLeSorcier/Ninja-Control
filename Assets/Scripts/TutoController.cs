using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutoController : MonoBehaviour {

	public Button[] listButton;
	public GameObject[] listObject;

	void Start ()
	{
		PlaceController placeC;

		foreach(Button button in listButton)
		{
			if(button!=null)
				button.interactable=false;
		}

		foreach(GameObject objet in listObject)
		{
			if(objet!=null)
			{
				placeC=objet.GetComponent<PlaceController>();
				if(placeC!=null)
					placeC.enabled=false;
			}
		}

	}

	public void Reactivate()
	{
		PlaceController placeC;
		foreach(GameObject objet in listObject)
		{
			if(objet!=null)
			{
				placeC=objet.GetComponent<PlaceController>();
				if(placeC!=null)
					placeC.enabled=true;
			}
		}
		foreach(Button button in listButton)
		{
			if(button!=null)
				button.interactable=true;
		}
	}

}
