using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (ControllableBox))]
public class Player : MonoBehaviour {

    public float gravity;
    public float jumpVelocity;
    public float doubleJumpVelocity;
    public float groundSpeed;
    public float airSpeed;
    public float groundFriction;

    ControllableBox physics;

    bool doubleJump = false;
    bool pressJumpLast = false;

    void Start() {
        physics = GetComponent<ControllableBox>();
    }

    void FixedUpdate() {
        float sideMovement = Input.GetAxisRaw("Horizontal");
        bool attemptJump = Input.GetKey(KeyCode.Space);

        Vector2 vel = physics.GetVelocity();

        physics.CalculateCollisions();

        physics.Accel(new Vector2(0, gravity));
        if (physics.IsOnGround()) {
            physics.Accel(groundSpeed * sideMovement, 0);
            if (attemptJump) {
                physics.Accel(new Vector2(0, jumpVelocity));
                doubleJump = true;
            }
            physics.Accel(new Vector2(vel.x*groundFriction,0));
        } else {
            physics.Accel(airSpeed * sideMovement, 0);
            if (doubleJump && attemptJump && !pressJumpLast) {
                physics.Accel(new Vector2(0, doubleJumpVelocity));
                doubleJump = false;
            }
        }

        pressJumpLast = attemptJump;

        physics.runPhysics();
    }
}
