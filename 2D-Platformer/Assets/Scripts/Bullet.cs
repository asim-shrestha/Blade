using System.Collections;
using System.Collections.Generic;
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
    }

	// Update is called once per frame
	void FixedUpdate() {
		rb.velocity = new Vector2(bulletSpeed * transform.localScale.x, 0);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		//Create effects and destroy object
		Instantiate(bulletParticles, transform.position, transform.rotation);
		Destroy(this.gameObject);
	}
}
