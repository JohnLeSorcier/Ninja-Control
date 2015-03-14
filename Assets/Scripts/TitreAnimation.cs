using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitreAnimation : MonoBehaviour {

	public float blinkTime=1f;
	Text Titre;
	float startTime;
	float deltaTime;
	bool fading;
	Color originalColor;
	Color secondColor;


	void Start () 
	{
		Titre=GetComponent<Text>();
		startTime=Time.time;
		originalColor=Titre.color;
		secondColor=new Vector4 (originalColor.r, originalColor.g, originalColor.b, 0.8f);
		fading=false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		deltaTime=(Time.time-startTime)/blinkTime;

		if (fading) 
			Titre.color=Color.Lerp (secondColor, originalColor,deltaTime);
		else
			Titre.color=Color.Lerp (originalColor, secondColor,deltaTime);

		if (deltaTime>1)
		{
			startTime=Time.time;
			fading=!fading;
		}



	}
}
