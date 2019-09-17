using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class FindPlayer : NetworkBehaviour {
  public CinemachineFreeLook camera;
  private bool playerFound = false;

  void Start() {
    camera = GetComponent<CinemachineFreeLook>();
  }

  void Update() {
    if (!isLocalPlayer) {
      return;
    }
  }

  public void Find() {
    if (!playerFound) {
      camera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
      camera.LookAt = GameObject.FindGameObjectWithTag("Player").transform;
      playerFound = true;
    }
  }
}
