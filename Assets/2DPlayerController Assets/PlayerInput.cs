using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputTemp : MonoBehaviour {

	public float maxSpeed = 10;
	public float acceleration = 35;
	public float jumpSpeed = 8;
	public float jumpDuration;

	public bool enableDoubleJump = true;
	public bool wallHitDoubleJumpOverride = true;

	private bool canDoubleJump = true;
	private float jmpDuration;

	private bool jumpKeyDown = false;
	private bool canVariableJump = false;

	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		rb2d = this.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis ("Horizontal");

		if (horizontal < -0.1f) {
			if (this.rb2d.velocity.x > -this.maxSpeed) {
				this.rb2d.AddForce (new Vector2 (-this.acceleration, 0.0f));
			} else {
				this.rb2d.velocity = new Vector2 (-this.maxSpeed, this.rb2d.velocity.y);
			}
		} else if(horizontal > 0.1f) {
			if (this.rb2d.velocity.x < this.maxSpeed) {
				this.rb2d.AddForce (new Vector2 (this.acceleration, 0.0f));
			} else {
				this.rb2d.velocity = new Vector2 (this.maxSpeed, this.rb2d.velocity.y);
			}
		}

		bool onGround = isOnGround ();

		float vertical = Input.GetAxis ("Vertical");

		if (onGround) {
			canDoubleJump = true;
		}

		if (vertical > 0.1f) {
			if (!jumpKeyDown) { // 1st frame (just pressed up button)
				jumpKeyDown = true;

				if (onGround || (canDoubleJump && enableDoubleJump) || wallHitDoubleJumpOverride) {
					bool wallHit = false;
					int wallHitDirection = 0; // -1 jump to left, 1 jump to right

					bool leftWallHit = isOnWallLeft ();
					bool rightWallHit = isOnWallRight ();

					if (horizontal != 0) {
						if (leftWallHit) {
							wallHit = true;
							wallHitDirection = 1;
						} else if (rightWallHit) {
							wallHit = true;
							wallHitDirection = -1;
						}
					}

					if (!wallHit) {
						if (onGround || (canDoubleJump && enableDoubleJump)) {
							this.rb2d.velocity = new Vector2 (this.rb2d.velocity.x, this.jumpSpeed);

							jmpDuration = 0.0f;
							canVariableJump = true;
						} 
					} else {
						this.rb2d.velocity = new Vector2 (this.jumpSpeed * wallHitDirection, this.jumpSpeed);

						jmpDuration = 0.0f;
						canVariableJump = true;
					}

					if (!onGround && !wallHit) {
						canDoubleJump = false;
					}
				}
			} // 2nd frame (for variable jumping)
			else if(canVariableJump) {
				jumpDuration += Time.deltaTime;

				if(jmpDuration < this.jumpDuration / 1000) {
					this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.jumpSpeed);
				}
			}
		} else {
			jumpKeyDown = false;
			canVariableJump = false;
		}
	}

	private bool isOnGround() {
		float lengthToSearch = 0.1f;
		float colliderTreshold = 0.001f;

		Vector2 linestart = new Vector2 (transform.position.x, transform.position.y - this.spriteRenderer.bounds.extents.y - colliderTreshold);
		Vector2 lineend = new Vector2 (transform.position.x, linestart.y - lengthToSearch);

		RaycastHit2D hit = Physics2D.Linecast (linestart, lineend);

		return hit;
	}

	private bool isOnWallLeft() {
		float lengthToSearch = 0.1f;
		float colliderTreshold = 0.01f;

		Vector2 linestart = new Vector2 (transform.position.x - this.spriteRenderer.bounds.extents.x - colliderTreshold, transform.position.y);
		Vector2 lineend = new Vector2 (linestart.x - lengthToSearch, transform.position.y);

		RaycastHit2D hit = Physics2D.Linecast (linestart, lineend);

		return hit;
	}

	private bool isOnWallRight() {
		float lengthToSearch = 0.1f;
		float colliderTreshold = 0.01f;

		Vector2 linestart = new Vector2 (transform.position.x + this.spriteRenderer.bounds.extents.x + colliderTreshold, transform.position.y);
		Vector2 lineend = new Vector2 (linestart.x + lengthToSearch, transform.position.y);

		RaycastHit2D hit = Physics2D.Linecast (linestart, lineend);

		return hit;
	}
}
