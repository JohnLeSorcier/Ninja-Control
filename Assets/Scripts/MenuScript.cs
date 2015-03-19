using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	LevelButtonController[] ButtonLevelTab;

	void Start()
	{
		Time.timeScale=1.0f;
	}


	public void DeletSave()
	{
		PlayerPrefs.DeleteAll();

		Application.LoadLevel(Application.loadedLevel);
	}
}
