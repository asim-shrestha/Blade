using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Controller configuration")]
	[SerializeField] string playerNumber = "";

	[Header("Player configuration")]
	[SerializeField] float movementSpeed = 12;
	[SerializeField] float jumpSpeed = 15;
	[SerializeField] float variableJumpHeightMultiplier = 0.5f;
	[SerializeField] float jumpMovementForce = 15;
	[SerializeField] [Range(0, 1)] float airDragMultiplier = 1;
	[SerializeField] float wallSlideSpeed = 5;
	[SerializeField] float wallJumpForce = 15;
	[SerializeField] Vector2 wallJumpDirection = new Vector2 (1f,1f);
	[SerializeField] [Range(0,1)] float movementLockTime = 1f;
	[SerializeField] int maxBullets = 3;
	[SerializeField] int maxJumps = 2;
	[SerializeField] int maxWarps = 1;
	[SerializeField] float warpDistance = 5;

	[Header("Weapons")]
	[SerializeField] GameObject bullet;

	[Header("Player States")]
	[SerializeField] int health = 1;
	[SerializeField] int direction = 1;
	[SerializeField] int bulletCount = 0;
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
		CheckWallSlide();

		//Check for jumps
		HandleJump();

		//Check for warps
		HandleWarp();

		//Check for bullet firing
		HandleFire();
	}

	void CheckDirection() {
		if(movementDirection > 0) { direction = 1; }
		else if (movementDirection < 0) { direction = -1; }
	}

	//Runs every physics step
	void FixedUpdate() {
		//Move our character
		MovePlayer();
	}

	private void MovePlayer() {
		//Make sure the player isn't falling faster than wallSlideSpeed if he is wall sliding
		if (isWallSliding && rb.velocity.y < -wallSlideSpeed) { rb.velocity = new Vector2 (rb.velocity.x, -wallSlideSpeed); }

		//Air drag
		if(!isGrounded && !isWallSliding && movementDirection == 0) {
			if(rb.velocity.x != 0) {
				rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
			}
		}

		//Horizontal ground movement
		if (isGrounded) {
			rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
		}

		//Air movement
		if (!isGrounded && !isWallSliding && movementDirection != 0) {
			Vector2 forceVector = new Vector2(jumpMovementForce * movementDirection ,0f);
			rb.AddForce(forceVector);
			if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(movementSpeed * movementDirection)) {
				rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
			}
		}
	}

	private void HandleJump() {
		//Check if the jump button's been pressed and if the player is grounded
		//Also check to see if the player has reached the max jump count
		if (Input.GetButtonDown("Jump" + playerNumber) && (isGrounded || jumpCount != maxJumps) && !isWallSliding) {
			isVariableJumpEnabled = true;
			jumpCount++;
			Jump();
		}

		//Wall jumping
		else if (Input.GetButtonDown("Jump" + playerNumber) && isWallSliding) {
			isVariableJumpEnabled = false;
			Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * CheckWallSlide(), wallJumpForce * wallJumpDirection.y);
			rb.AddForce(forceToAdd, ForceMode2D.Impulse);
			MovementLock(direction);
		}

		//Early jump release for variable jump height
		if (Input.GetButtonUp("Jump" + playerNumber) && rb.velocity.y > 0f && isVariableJumpEnabled) {
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
		}
	}

	private void Jump() {
		rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
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
		//Make sure player is off the ground and is falling in the air
		if (isGrounded == true){//|| rb.velocity.y > 0) {
			isWallSliding = false;
			return 0;
		}

		//Distance between player pivot and the side of the player
		float distFromWall = GetComponent<BoxCollider2D>().bounds.extents.x + 0.2f;   //Needs a little offset so the ray actually hits walls
		//Cast rays and make sure the proper button is being held
		//Check right wall 
		if (Physics2D.Raycast(transform.position, Vector2.right, distFromWall, groundLayerMask).collider != null && direction > 0) {
			isWallSliding = true;
			GetComponent<SpriteRenderer>().color = Color.blue;
			return -1;
		}
		//Check left wall
		else if (Physics2D.Raycast(transform.position, Vector2.left, distFromWall, groundLayerMask).collider != null && direction < 0) {
			isWallSliding = true;
			GetComponent<SpriteRenderer>().color = Color.blue;
			return 1;
		}

		//No walls found
		else {
			GetComponent<SpriteRenderer>().color = Color.white;
			isWallSliding = false;
			return 0;
		}
	}

	private void HandleWarp() {
		//Make sure there are warps avaliable
		if(warpCount == maxWarps) { return; }

		//Make sure warp button has been pressed
		if (Input.GetButtonDown("Fire1" + playerNumber)) {
			//Make sure player is moving left or right
			if(direction== 0) { return; }

			//Find where the player would warp through ray cast
			//Distance between player pivot and end warp point
			float rayDistance = (GetComponent<BoxCollider2D>().bounds.extents.x * 2) + warpDistance + 0.1f;   //Needs a little offset so the ray may hit walls

			//Ray cast
			if (Physics2D.Raycast(transform.position, Vector2.right * direction, rayDistance, groundLayerMask).collider == null) {
				warpCount++;
				transform.position = new Vector2(transform.position.x + (warpDistance * direction) , transform.position.y);
			}

		}
	}

	private void HandleFire() {
		if (Input.GetButtonDown("Fire2" + playerNumber)) {
			//Check if there are bullets left
			if(bulletCount >= maxBullets) { return; }

			//Calculate where the bullet will be fired
			Vector3 bulletPosition = new Vector3(transform.position.x + (GetComponent<BoxCollider2D>().bounds.extents.x*2 + 0.1f ) * direction, transform.position.y);

			//Fire bullet and set direction
			bulletCount++;
			GameObject bulletClone = Instantiate(bullet, bulletPosition, transform.rotation);
			bulletClone.transform.localScale = new Vector3(direction, 1, 1);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		health--;
		if (health <= 0) {
			Destroy(this.gameObject);
		}
		else {
			GetComponent<SpriteRenderer>().color = Color.white;
			StartCoroutine(HitAnimation());

		}
	}

	IEnumerator HitAnimation() {
		GetComponent<SpriteRenderer>().color = Color.red;
		yield return new WaitForSeconds(.1f);
		GetComponent<SpriteRenderer>().color = Color.white;
	}

	private void MovementLock(int disabledDirection) {
		movementLockDirection = disabledDirection;
		movementLockCounter = movementLockTime;
	}

}
