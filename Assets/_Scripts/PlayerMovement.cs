using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour {
  public float speed = 6f;
  public float gravity = -9.8f;

  public CharacterController cc;

  void Start() {
    GameObject.FindGameObjectWithTag("Camera").GetComponent<FindPlayer>().SendMessage("Find");
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

    cc.Move(movement);
  }
}