using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour {
  public float speed = 6f;
  public float gravity = -9.8f;

  private float turnSmoothVelocity;
  public float turnSmoothTime = 3.0f;

  public CharacterController cc;

  void Start() {
    // if (!isLocalPlayer) {
    //   return;
    // }
    GameObject.FindGameObjectWithTag("Camera").GetComponent<FindPlayer>().SendMessage("Find");

    Color random = new Color (Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    GetComponent<Renderer>().material.color = random;
  }

  void Update() {
    if (!isLocalPlayer) {
      return;
    }

    float deltaX = Input.GetAxis("Horizontal")*speed;
    float deltaZ = Input.GetAxis("Vertical")*speed;
    Vector3 movement = new Vector3(deltaX, 0, deltaZ);
    movement = Vector3.ClampMagnitude(movement, speed);

    movement.y = gravity;
    movement *= Time.deltaTime;
    movement = transform.TransformDirection(movement);

    Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    inputDir = inputDir.normalized;
    float targetRot = Mathf.Atan2(inputDir.x, inputDir.y)*Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
    transform.eulerAngles = Vector3.up*Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, turnSmoothTime);

    cc.Move(movement);
  }
}
