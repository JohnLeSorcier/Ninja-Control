using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;

public class MenuFinController : MonoBehaviour {

	int nbLevel;
	public Text endText;
	public Button dwnldButton;
	
	LanguageManager languageManager;
	
	void Start () 
	{
		nbLevel = Application.levelCount - 1;
		
		string language;
		if(PlayerPrefs.HasKey("Language"))
			language=PlayerPrefs.GetString("Language");
		else
			language = "en";
		
		languageManager= LanguageManager.Instance;
		languageManager.ChangeLanguage(language);
		
		string endKey;	
		
		if (nbLevel<55)
		{
			endKey="Free";
			dwnldButton.gameObject.SetActive(true);
		}
		else
		{
			endKey="Support";
			dwnldButton.gameObject.SetActive(false);
		}
		
		string endTxt=languageManager.GetTextValue("Fin."+endKey+".Txt");
		endText.text=endTxt;
		
	}
	
	public void ouvrirLien()
	{	
		string link="http://www.google.fr";
		#if UNITY_ANDROID
			link="http://www.lienplaystore.com";
		#elif UNITY_IOS
			link="http://www.lienapplestore.com";
		#endif
		
		Application.OpenURL(link);
	}	
	
	public void lienCMA()
	{
		Application.OpenURL("http://www.cmasyndrome.com");
	}

	public void RetourMenu()
	{
		Application.LoadLevel(0);
	}
}
