using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] float bulletSpeed = 5;
	[SerializeField] GameObject bulletParticles;
	private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		FindObjectOfType<SoundManager>().PlayBulletFireSound();
    }

	// Update is called once per frame
	void FixedUpdate() {
		rb.velocity = new Vector2(bulletSpeed * transform.localScale.x, 0);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		//Check if it is colliding with bullets from the same player
		if(collision.tag == this.tag) { return; }

		//Add camera shake
		FindObjectOfType<CameraController>().StartCameraShake();

		if (collision.tag != "Player") {
			//Play wall hit sound
			FindObjectOfType<SoundManager>().PlayWallHitSound();
		}

		//Create particle effects and destroy object
		GameObject particles = Instantiate(bulletParticles, transform.position, transform.rotation);
		Destroy(particles, 2f);
		Destroy(this.gameObject);
	}
}
