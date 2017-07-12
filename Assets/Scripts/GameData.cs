using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour 
{

	public static int score;
	public static string time;
	public static int multiplier;

	void Start () 
	{
		score = 0;
		time = "0:00";
		multiplier = 0;
	}
}
