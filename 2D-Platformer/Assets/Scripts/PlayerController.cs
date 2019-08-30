using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Player configuration")]
	[SerializeField] float playerSpeed = 12;
	[SerializeField] float jumpSpeed = 15;
	[SerializeField] float wallJumpSpeed = 5;
	[SerializeField] float wallSlideSpeed = 5;
	[SerializeField] float variableJumpHeightMultiplier = 0.5f;
	[SerializeField] int maxJumps = 2;

	[Header("Player States")]
	[SerializeField] int jumpCount = 0;
	[SerializeField] float movementDirection;
	[SerializeField] bool isGrounded = true;
	[SerializeField] bool isWallSliding = false;
	[SerializeField] bool isCrouching = false;

	private Rigidbody2D rb;
	private int groundLayerMask;


	// Start is called before the first frame update
	void Start() {
		//Get rigid body component
		rb = this.GetComponent<Rigidbody2D>();

		//Get layermask so ray casts do not intersect with the player layer
		int groundLayer = 9;
		groundLayerMask = 1 << groundLayer;
	}

	// Update is called once per frame
	void Update() {
		//Find what direction the movement is taking place based on input
		movementDirection = Input.GetAxis("Horizontal");

		//Check if the player is grounded
		CheckGrounded();

		//Check if the player is sliding on a wall
		CheckWallSlide();

		//Check for jumps
		HandleJump();

		//Check for wall jumps
		//WallJump();
	}

	//Runs every physics step
	void FixedUpdate() {
		//Move our character
		MovePlayer();
	}

	private void MovePlayer() {
		//Horizontal movement
		rb.velocity = new Vector2(movementDirection * playerSpeed, rb.velocity.y);

		//Make sure the player isn't falling faster than the wall slide speed if he is wall sliding
		if (isWallSliding && rb.velocity.y < -wallSlideSpeed) { rb.velocity = new Vector2 (rb.velocity.x, -wallSlideSpeed); }
	}

	private void HandleJump() {
		//Check if the jump button's been pressed and if the player is grounded
		//Also check to see if the player has reached the max jump count
		if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount != maxJumps)) {
			jumpCount++;
			rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
		}

		//Wall jumping
		else if (Input.GetButtonDown("Jump") && isWallSliding) {
			Vector2 wallJumpDirection = new Vector2(1f, 1f).normalized;
			rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
		}

		//Early jump release for variable jump height
		if (Input.GetButtonUp("Jump")) {
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
		}
	}

	private void WallJump() {
		//Check if the jump button's been pressed and if the player is grounded 
		if (Input.GetButtonDown("Jump") && !isGrounded) {
			//Check what wall we are sliding on
			//-1 indicates a right wall, 0 indicates no wall, 1 indicates a left wall
			int wallPosition = CheckWallSlide();
			if (wallPosition == 0) { return; }

			Debug.Log(wallPosition);
			rb.velocity += (new Vector2(1f * wallPosition, 1f) * wallJumpSpeed);
		}
	}

	//Use raycasting to figure out if the player is standing on the ground
	private void CheckGrounded() {
		//Distance between player pivot and the bottom of the player
		float distFromGround = GetComponent<BoxCollider2D>().bounds.extents.y + 0.1f;   //Needs a little offset so the ray actually hits ground

		//Cast ray and check if we found an object
		if (Physics2D.Raycast(transform.position, Vector2.down, distFromGround, groundLayerMask).collider != null) {
			isGrounded = true;
			jumpCount = 0;
			return;
		}

		//Ray didn't intersect so the player is not grounded
		isGrounded = false;
		//Make sure the player has 1 less jump avaliable
		if(jumpCount == 0) { jumpCount++; }
		return;
	}

	private int CheckWallSlide() {
		//Make sure player is off the ground and is falling in the air
		if(isGrounded == true || rb.velocity.y > 0) {
			isWallSliding = false;
			return 0;
		}

		//Distance between player pivot and the side of the player
		float distFromWall = GetComponent<BoxCollider2D>().bounds.extents.x + 0.1f;   //Needs a little offset so the ray actually hits walls
		//Cast rays and make sure the proper button is being held
		//Check right wall 
		if (Physics2D.Raycast(transform.position, Vector2.right, distFromWall, groundLayerMask).collider != null && Input.GetAxis("Horizontal") > 0) {
			isWallSliding = true;
			return -1;
		}
		//Check left wall
		else if (Physics2D.Raycast(transform.position, Vector2.left, distFromWall, groundLayerMask).collider != null && Input.GetAxis("Horizontal") < 0) {
			isWallSliding = true;
			return 1;
		}

		//No walls found
		else {
			isWallSliding = false;
			return 0;
		}
	}
}
