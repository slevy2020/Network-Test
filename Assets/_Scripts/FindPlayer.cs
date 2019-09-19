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

  public void Find(GameObject playerObject) {
    if (!playerFound) {
      camera.Follow = playerObject.transform;
      camera.LookAt = playerObject.transform;
      playerFound = true;
    }
  }
}
