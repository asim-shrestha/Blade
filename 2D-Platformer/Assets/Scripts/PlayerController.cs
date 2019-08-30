using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] float playerSpeed;
	[SerializeField] float minJumpSpeed;
	[SerializeField] float maxJumpSpeed;
	[SerializeField] Vector2 movementVector;
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
		//Check if the jump button's been pressed and if the player is grounded 
		if (Input.GetButtonDown("Jump") && CheckGrounded()) {
			rigidbody.velocity += (new Vector2(0f, 1f) * maxJumpSpeed);
		}
	}

	private bool CheckGrounded() {
		//Distance between object pivot and the bottom of the object
		float distFromGround = GetComponent<BoxCollider2D>().bounds.extents.y + 0.1f;	//Needs a little offset so the ray actually hits ground

		//Get layermask so ray does not intersect with player
		int groundLayer = 9;
		int layerMask = 1 << groundLayer;

		//Cast ray and check if we found an object
		if (Physics2D.Raycast(transform.position, Vector2.down, distFromGround, layerMask).collider != null)
		{
			return true;
		}

		//Ray didn't intersect so the player is not grounded
		return false;
	}
}
