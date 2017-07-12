using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraScroll : MonoBehaviour {

	public GameObject player;
	private float cameraSpeed;
	public GameObject boundaryLeft;
	public GameObject boundaryRight;
	public GameObject boundaryTop;

	private Vector3 cameraVector;
	private Vector2 boundaryLeftVector;
	private Vector2 boundaryRightVector;
	private Vector2 boundaryTopVector;

	void Start ()
	{
		initScreenBoundaryPositions();
		cameraSpeed = 0.07f; 
	}

	void FixedUpdate () 
	{
		updateScreenBoundaryPositions();
	}

	void initScreenBoundaryPositions()
	{
		boundaryLeftVector = new Vector2 (boundaryLeft.transform.position.x, boundaryLeft.transform.position.y);
		boundaryRightVector = new Vector2 (boundaryRight.transform.position.x, boundaryRight.transform.position.y);
		boundaryTopVector = new Vector2 (boundaryTop.transform.position.x, boundaryTop.transform.position.y);
	}

	void updateScreenBoundaryPositions()
	{
		cameraVector = transform.position;
		cameraVector.x += cameraSpeed;
		transform.position = cameraVector;
		boundaryLeftVector.x += cameraSpeed;
		boundaryLeft.transform.position = boundaryLeftVector;
		boundaryRightVector.x += cameraSpeed;
		boundaryRight.transform.position = boundaryRightVector;
		boundaryTopVector.x += cameraSpeed;
		boundaryTop.transform.position = boundaryTopVector;
	}
}
