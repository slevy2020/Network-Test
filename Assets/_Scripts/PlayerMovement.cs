using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour {
  private Transform target; //for camera rotation

  //vars for player movement
  public float speed;
  public float walkSpeed = 3f;
  public float runSpeed = 6f;
  public float rotSpeed = 15.0f;
  public float gravity = -9.8f;
	public float jumpSpeed = 15.0f;
  private float vertSpeed;
	public float terminalVelocity = -20.0f;
	public float minFall = -1.5f;
	public float pushForce = 3.0f;
	private ControllerColliderHit contact;

  private Animator animator; //animator

  [SyncVar(hook = nameof(ChangeColor))] public Color random = Color.black; //sync var for random color changing (currently broken!)

  public CharacterController cc; //player's character controller

  //on player created, assign random new color
  public override void OnStartServer() {
    base.OnStartServer();
    cc.enabled = true;
    animator = GetComponent<Animator>();
    random = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
  }

  // public override void OnStartLocalPlayer() {
  //   base.OnStartLocalPlayer();
  // }

  void Start() {
    //client, only apply to local player
    if (!isLocalPlayer) {
      return;
    }

    //assign vars
    vertSpeed = minFall;

    GameObject.FindGameObjectWithTag("Camera").GetComponent<FindPlayer>().SendMessage("Find", this.gameObject);

    target = Camera.main.transform;
  }

  //method to change player color
  void ChangeColor(Color col) {
      GetComponent<Renderer>().material.color = col;
  }

  void Update() {
    //client, only apply to local player
    if (!isLocalPlayer) {
      return;
    }

    //create vector3, assign horizontal and vertical player input
    Vector3 movement = Vector3.zero;
    float horInput = Input.GetAxis("Horizontal");
		float vertInput = Input.GetAxis("Vertical");
    //if the player is inputting a direction
		if (horInput != 0 || vertInput != 0) {
      //check if player is running
      if (Input.GetKey(KeyCode.LeftShift)) {
        speed = runSpeed;
        animator.SetBool("Running", true);
      } else {
        speed = walkSpeed;
        animator.SetBool("Running", false);
      }

      //apply input to the movement
      movement.x = horInput * speed;
      movement.z = vertInput * speed;
      movement = Vector3.ClampMagnitude(movement, speed);

      //determine which direction to head based on where camera is pointing
      Quaternion tmp = target.rotation;
      target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
      movement = target.TransformDirection(movement);
      target.rotation = tmp;

      //actually rotate player
      Quaternion direction = Quaternion.LookRotation(movement);
      transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
    }

    //check if player is touching the ground
    bool hitGround = false;
    RaycastHit hit;
    if (vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit)) {
      float check = (cc.height + cc.radius) / 1.9f;
      hitGround = hit.distance <= check;	// to be sure check slightly beyond bottom of capsule
    }

    animator.SetFloat("Speed", movement.sqrMagnitude); //control speed float on the animator

    //if touching the ground, the player can jump
    if (hitGround) {
			if (Input.GetButtonDown("Jump")) {
				vertSpeed = jumpSpeed;
			} else {
				vertSpeed = minFall;
				animator.SetBool("Jumping", false);
			}
		} else { //if player is not on the ground, fall
			vertSpeed += gravity * 5 * Time.deltaTime;
			if (vertSpeed < terminalVelocity) {
				vertSpeed = terminalVelocity;
			}
  	  if (contact != null ) {	// not right at level start
     	  animator.SetBool("Jumping", true);
      }
			if (cc.isGrounded) {
				if (Vector3.Dot(movement, contact.normal) < 0) {
					movement = contact.normal * speed;
				} else {
					movement += contact.normal * speed;
				}
  		}
		}
		movement.y = vertSpeed; //assign vertical speed (positive if jumping, negative if falling) to y value of movement
    movement *= Time.deltaTime;

    //move
    cc.Move(movement);
  }

  // store collision to use in Update
	void OnControllerColliderHit(ControllerColliderHit hit) {
		contact = hit;

		Rigidbody body = hit.collider.attachedRigidbody;
		if (body != null && !body.isKinematic) {
			body.velocity = hit.moveDirection * pushForce;
		}
	}
}
