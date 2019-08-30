using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] float playerSpeed;
	[SerializeField] float jumpSpeed;
	[SerializeField] Vector2 movementVector;
	[SerializeField] bool isGrounded;
	[SerializeField] bool isCrouching;

	public Rigidbody2D rigidbody;


	// Start is called before the first frame update
	void Start()
    {
		rigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
		//Find what direction the movement is taking place based on input
		movementVector = new Vector2(Input.GetAxis("Horizontal"), 0f);
		//CheckGrounded();
		//Check for jumps
		Jump();
    }

	//Runs every physics step
	void FixedUpdate() {
		//Move our character
		MovePlayer(movementVector);
	}

	private void MovePlayer(Vector2 direction) {
		transform.position += (Vector3)direction * playerSpeed * Time.deltaTime;
		//rigidbody.MovePosition((Vector2)transform.position + (direction * playerSpeed * Time.deltaTime));
	}

	private void Jump() {
		if (Input.GetButtonDown("Jump")) {
			CheckGrounded();
			rigidbody.velocity += (new Vector2(0f, 1f) * jumpSpeed);
		}
	}

	private bool CheckGrounded() {
		//Distance between object pivot and the bottom of the object
		float distToGround = GetComponent<BoxCollider2D>().bounds.extents.y;

		//Cast ray
		Debug.Log("test");
		Debug.Log(Physics2D.Raycast((Vector2)transform.position, Vector2.down, distToGround + 5f).collider.name);
		return isGrounded;
	}
}
