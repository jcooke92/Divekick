using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour {

	public float tileSize;
	public int chunkSize;
	public GameObject template;
	private GameObject[] tiles;

	void Start() 
	{
		template.GetComponent<EdgeCollider2D> ().offset = new Vector2 (0f, 0.5f * tileSize);

		float positionX = this.gameObject.transform.position.x - (tileSize * 0.5f * (chunkSize - 1f));
		float positionY = this.gameObject.transform.position.y;

		tiles = new GameObject[chunkSize];

		for (int i = 0; i < tiles.Length; i++) 
		{
			tiles [i] = (GameObject)Instantiate (template, new Vector3 (positionX, positionY, 0f), Quaternion.identity);
			positionX += tileSize;
		}

		foreach (GameObject tile in tiles)
		{
			Sprite tileSprite = GameManager.Instance ().RandomTile ();
			tile.GetComponent<SpriteRenderer> ().sprite = tileSprite;
		}
	}
		
	public void Die()
	{
		if (gameObject == null)
			return;
		
		foreach (GameObject tile in tiles)
			if(tile != null)
				Destroy (tile);
		
		Destroy (gameObject, 1f);
	}

	public float GetRightEdge()
	{
		return this.gameObject.transform.position.x + (tileSize * 0.5f * (chunkSize - 1f));
	}

	public float GetWidth() 
	{
		return tileSize * chunkSize;
	}
}
