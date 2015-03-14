using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public void DeletSave()
	{
		PlayerPrefs.DeleteAll();
	}
}
