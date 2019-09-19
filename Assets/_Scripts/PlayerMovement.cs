using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour {
  public float speed = 6f;
  public float gravity = -9.8f;
	public float jumpSpeed = 15.0f;
  private float vertSpeed;
	public float terminalVelocity = -20.0f;
	public float minFall = -1.5f;
	public float pushForce = 3.0f;
	private ControllerColliderHit contact;

  private float turnSmoothVelocity;
  public float turnSmoothTime = 3.0f;

  [SyncVar(hook = nameof(ChangeColor))] public Color random = Color.black;

  public CharacterController cc;

  public override void OnStartServer() {
    base.OnStartServer();
    random = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
  }

  void Start() {
    if (!isLocalPlayer) {
      return;
    }

    vertSpeed = minFall;

    GameObject.FindGameObjectWithTag("Camera").GetComponent<FindPlayer>().SendMessage("Find", this.gameObject);
  }

  void ChangeColor(Color col) {
      GetComponent<Renderer>().material.color = col;
  }

  void Update() {
    if (!isLocalPlayer) {
      return;
    }

    float deltaX = Input.GetAxis("Horizontal")*speed;
    float deltaZ = Input.GetAxis("Vertical")*speed;
    Vector3 movement = new Vector3(deltaX, 0, deltaZ);
    movement = Vector3.ClampMagnitude(movement, speed);

    //movement.y = gravity;
    //movement *= Time.deltaTime;
    movement = transform.TransformDirection(movement);

    Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    inputDir = inputDir.normalized;
    float targetRot = Mathf.Atan2(inputDir.x, inputDir.y)*Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
    transform.eulerAngles = Vector3.up*Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, turnSmoothTime);

    bool hitGround = false;
    RaycastHit hit;
    if (vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit)) {
      float check = (cc.height + cc.radius) / 1.9f;
      hitGround = hit.distance <= check;	// to be sure check slightly beyond bottom of capsule
    }

    if (hitGround) {
			if (Input.GetButtonDown("Jump")) {
				vertSpeed = jumpSpeed;
			} else {
				vertSpeed = minFall;
				// _animator.SetBool("Jumping", false);
			}
		} else {
			vertSpeed += gravity * 5 * Time.deltaTime;
			if (vertSpeed < terminalVelocity) {
				vertSpeed = terminalVelocity;
			}
			   // if (contact != null ) {	// not right at level start
		     // 	_animator.SetBool("Jumping", true);
		     // }
		// 	if (cc.isGrounded) {
		// 		if (Vector3.Dot(movement, contact.normal) < 0) {
		// 			movement = contact.normal * speed;
		// 		} else {
		// 			movement += contact.normal * speed;
		// 		}
		// }
		}
		movement.y = vertSpeed;
    movement *= Time.deltaTime;

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
