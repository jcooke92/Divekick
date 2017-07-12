using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class InterfaceManager : MonoBehaviour {
	

	public GameObject scoreText;
	public GameObject timeText;
	public GameObject multiText;

	void Start () 
	{
		if (SceneManager.GetActiveScene ().name.Equals ("Results")) 
		{
			scoreText.GetComponent<Text> ().text = GameData.score.ToString ();
			timeText.GetComponent<Text> ().text = GameData.time;
			multiText.GetComponent<Text> ().text = GameData.multiplier.ToString ();
		}
	}

	public void MainMenu() 
	{
		SceneManager.LoadScene ("MainMenu");
	}

	public void StartGame()
	{
		SceneManager.LoadScene ("City");
	}

	public void StartIntro()
	{
		SceneManager.LoadScene ("Intro");
	}

	public void Credits()
	{
		SceneManager.LoadScene ("Credits");
	}
}
