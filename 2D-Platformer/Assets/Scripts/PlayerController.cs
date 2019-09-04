using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Controller configuration")]
	[SerializeField] public string playerNumber = "";

	[Header("Player configuration")]
	[SerializeField] float movementSpeed = 12;

	[SerializeField] int maxJumps = 2;
	[SerializeField] float jumpSpeed = 15;
	[SerializeField] float jumpMovementForce = 15;
	[SerializeField] float variableJumpHeightMultiplier = 0.5f;
	[SerializeField] [Range(0, 1)] float airDragMultiplier = 1;
	[SerializeField] float wallSlideSpeed = 5;
	[SerializeField] float wallJumpForce = 15;
	[SerializeField] Vector2 wallJumpDirection = new Vector2 (1f,1f);
	[SerializeField] [Range(0,1)] float movementLockTime = 1f;
	[SerializeField] int maxWarps = 1;
	[SerializeField] float warpDistance = 5;

	[Header("Player States")]
	[SerializeField] public int facingDirection = 1;
	[SerializeField] int jumpCount = 0;
	[SerializeField] int warpCount = 0;
	[SerializeField] float movementDirection;
	[SerializeField] float movementLockCounter;
	[SerializeField] int movementLockDirection = 0;
	[SerializeField] bool isVariableJumpEnabled = true;
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

		wallJumpDirection.Normalize();
	}

	// Update is called once per frame
	void Update() {
		//Update movement disabled counter
		if(movementLockCounter > 0) {
			movementLockCounter -= Time.deltaTime;
			//While movement is locked in one direction, the player is forced to be moving in the other direction
			movementDirection = -movementLockDirection;
		}

		//Find what direction the movement is taking place based on input
		else {
			movementLockDirection = 0;
			movementDirection = Input.GetAxisRaw("Horizontal" + playerNumber);
		}

		//Check where the player should be facing
		CheckDirection();

		//Check if the player is grounded
		CheckGrounded();

		//Check if the player is sliding on a wall
		if (isGrounded == false) { CheckWallSlide(); }
		else {
			GetComponent<SpriteRenderer>().color = Color.white;
			isWallSliding = false;
		}

		//Check for jumps
		if (Input.GetButtonDown("Jump" + playerNumber)) { HandleJump(); }

		//Early jump release for variable jump height
		if (Input.GetButtonUp("Jump" + playerNumber) && rb.velocity.y > 0f && isVariableJumpEnabled) {
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
		}

		//Check for warps
		if (Input.GetButtonDown("Fire1" + playerNumber)) { Warp(); }
	}

	void CheckDirection() {
		//Facing right
		if(movementDirection > 0) {
			facingDirection = 1;
		}

		//Facing left
		else if (movementDirection < 0) {
			facingDirection = -1;
		}

		//Make player face the right direction
		transform.localScale = new Vector3(facingDirection, 1, 1);
	}

	//Runs every physics step
	void FixedUpdate() {
		//Move our character
		MovePlayer();
	}

	private void MovePlayer() {
		//Horizontal ground movement
		if (isGrounded) {
			rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
		}

		//Air movement
		else if (!isGrounded && !isWallSliding && movementDirection != 0) {
			//Add force based on what direction you are trying to move in
			Vector2 forceVector = new Vector2(jumpMovementForce * movementDirection ,0f);
			rb.AddForce(forceVector);

			//Make sure you aren't moving in the air faster than you would on the ground
			if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(movementSpeed * movementDirection)) {
				rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
			}
		}

		//Make sure the player isn't falling faster than wallSlideSpeed if he is wall sliding
		if (isWallSliding && rb.velocity.y < -wallSlideSpeed) { rb.velocity = new Vector2 (rb.velocity.x, -wallSlideSpeed); }

		//Air drag
		if(!isGrounded && !isWallSliding && movementDirection == 0) {
			if(rb.velocity.x != 0) {
				rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
			}
		}
	}

	private void HandleJump() {
		//Check to see if the player is grounded or if the player still has jumps left
		if ((isGrounded || jumpCount != maxJumps) && !isWallSliding) {
			isVariableJumpEnabled = true;	//Can choose the height of a normal jump
			movementLockDirection = 0;		//Unlock movement for normal jumps
			jumpCount++;
			rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
		}

		//Wall jumping
		else if (isWallSliding) {
			//Disable variable height jump
			isVariableJumpEnabled = false;

			//Find the force to add
			Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * CheckWallSlide(), wallJumpForce * wallJumpDirection.y);
			rb.AddForce(forceToAdd, ForceMode2D.Impulse);

			//Lock moving in the direction of the same wall
			MovementLock(facingDirection);
		}
	}

	//Use raycasting to figure out if the player is standing on the ground
	private void CheckGrounded() {
		//Distance between player pivot and the bottom of the player
		float distFromGround = GetComponent<BoxCollider2D>().bounds.extents.y + 0.1f;   //Needs a little offset so the ray actually hits ground
		Vector2 boxSize = new Vector2(GetComponent<BoxCollider2D>().bounds.extents.x, 0.01f);

		//Cast ray and check if we found an object
		if (Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.down, distFromGround, groundLayerMask).collider != null) {
			isGrounded = true;
			jumpCount = 0;
			warpCount = 0;
			return;
		}

		//Ray didn't intersect so the player is not grounded
		isGrounded = false;
		//Make sure the player has 1 less jump avaliable
		if(jumpCount == 0) { jumpCount++; }
		return;
	}

	private int CheckWallSlide() {
		//Distance between player pivot and the side of the player
		float distFromWall = GetComponent<BoxCollider2D>().bounds.extents.x + 0.1f;   //Needs a little offset so the ray actually hits walls

		//Cast rays in the facing direction and make sure that that direction is being moved to 
		if (Physics2D.Raycast(transform.position, Vector2.right * facingDirection, distFromWall, groundLayerMask).collider != null && movementDirection == facingDirection) {
			isWallSliding = true;
			GetComponent<SpriteRenderer>().color = Color.blue;
			return -facingDirection;
		}

		//No walls found
		GetComponent<SpriteRenderer>().color = Color.white;
		isWallSliding = false;
		return 0;
	}

	private void Warp() {
		//Make sure there are warps avaliable
		if(warpCount == maxWarps) { return; }

		//Distance between player pivot and end warp point
		float rayDistance = (GetComponent<BoxCollider2D>().bounds.extents.x) + warpDistance;

		//Make sure nothing is in the way if we are warping
		if (Physics2D.Raycast(transform.position, Vector2.right * facingDirection, rayDistance, groundLayerMask).collider == null) {
			warpCount++;
			transform.position = new Vector2(transform.position.x + (warpDistance * facingDirection) , transform.position.y);
		}
	}

	private void MovementLock(int disabledDirection) {
		//Locking movement towards a wall that the player wall jumped from
		movementLockDirection = disabledDirection;
		movementLockCounter = movementLockTime;
	}

	public void SetPlayerNumber(string playerNumber) {
		this.playerNumber = playerNumber;
	}

	public string GetPlayerNumber() {
		return playerNumber;
	}

	public int GetDirection() {
		return facingDirection;
	}
}