using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelButtonController : MonoBehaviour {

	public int levelIndex;

	public Image[] starsImage;
	public Image[] nStarsImage;
	public Image cash;
	public GameObject ErrorPanel;

	int stars=0;

	void Start () 
	{
		Button button=GetComponent<Button>();

		for(int i=0;i<3;i++)
		{
			starsImage[i].enabled=false;
		}

		int prevLevelIndex=levelIndex-1;
		if(levelIndex==1 || PlayerPrefs.HasKey(prevLevelIndex+"_stars"))
		{
			button.interactable=true;
			cash.enabled=false;
		}
		else
			button.interactable=false;

		if(PlayerPrefs.HasKey(levelIndex+"_stars"))
		{
			stars=PlayerPrefs.GetInt(levelIndex+"_stars");
		}



		if(stars>0)
		{
			cash.enabled=false;
			for(int i=0;i<stars;i++)
			{
				starsImage[i].enabled=true;
				nStarsImage[i].enabled=false;
			}
		}	
	}

	public void LoadLevel()
	{
		if (levelIndex<Application.levelCount)
			Application.LoadLevel(levelIndex);
		else
			ErrorPanel.SetActive(true);
	}

}
