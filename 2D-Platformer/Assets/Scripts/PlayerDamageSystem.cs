using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageSystem : MonoBehaviour
{
	[Header("Bullet System")]
	[SerializeField] GameObject bullet;
	[SerializeField] int maxBullets = 3;
	[SerializeField] int bulletsUsed = 0;

	[Header("Damage System")]
	[SerializeField] Transform firePoint;
	[SerializeField] int totalHealth = 1;
	[SerializeField] int damageTaken = 0;
	[SerializeField] int damagePerBullet = 1;

	private PlayerController pc;
	private string playerNumber = "";
	private int direction = 0;

	// Start is called before the first frame update
	void Start()
    {
		pc = GetComponent<PlayerController>();
		playerNumber = pc.GetPlayerNumber();
    }

	// Update is called once per frame
	void Update() {
		//Handle fireing 
		if (Input.GetButtonDown("Fire2" + playerNumber)) {
			FireBullet();
		}
	}

	private void FireBullet() {
		//Find player direction
		direction = pc.GetDirection();

		//Check if the player has bullets left
		if (bulletsUsed >= maxBullets) { return; }

		//Calculate where the bullet should be fired
		Vector3 firePosition = new Vector3(firePoint.position.x, firePoint.position.y);

		//Fire bullet and set direction / tag
		bulletsUsed++;
		GameObject bulletClone = Instantiate(bullet, firePosition, transform.rotation);
		bulletClone.transform.localScale = new Vector3(direction, 1, 1);    //Make the bullet face the right direction
		bulletClone.tag = "bullet" + playerNumber;		//Show which player shot the bullet
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag != "wall") {
			//Check that the the enemy collided from the top of the player and that they were falling down
			if (collision.contacts[0].normal.y <= 0 && collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0) {
				//Give the stomper some upward force
				collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				HandleDeath();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		//Add damage taken and check if the player is dead
		damageTaken++;
		if (damageTaken >= totalHealth) {
			HandleDeath();
		}

		// Player isn't dead, play hit animation
		else {
			StopCoroutine(HitAnimation());	//Reset the coroutines so that they don't intersect with each other
			StartCoroutine(HitAnimation());
		}
	}

	private void HandleDeath() {
		//Find the level manager and tell it to reset the game
		LevelManager lm = FindObjectOfType<LevelManager>();
		lm.PlayerKilled(playerNumber);

		//Play sound
		FindObjectOfType<SoundManager>().PlayWallHitSound();
		//Destroy self
		Destroy(this.gameObject);
	}

	IEnumerator HitAnimation() {
		//Coroutine to momentarily turn the character red
		float hitAnimationTime = 0.1f;
		SpriteRenderer sr = GetComponent<SpriteRenderer>();

		//Turn colour red, wait, turn colour back
		sr.color = Color.red;
		yield return new WaitForSeconds(hitAnimationTime);
		sr.color = Color.white;
	}
}
