using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class PlayerController : MonoBehaviour {

	[SerializeField] private float movementSpeed;
	[SerializeField] private float jumpHeight;
	[SerializeField] private float accel = 14f;

	[SerializeField] private LayerMask layerMask;


	// Player Components
	private Rigidbody2D playerRb;
	private SpriteRenderer sr;
	private Animator animator;
	private BoxCollider2D boxCollider;
	//

	private Vector2 input;
	private Vector2 velocity;

	private bool isJumping = false;

	private float jumpDuration;
	private float jumpDurationThreshold = 0.25f;


	private float rayCastLength = 0.5f;

	private float halfColliderWidth;
	private float halfColliderHeight;

	int Scores;
	public Text Score;
	public Text WinText;

	void Start () {
		WinText.text = " ";
		playerRb = GetComponent<Rigidbody2D> ();
		sr = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();

		halfColliderWidth = boxCollider.bounds.extents.x;
		halfColliderHeight = boxCollider.bounds.extents.y + 0.1f;
	}


	void Update () {
		input.x = Input.GetAxis ("Horizontal");
		input.y = Input.GetAxis ("Jump");
	
		if(this.gameObject.transform.position.y <= -11)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
		}

		if (input.x > 0f) {
			sr.flipX = false;
		} else if (input.x < 0f) {
			sr.flipX = true;
		}
	}

	void FixedUpdate() {

		var acceleration = accel;

		var xVelocity = 0f;
		if (input.x == 0) {
			xVelocity = 0;
		} else {
			xVelocity = playerRb.velocity.x;
		}


		var yVelocity = 0f;
		if (IsPlayerOnGround() && input.y > 0) {
			yVelocity = GetJumpVelocity ();
		} else {
			yVelocity = playerRb.velocity.y;
		}


		playerRb.AddForce (new Vector2(((input.x * movementSpeed) - playerRb.velocity.x) * acceleration, 0f));


		playerRb.velocity = new Vector2(xVelocity, yVelocity);

		animator.SetFloat ("Speed", Mathf.Abs(playerRb.velocity.normalized.x));

		animator.SetBool ("IsOnGround", IsPlayerOnGround ());
	}


	float GetJumpVelocity() {
		return Mathf.Sqrt (2 * Physics.gravity.magnitude * jumpHeight);
	}


	private bool IsPlayerOnGround() {

		bool centerCheck = Physics2D.Raycast (transform.position + (Vector3.down * halfColliderHeight) , -Vector2.up, rayCastLength, layerMask);
		bool leftCheck = Physics2D.Raycast (transform.position + (Vector3.down * halfColliderHeight) + (Vector3.left * halfColliderWidth), -Vector2.up, rayCastLength, layerMask);
		bool rightCheck = Physics2D.Raycast (transform.position + (Vector3.down * halfColliderHeight) + (Vector3.right * halfColliderWidth), -Vector2.up, rayCastLength, layerMask);

		if (centerCheck || leftCheck || rightCheck) {
			return true;
		}

		return false;
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "PickUp")
		{
			collision.gameObject.SetActive(false);
			Scores = Scores + 1;
			Score.text = "Score : " + Scores.ToString();
			if (Scores >= 8)
				StartCoroutine(Restart());
		}
	}

	IEnumerator Restart()
	{
		WinText.text = "You Win!";
		yield return new WaitForSeconds(3.0f);
		WinText.text = "Restarting Hold Up...";
		yield return new WaitForSeconds(2.0f);
		UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}
}
