using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour 
{

	public GameObject chunk;
	public BoxCollider2D player;
	public GameObject backgroundA;
	public GameObject backgroundB;
	public GameObject time;
	public GameObject pedestrian;
	public GameObject car;
	public GameObject boundaryRight;
	public GameObject boundaryLeft;
	public Sprite[] backgroundSprites;
	private List<GameObject> clones;
	private float transitionPoint;
	private float leftSide;
	private float width;
	private int enemyCooldown;
	private float enemySpawnTimer;
	private bool isMoving;

	void Start () 
	{
		GameManager.Instance().LoadTiles();
		width = backgroundA.GetComponent<BoxCollider2D> ().bounds.size.x;
		leftSide = backgroundA.GetComponent<BoxCollider2D> ().bounds.min.x;
		transitionPoint = backgroundA.transform.position.x + 0.1f * width;
		clones = new List<GameObject> ();
		isMoving = true;
		leftSide = CreateChunks(width);
		isMoving = false;

		enemyCooldown = (int)(Random.value * 2) + 2;

		enemySpawnTimer = 0f;
	}

	void Update () 
	{
		updateBackground();

		attemptEnemySpawn();
	}

	void updateBackground()
	{
		if (Camera.main.transform.position.x > transitionPoint && !isMoving) 
		{
			isMoving = true;

			if (backgroundA.GetComponent<BoxCollider2D> ().IsTouching (player)) 
				JoinBackground (backgroundA, backgroundB);
			else if (backgroundB.GetComponent<BoxCollider2D> ().IsTouching (player)) 
				JoinBackground (backgroundB, backgroundA);
			else 
				Debug.Log ("Player collider not found");
		}
	}

	void attemptEnemySpawn()
	{
		if (enemySpawnTimer <= enemyCooldown) 
		{
			enemySpawnTimer += Time.deltaTime;
		} 
		else if (enemySpawnTimer > enemyCooldown) 
		{
			SpawnEnemies ();
			enemySpawnTimer = 0f;
		}
	}
		
	void FixedUpdate() 
	{
		UpdateTime ();
	}

	void JoinBackground(GameObject currentBackground, GameObject nextBackground)
	{
		DestroyChunks ();
		int index = (int)(Random.value*backgroundSprites.Length);
		nextBackground.GetComponent<SpriteRenderer> ().sprite = backgroundSprites [index];
		Vector3 nextPosition = currentBackground.transform.position;
		nextPosition.x += width;
		nextBackground.transform.position = nextPosition;
		leftSide = CreateChunks(width);
		transitionPoint += width;
		isMoving = false;
	}

	float CreateChunks(float width) {
		float chunkWidth = chunk.GetComponent<Chunk>().GetWidth();
		float rightEdge = 0f;

		for (float i = 0f; i < width; i += chunkWidth) 
		{
			if (i + chunkWidth >= width)
				rightEdge = CreateChunk (leftSide + i, -2f);
			else
				CreateChunk (leftSide + i);

		}

		return rightEdge;
	}

	void DestroyChunks()
	{
		clones.RemoveAll (c => c == null);

		foreach (GameObject clone in clones)
			if(clone.transform.position.x < (boundaryLeft.transform.position.x - 3f))
				clone.GetComponent<Chunk> ().Die ();
	}

	float CreateChunk(float xPosition, float yOffset = 0f)
	{
		float chunkWidth = chunk.GetComponent<Chunk> ().GetWidth();
		Vector3 nextPosition = chunk.transform.position;
		nextPosition.x = xPosition + (0.5f * chunkWidth);
		nextPosition.y += yOffset;

		GameObject nextChunk = Instantiate (chunk, nextPosition, Quaternion.identity) as GameObject;
		clones.Add (nextChunk);
		return nextChunk.transform.position.x + (0.5f * chunkWidth);
	}

	void SpawnEnemies()
	{
		if (Random.value * 4 > 3f)
			SpawnCar ();
		else
			SpawnPedestrians ();
	}

	void SpawnCar()
	{
		Vector3 nextPosition = boundaryRight.transform.position;
		nextPosition.y = car.transform.position.y;
		Instantiate (car, nextPosition, Quaternion.identity);
	}

	void SpawnPedestrians()
	{
		int numEnemies = ((int)(Random.value*2)+1);

		Vector3 nextPosition = boundaryRight.transform.position;
		nextPosition.y = pedestrian.transform.position.y-0.4f;

		for (int i = 0; i < numEnemies; ++i)
		{
			Instantiate (pedestrian, nextPosition, Quaternion.identity);
			nextPosition.x += 5f;
		}
	}

	void UpdateTime() 
	{
		time.GetComponent<Text> ().text = GameManager.Instance().SecondsToTime (Mathf.FloorToInt(Time.timeSinceLevelLoad));
	}
}
