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
    public float wallJumpVelocity;
    public float wallJumpAngleFromVertical;
    public float ropeDistance;
    public float ropeSpeed;
    public LayerMask collisionMask;

    ControllableBox physics;

    bool doubleJump = false;
    bool pressJumpLast = false;
    bool grapple = false;

    Vector2 startPosition;
    Vector2 ropeDirection;

    void Start() {
        physics = GetComponent<ControllableBox>();
        startPosition = new Vector2(transform.position.x, transform.position.y);
    }

    void Update() {
        if(!grapple) {
            if (Input.GetMouseButtonDown(0)) {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 playerPosition = transform.position;
                RaycastHit2D raycastHit = Physics2D.Raycast(playerPosition, mousePosition - playerPosition, ropeDistance, collisionMask);
                if (raycastHit) {
                    ropeDirection = raycastHit.point - playerPosition;
                    grapple = true;
                }
            }
        }
    }

    void FixedUpdate() {
        float sideMovement = Input.GetAxisRaw("Horizontal");
        bool attemptJump = Input.GetKey(KeyCode.Space);

        Vector2 vel = physics.GetVelocity();

        physics.CalculateCollisions();

        Obstacle[] collisions = physics.GetCollisionObjects();
        for (int i=0;i<collisions.Length;i++) {
            if (collisions[i].dieOnCollision) {
                physics.Accel(-physics.GetVelocity());
                physics.Move(startPosition - new Vector2(transform.position.x, transform.position.y));
                physics.runPhysics();
                return;
            } else if (collisions[i].winOnCollision) {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Win");
            }
        }

        physics.Accel(new Vector2(0, gravity));
        if (physics.IsOnGround()) {
            physics.Accel(groundSpeed * sideMovement, 0);
            doubleJump = true;
            if (attemptJump) {
                physics.Accel(new Vector2(0, jumpVelocity));
            }
            physics.Accel(new Vector2(vel.x*groundFriction,0));
        } else if (physics.IsOnLeftWall() && attemptJump && !pressJumpLast) {
                physics.Accel(new Vector2(wallJumpVelocity * Mathf.Sin(Mathf.PI * wallJumpAngleFromVertical / 180), wallJumpVelocity * Mathf.Cos(Mathf.PI * wallJumpAngleFromVertical / 180)));
		} else if (physics.IsOnRightWall() && attemptJump && !pressJumpLast) {
                physics.Accel(new Vector2(-1 * wallJumpVelocity * Mathf.Sin(Mathf.PI * wallJumpAngleFromVertical / 180), wallJumpVelocity * Mathf.Cos(Mathf.PI * wallJumpAngleFromVertical / 180)));
		} else {
            physics.Accel(airSpeed * sideMovement, 0);
            if (doubleJump && attemptJump && !pressJumpLast) {
                physics.Accel(new Vector2(0, doubleJumpVelocity));
                doubleJump = false;
            }
        }

        if (grapple) {
            grapple = false;
            ropeDirection.Normalize();
            physics.Accel(ropeDirection * ropeSpeed);
        }

        pressJumpLast = attemptJump;

        physics.runPhysics();
    }
}
