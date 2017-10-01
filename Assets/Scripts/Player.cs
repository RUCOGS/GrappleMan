using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (ControllableBox))]
public class Player : MonoBehaviour {

    public float gravity;
    public float jumpVelocity;
    public float groundSpeed;
    public float airSpeed;

    ControllableBox physics;

    void Start() {
        physics = GetComponent<ControllableBox>();
    }

    void FixedUpdate() {
        float sideMovement = Input.GetAxisRaw("Horizontal");
        bool attemptJump = Input.GetKeyDown(KeyCode.Space);

        physics.CalculateCollisions();

        physics.Accel(new Vector2(0, gravity));
        if (physics.IsOnGround()) {
            physics.Accel(groundSpeed*sideMovement, 0);
            if (attemptJump) {
                physics.Accel(new Vector2(0, jumpVelocity));
            }
        } else {
            physics.Accel(airSpeed*sideMovement, 0);
        }

        physics.runPhysics();
    }
}
