using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

	public Text cheatText;

	void Start()
	{
		Time.timeScale=1.0f;
		string CheatC = "No";
		
		if (PlayerPrefs.HasKey("Cheats_Enabled"))
			CheatC=PlayerPrefs.GetString("Cheats_Enabled");

		if(CheatC == "Yes")
			cheatText.enabled=true;
		else
			cheatText.enabled=false;
	}


	public void DeletSave()
	{
		PlayerPrefs.DeleteAll();

		Application.LoadLevel(Application.loadedLevel);
	}

	public void CheatEnabled()
	{
		string CheatC = "No";
		
		if (PlayerPrefs.HasKey("Cheats_Enabled"))
			CheatC=PlayerPrefs.GetString("Cheats_Enabled");
		
		string newCheatC="No";
		
		if (CheatC == "No")
			newCheatC="Yes";
		
		PlayerPrefs.SetString("Cheats_Enabled",newCheatC);
		PlayerPrefs.Save ();
		Application.LoadLevel(Application.loadedLevelName);
	}

}
