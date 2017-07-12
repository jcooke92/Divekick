using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{

	RequireComponent Rigidbody2D;
	RequireComponent Animator;
	RequireComponent AudioSource;

	private static Player instance;
	public GameObject score;
	public GameObject combo;
	public GameObject canflipnow;
	public int scoreValue;
	public int chainMulti;
	private int keepmulti;

	public bool isDead = false;
	public float speed = 2; //2
	public float forceX = 300f;
	public float forceY = 300f;
	public const float BACKFLIP_COOLDOWN = 2f;
	public float backflipTimer = BACKFLIP_COOLDOWN;
	public AudioClip kickSound;
	public AudioClip deathScream;
	public AudioClip manHit;
	public AudioClip carExplosion;
	public AudioClip[] comboSounds;
	public GameObject[] comboImages;

	private Rigidbody2D rigidbody;
	private Animator animator;
	private AudioSource audio;

	public static Player Instance()
	{
		return instance;
	}
		
	void Start () 
	{
		instance = this;
		rigidbody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		audio = GetComponent<AudioSource> ();
		scoreValue = 0;
		chainMulti = 1;
		keepmulti = 0;
		Jump();
		rigidbody.velocity = Vector2.right * speed; //* 1.50f;
	}
		
	void Update () 
	{

		if (rigidbody.velocity.y > 0) 
			Jump();

		processTouchInput();

		updateVelocity();

		updateBackflipStatus();

		dieIfOutOfBounds();
	}

	void processTouchInput()
	{
		if (!isDead && Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) 
		{

			Vector2 touchPos = Input.GetTouch (0).position;

			if (touchPos.x <= Screen.width / 2) 
			{

				if (backflipTimer >= BACKFLIP_COOLDOWN) 
				{
					rigidbody.velocity = (new Vector2 (0, 0));
					rigidbody.AddForce (new Vector2 (forceX * speed * -1f, forceY * speed * 0.5f));
					Backflip ();
					backflipTimer = 0.0f;
				}
			} 
			else if (touchPos.x > Screen.width / 2) 
			{
				if(animator.GetBool("isGrounded"))
				{
					rigidbody.velocity = (new Vector2 (0, 0));
					rigidbody.AddForce (new Vector2 (forceX * speed, forceY * speed));
					Jump();
				} 
				else if(animator.GetBool("isJumping"))
				{
					rigidbody.velocity = (new Vector2 (0, 0));
					rigidbody.AddForce (new Vector2 (forceX * speed * 2.0f, forceY * speed * -1.0f));
					Kick();
				}
			}
		} 
	}

	void updateVelocity()
	{
		if(animator.GetBool("isGrounded"))
		{
			float velY = rigidbody.velocity.y;
			if (rigidbody.velocity.x > speed * 1.50f) 
				rigidbody.velocity = new Vector2 (rigidbody.velocity.x - 1.5f, velY);
			else if (rigidbody.velocity.x < speed * 1.50f) 
				rigidbody.velocity = new Vector2 (rigidbody.velocity.x + 1f, velY);
		}

		if (!isDead && Input.GetKeyDown (KeyCode.Space) && animator.GetBool("isGrounded")) 
		{
			rigidbody.velocity = (new Vector2 (0, 0));
			rigidbody.AddForce (new Vector2 (forceX * speed, forceY * speed));
			Jump ();
		} 
		else if (!isDead &&Input.GetKeyDown (KeyCode.Backspace) && backflipTimer >= BACKFLIP_COOLDOWN) 
		{
			rigidbody.velocity = (new Vector2 (0, 0));
			rigidbody.AddForce (new Vector2 (forceX * speed * -1f, forceY * speed * 0.5f));
			Backflip ();
			backflipTimer = 0.0f;
		}  
		else if (!isDead &&Input.GetKeyDown(KeyCode.Space) && animator.GetBool("isJumping"))
		{
			rigidbody.velocity = (new Vector2 (0, 0));
			rigidbody.AddForce (new Vector2 (forceX * speed * 3.0f, forceY * speed * -2.0f));
			Kick ();
		}
		else if(animator.GetBool("isGrounded"))
		{
			float velY = rigidbody.velocity.y;
			if (Mathf.Abs (rigidbody.velocity.x - (speed * 1.50f)) <= 1f) 
				rigidbody.velocity = new Vector2 (3.0f, velY);
			else if (rigidbody.velocity.x > speed * 1.50f) 
				rigidbody.velocity = new Vector2 (rigidbody.velocity.x - 1f, velY);
			else if (rigidbody.velocity.x < speed * 1.50f) 
				rigidbody.velocity = new Vector2 (rigidbody.velocity.x + 1f, velY);
		}
	}

	void updateBackflipStatus()
	{
		if (backflipTimer < BACKFLIP_COOLDOWN)
			backflipTimer += Time.deltaTime;
		canflipnow.GetComponent<Text> ().text = string.Format ("No Back-Flip");

		if (backflipTimer >= BACKFLIP_COOLDOWN) 
			canflipnow.GetComponent<Text> ().text = string.Format ("Back-Flip"); 
	}

	void dieIfOutOfBounds()
	{
		Vector2 screenPosition = Camera.main.WorldToScreenPoint (transform.position);
		if (screenPosition.y < 0f)
			DieNoAnimation ();
	}

	void FixedUpdate () 
	{
		UpdateScore ();
		KeepMulti ();
	}

	void OnCollisionEnter2D(Collision2D coll) 
	{
		if (coll.gameObject.layer == 8)
		{
			if(transform.position.y < coll.collider.bounds.max.y + 0.3f)
				transform.Translate (new Vector2(0f, -1f));

			Run ();
			chainMulti = 1;
		}
		else if (coll.gameObject.tag == "Enemy")
		{
			audio.clip = kickSound;
			audio.Play ();
			if(coll.gameObject.GetComponent<Enemy>().isVehicle)
				audio.PlayOneShot (carExplosion);
			else
				audio.PlayOneShot (manHit);
			if(animator.GetBool("isKicking"))
			{
				coll.gameObject.GetComponent<Enemy>().Die();
				++chainMulti;
				CheckMultiplier ();
				rigidbody.velocity = Vector2.zero;
				rigidbody.AddForce(new Vector2(3f, 6.5f), ForceMode2D.Impulse);
				animator.SetTrigger ("Collide");
			}
			else if(coll.gameObject.GetComponent<Enemy>().isAlive())
			{
				AnimatorClipInfo[] info = animator.GetCurrentAnimatorClipInfo (0);
				if (info.Length > 0 && !info [0].clip.name.Equals ("PlayerCollide"))
				{
					coll.gameObject.GetComponent<Animator> ().SetTrigger ("Attack");
					Die ();
				}
			}
		}
	}
		
	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.gameObject.layer == 8)
		{ Vector2 screenPosition = Camera.main.WorldToScreenPoint (transform.position); }
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		GameObject chunk = collider.gameObject;

		if (chunk.GetComponent<Chunk> () != null) 
			Destroy (chunk);
	}

	void UpdateScore() 
	{
		score.GetComponent<Text> ().text = string.Format("Score: {0}\nMultiplier: {1}", scoreValue, chainMulti);
	}

	void KeepMulti()
	{
		if(keepmulti < chainMulti)
			keepmulti = chainMulti;
	}

	void CheckMultiplier()
	{
		GameObject image = null;
		AudioClip sound = null;
		float duration = 0f;

		if (chainMulti == 2)
		{
			image = comboImages [0];
			sound = comboSounds[0];
			duration = 1.25f;
		}
		else if (chainMulti == 4)
		{
			image = comboImages [1];
			sound = comboSounds[1];
			duration = 2.25f;
		}
		else if (chainMulti == 6)
		{
			image = comboImages [2];
			sound = comboSounds[2];
			duration = 2.5f;
		}
		else if (chainMulti == 10)
		{
			image = comboImages [3];
			sound = comboSounds[3];
			duration = 3f;
		}
		else if (chainMulti == 20)
		{
			image = comboImages [4];
			sound = comboSounds[4];
			duration = 3.75f;
		}

		if (sound != null)
			audio.PlayOneShot (sound);

		if (image != null)
		{
			image.SetActive (true);
			StartCoroutine (ComboMessage (image, duration));
		}
	}

	IEnumerator ComboMessage(GameObject image, float duration)
	{
		yield return new WaitForSeconds(duration);
		image.SetActive (false);

	}

	void Die()
	{
		audio.clip = deathScream;
		audio.Play ();
		isDead = true;
		animator.SetTrigger ("Die");
		rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
		StartCoroutine (WaitForTrigger ());
		StartCoroutine (WaitForDieAnimation ());
	}

	void DieNoAnimation()
	{
		audio.clip = deathScream;
		audio.Play ();
		GameData.score = scoreValue;
		GameData.time = GameManager.Instance().SecondsToTime (Mathf.FloorToInt(Time.timeSinceLevelLoad)).Substring(6);
		GameData.multiplier = keepmulti; 

		SceneManager.LoadScene ("Results");
	}

	IEnumerator WaitForTrigger()
	{
		yield return new WaitForSeconds (0.5f);
		GetComponent<BoxCollider2D> ().enabled = false;
	}

	IEnumerator WaitForDieAnimation()
	{
		yield return new WaitForSeconds (1.5f);
		DieNoAnimation ();
	}

	void UpdateAnimationState(bool isGrounded, bool isJumping = false, bool isBackflipping = false, bool isKicking = false)
	{
		animator.SetBool ("isGrounded", isGrounded);
		animator.SetBool ("isJumping", isJumping);
		animator.SetBool ("isBackflipping", isBackflipping);
		animator.SetBool ("isKicking", isKicking);
	}

	void Jump()
	{
		UpdateAnimationState(isGrounded: false, isJumping: true);
	}

	void Run()
	{
		UpdateAnimationState(isGrounded: true);
	}

	void Backflip()
	{
		UpdateAnimationState(isGrounded: false, isJumping: true, isBackflipping: true);
	}

	void Kick()
	{
		UpdateAnimationState(isGrounded: false, isJumping: true, isBackflipping: false, isKicking: true);
	}
}
