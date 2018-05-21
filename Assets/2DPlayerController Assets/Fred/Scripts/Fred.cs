using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fred : MonoBehaviour {

	public float moveSpeed; // when grounded
	public float moveForce; // when airbourne
	public float jumpForce;
	public float wallJumpForceX;
	public float wallJumpForceY;
	public float wallSlideFriction;
	public float wallSlideFrictionExtra;

	private Rigidbody2D rb;
	//private SpriteRenderer spriteRenderer;
	private Collider2D collider;
	private Animator animator;

	private bool facingRight = true;
	private float dirX;
	private bool isOnWall;
	private bool isGrounded;
	private bool canDoubleJump;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		//spriteRenderer = GetComponent<SpriteRenderer> ();
		collider = GetComponent<Collider2D>();
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		//dirX = Input.GetAxis ("Horizontal") * moveSpeed;
		dirX = Input.GetAxisRaw("Horizontal");

		if (isGrounded) {
			// moved to oncollision
		}
	}

	void FixedUpdate() {
		//rb.velocity = new Vector2 (dirX, rb.velocity.y);
		if (isGrounded) {
			rb.velocity = new Vector2 (dirX * moveSpeed, rb.velocity.y);
		} else {
			rb.AddForce (Vector2.right * dirX * moveForce);
		}

		//print (string.Format("Left: {0}", isOnWallLeft ()));
		//print (string.Format("Right: {0}", isOnWallRight ()));

		// flip player if needed
		if (dirX > 0 && !facingRight) {
			Flip ();
		} else if (dirX < 0 && facingRight) {
			Flip ();
		}

		animator.SetFloat("hSpeed", Mathf.Abs(rb.velocity.x));
		animator.SetFloat("vSpeed", rb.velocity.y);
		animator.SetFloat("Moving", Mathf.Abs (dirX));

		if (Input.GetButtonDown ("Jump")) {
			if (isOnWall && !isGrounded) {
				if (isOnWallLeft ()) {
					WallJump (true);
				} else {
					WallJump (false);
				}
			}
			else if (isGrounded) {
				print ("Regular jump");
				Jump ();
				canDoubleJump = true;
			} else {
				if (canDoubleJump) {
					print ("Double jump");
					Jump ();
					canDoubleJump = false;
				}
			}
		}

		// slide down wall
		if(isOnWall) {
			print ("onwall");
			if (Input.GetKey (KeyCode.LeftArrow)) {
				rb.AddForce (Vector2.up * wallSlideFrictionExtra);
			} else {
				rb.AddForce (Vector2.up * wallSlideFriction);
			}
		}
	}



	// actions

	private void Jump() {
		rb.velocity = new Vector2 (rb.velocity.x, jumpForce);
		animator.SetTrigger ("JumpTrigger");
	}

	private void WallJump(bool wallOnLeft) {
		if (wallOnLeft) {
			print ("Wall jump from left wall");
			rb.velocity = new Vector2 (wallJumpForceX, wallJumpForceY);
		} else {
			print ("Wall jump from right wall");
			rb.velocity = new Vector2 (-wallJumpForceX, wallJumpForceY);
		}
		animator.SetTrigger("JumpTrigger");
		//Flip ();
	}

	private void Flip() {
		facingRight = !facingRight;

		Vector2 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	// collisions

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag.Equals ("Ground")) {
			isGrounded = true;
			canDoubleJump = true;
			animator.SetBool ("Grounded", true);
		} else if (col.gameObject.tag.Equals ("Wall")) {
			isOnWall = true;
			animator.SetBool ("OnWall", true);
		}
	}

	void OnCollisionExit2D(Collision2D col) {
		if (col.gameObject.tag.Equals ("Ground")) {
			isGrounded = false;
			animator.SetBool ("Grounded", false);
		} else if (col.gameObject.tag.Equals ("Wall")) {
			isOnWall = false;
			animator.SetBool ("OnWall", false);
		}
	}

	private bool isOnWallRight() {
		float distance = this.collider.bounds.extents.x + 0.1f;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.right, distance, LayerMask.GetMask("Walls"));
		return hit;
	}

	private bool isOnWallLeft() {
		float distance = this.collider.bounds.extents.x + 0.1f;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.left, distance, LayerMask.GetMask("Walls"));
		return hit;
	}
}
