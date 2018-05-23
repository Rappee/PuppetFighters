using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float maxSpeed;
	public float jumpForce;
	public float airSpeed;
	public float wallJumpSpeedX;
	public float wallJumpSpeedY;
	public float wallSlideFriction;
	public int wallStickFrames;
	//public int wallDepartDelay; // wait ... frames before departing for wall (for more comfertable walljumps)

	private bool isGrounded;
	private bool canDoubleJump;
	private float horizontal;

	private bool isOnWall;
	private bool slidingDownWall;
	private int wallStickFramesCounter;
	private int wallDepartDelayCounter;

	private Rigidbody2D rb;
	private Collider2D collider;
	public GameObject cloudsPuff;
	private ParticleSystem landingParticles;

	void Start () {
		collider = GetComponentInChildren<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();

		wallStickFramesCounter = wallStickFrames;
	}
	
	// Update is called once per frame
	void Update () {
		horizontal = Input.GetAxis ("Horizontal");

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (isGrounded) {
				Jump ();
			} else { 
				if (!isOnWall && canDoubleJump) {
					Jump ();
					canDoubleJump = false;
				} else if (isOnWall) {
					wallStickFramesCounter = 0;
					WallJump (isOnWallLeft ());
				}
			}
		}
	}

	void FixedUpdate() {

		//print (string.Format ("isOnWall: {0}", isOnWall));
		//print(string.Format("wallstickcounter {0}", wallStickFramesCounter));

		// horizontal momemevent
		if (isGrounded) {
			rb.velocity = new Vector2 (horizontal * maxSpeed, rb.velocity.y);
		} else {
			if (!isOnWall) {
				rb.velocity = new Vector2 (horizontal * airSpeed, rb.velocity.y);
			} else { // on wall
				//stick to the wall for wallStickFrames frames

				if (wallStickFramesCounter > 0) {
					print ("sticking to wall");
					rb.velocity = new Vector2 (0, 0);
					wallStickFramesCounter--;
				} 
				else
				{
					if (Mathf.Abs (horizontal) < 0.5f) {
						//print ("sliding");
						rb.AddForce (Vector2.up * wallSlideFriction);
					} else {
						if ((isOnWallRight () && horizontal < 0) || (isOnWallLeft() && horizontal > 0)) {
							print ("moving away from wall");
							rb.velocity = new Vector2 (horizontal * airSpeed, rb.velocity.y);
						}
					}
				}


			}
		}

	}

	void Jump() {
		rb.velocity = new Vector2 (rb.velocity.x, jumpForce);
	}

	void WallJump(bool wallonleft) {
		if (wallonleft) {
			if (horizontal < 0.01f) {
				print ("walljump straight up");
				rb.velocity = new Vector2 (wallJumpSpeedX/2, wallJumpSpeedY);
			} else {
				print ("walljump to the right");
				rb.velocity = new Vector2 (wallJumpSpeedX, wallJumpSpeedY);
			}
		} else {
			if (horizontal > -0.01f) {
				print ("walljump straight up");
				rb.velocity = new Vector2 (-wallJumpSpeedX/2, wallJumpSpeedY);
			} else {
				print ("walljump to the left");
				rb.velocity = new Vector2 (-wallJumpSpeedX, wallJumpSpeedY);
			}
		}
	}

	void MakeLandingParticles() {
		Vector3 down = new Vector3 (0, -this.collider.bounds.extents.y, 0);
		GameObject clouds = Instantiate (cloudsPuff, this.transform.position + down, this.transform.rotation) as GameObject;
		landingParticles = clouds.GetComponent<ParticleSystem> ();
		landingParticles.Play ();
		Destroy (clouds, landingParticles.duration);
	}
		

	// collisions

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag.Equals ("Ground")) {
			isGrounded = true;
			canDoubleJump = true;
			MakeLandingParticles ();
		} else if (col.gameObject.tag.Equals ("Wall")) {
			isOnWall = true;
		}
	}

	void OnCollisionExit2D(Collision2D col) {
		if (col.gameObject.tag.Equals ("Ground")) {
			isGrounded = false;
		} else if (col.gameObject.tag.Equals ("Wall")) {
			isOnWall = false;
			wallStickFramesCounter = wallStickFrames;
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
