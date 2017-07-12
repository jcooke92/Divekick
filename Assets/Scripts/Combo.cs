using UnityEngine;
using System.Collections;

public class Combo : MonoBehaviour 
{
	void FixedUpdate () {
		if (gameObject.activeSelf)
			gameObject.GetComponent<RectTransform> ().Rotate (new Vector3 (0f, 0f, 5f));
	}
}
