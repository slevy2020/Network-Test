using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour {

  void Update() {
    if (!isLocalPlayer) {
      return;
    }

    //code to move
  }
}
