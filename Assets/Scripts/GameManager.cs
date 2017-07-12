using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager
{

	private static GameManager instance;
	private static Sprite[] atlas;
	private static Sprite[] enemies;

	GameManager() 
	{
		Application.targetFrameRate = 60;
	}

	public static GameManager Instance()
	{
		if (instance == null)
			instance = new GameManager ();

		return instance;
	}

	public void LoadTiles() 
	{
		string tilePath = string.Format ("Sprites/Terrain/{0}", SceneManager.GetActiveScene ().name);
		atlas = Resources.LoadAll<Sprite>(tilePath);
	}

	public void LoadSprites() 
	{
		string enemyPath = string.Format ("Sprites/Characters/{0}", SceneManager.GetActiveScene ().name);
		enemies = Resources.LoadAll<Sprite> (enemyPath);
	}

	public Sprite RandomTile() 
	{
		if (atlas == null)
			LoadTiles ();

		int index = (int)(Random.value * (atlas.Length - 2) + 1);

		return atlas [index];
	}

	public Sprite RandomEnemy() 
	{
		if (enemies == null)
			LoadSprites ();

		int index = (int)(Random.value * (enemies.Length - 1));

		return enemies [index];
	}

	public Sprite PitTile() 
	{
		if (atlas == null)
			LoadTiles ();

		return atlas [0];
	}

	public string SecondsToTime(int seconds) 
	{
		int remainder = seconds % 60;

		return string.Format ("Time: {0}:{1}{2}", (seconds / 60), remainder > 9 ? "" : "0", remainder);
	}
}
