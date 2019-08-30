using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[SerializeField] float playerSpeed = 12;
	[SerializeField] float wallJumpSpeed ;
	[SerializeField] float jumpSpeed = 15;
	[SerializeField] Vector2 movementVector;

	[SerializeField] bool isCrouching = false;

	private Rigidbody2D rb;
	private int groundLayerMask;


	// Start is called before the first frame update
	void Start() {
		rb = this.GetComponent<Rigidbody2D>();

		//Get layermask so ray does not intersect with player
		int groundLayer = 9;
		groundLayerMask = 1 << groundLayer;
	}

	// Update is called once per frame
	void Update() {
		//Find what direction the movement is taking place based on input
		movementVector = new Vector2(Input.GetAxis("Horizontal"), 0f);

		//Check for jumps
		Jump();

		//Check for wall jumps
		//WallJump();
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
			rb.velocity += (new Vector2(0f, 1f) * jumpSpeed);
		}
	}

	private void WallJump() {
		//Check if the jump button's been pressed and if the player is grounded 
		if (Input.GetButtonDown("Jump") && !CheckGrounded()) {
			//Check what wall we are sliding on
			//-1 indicates a right wall, 0 indicates no wall, 1 indicates a left wall
			int wallPosition = FindWallSlide();
			if (wallPosition == 0) { return; }

			Debug.Log(wallPosition);
			rb.velocity += (new Vector2(1f * wallPosition, 1f) * wallJumpSpeed);
		}
	}

	private bool CheckGrounded() {
		//Distance between player pivot and the bottom of the player
		float distFromGround = GetComponent<BoxCollider2D>().bounds.extents.y + 0.1f;   //Needs a little offset so the ray actually hits ground

		//Cast ray and check if we found an object
		if (Physics2D.Raycast(transform.position, Vector2.down, distFromGround, groundLayerMask).collider != null) {
			return true;
		}

		//Ray didn't intersect so the player is not grounded
		return false;
	}

	private int FindWallSlide() {
		//Distance between player pivot and the side of the player
		float distFromWall = GetComponent<BoxCollider2D>().bounds.extents.x + 0.1f;   //Needs a little offset so the ray actually hits walls

		//Cast rays
		//Check left wall 
		if (Physics2D.Raycast(transform.position, Vector2.right, distFromWall, groundLayerMask).collider != null) {
			return -1;
		}
		//Check right wall
		else if (Physics2D.Raycast(transform.position, Vector2.left, distFromWall, groundLayerMask).collider != null) {
			return 1;
		}

		//No walls found
		else { return 0; }
	}
}
