using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class TransitionScript : MonoBehaviour 
{

	void Start () 
	{
		StartCoroutine (WaitForSceneEnd ());
	}

	IEnumerator WaitForSceneEnd()
	{
		yield return new WaitForSeconds (19f);
		SceneManager.LoadScene ("City");
	}
}
