using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{

	RequireComponent Rigidbody2D;
	RequireComponent BoxCollider2D;

	public bool isVehicle;
	public float speed;
	public float forceX = 12f;
	public float forceY = 5f;
	public float vehicleDeathTimer = 0f;
	private Vector2 position;
	private Rigidbody2D rigidbody;
	private BoxCollider2D boxCollider;

	private Animator animator;

	void Start () 
	{
		rigidbody = GetComponent<Rigidbody2D> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		animator = GetComponent<Animator> ();
	}

	void Update ()
	{
		destroyIfOffScreen();

		updateScreenPosition();
	}

	void OnCollisionEnter2D(Collision2D coll) 
	{
		if (coll.gameObject.tag == "Enemy") {
			Enemy target = coll.gameObject.GetComponent<Enemy> ();
			if (target.isAlive() && !target.isVehicle)
				target.Die ();
			
			if (target.isVehicle && !isVehicle) Destroy (gameObject);
		}

		if (!isVehicle && coll.gameObject.tag == "Player")
			speed = 0;
	}

	public bool isAlive()
	{
		if (isVehicle && boxCollider.enabled)
			return true;

		if (!isVehicle && transform.rotation.Equals (Quaternion.identity))
			return true;

		return false;
	}

	public void Die()
	{
		Player.Instance ().scoreValue += Player.Instance ().chainMulti * 100;
		animator.SetTrigger ("Die");
		if (isVehicle)
			StartCoroutine (VehicleDeath ());
		else
			PedestrianDeath ();
	}

	IEnumerator VehicleDeath()
	{
		yield return new WaitForSeconds(0.5f);
		boxCollider.enabled = false;
		rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
		Destroy (gameObject);
	}

	void PedestrianDeath()
	{
		transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		Vector2 force = new Vector2(15f, 10f);
		rigidbody.AddForce(force, ForceMode2D.Impulse);
	}

	void destroyIfOffScreen()
	{
		Vector2 screenPosition = Camera.main.WorldToScreenPoint (transform.position);
		if (screenPosition.y < 0f)
			Destroy(gameObject);
		if (!isAlive () && screenPosition.x > Screen.width)
			Destroy (gameObject);
	}

	void updateScreenPosition()
	{
		position = transform.position;
		position.x -= speed * Time.deltaTime;
		transform.position = position;
	}
}
