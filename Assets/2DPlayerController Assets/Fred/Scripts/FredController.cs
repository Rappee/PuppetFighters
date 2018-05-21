using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FredController : MonoBehaviour {

	public float speed;
	public float jumpVelocity;
	public float transitionVelocity;
	public LayerMask groundLayer;

	private bool canDoubleJump = true;
	private bool facingRight = true;
	private bool jumped = false;

	private Rigidbody2D rb2d;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private BoxCollider2D boxCollider;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Space) && (IsGrounded() || canDoubleJump)) {
			Jump ();
		}

		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.RightArrow)) {
			animator.SetBool ("IsMoving", true);
		}
		else if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.RightArrow)) {
			animator.SetBool ("IsMoving", false);
		}
	}

	void FixedUpdate() 
	{
		bool onGround = IsGrounded ();
		print (onGround);

		if (onGround == true) {
			canDoubleJump = true;
			if (jumped == true) {
				animator.SetTrigger ("LandingTrigger");
				jumped = false;
			}
		}

		float inputX = Input.GetAxis ("Horizontal");

		animator.SetFloat ("Speed", Mathf.Abs (inputX));
		animator.SetFloat ("vSpeed", rb2d.velocity.y);

		if (Mathf.Abs (inputX) > 0.1f) {
			rb2d.velocity = new Vector2 (inputX * this.speed, rb2d.velocity.y);

			// flip player if needed
			if (inputX > 0 && !facingRight) {
				Flip ();
			} else if (inputX < 0 && facingRight) {
				Flip ();
			}
		}

		if (rb2d.velocity.y > 0 && rb2d.velocity.y < transitionVelocity) {
			animator.SetTrigger ("TransitionTrigger");
		}


	}

	private void Jump() {
		rb2d.velocity = new Vector2 (rb2d.velocity.x, this.jumpVelocity);
		canDoubleJump = false;
		animator.SetTrigger ("JumpTrigger");
		jumped = true;
	}

	private bool IsGrounded() {
		int layerMask = 1 << 8; // 8 is ground layer
		RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.down, boxCollider.bounds.extents.y + 0.1f, layerMask);
		if (hit.collider != null) {
			return true;
		}
		return false;
	}

	private void Flip() {
		facingRight = !facingRight;

		Vector2 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
}
