using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (ControllableBox))]
public class Player : MonoBehaviour {

    public float gravity;
    public float groundFriction;
    //public float ropeDistance;
    public float ropePullSpeed;
    //public float ropeSwingSpeed;
    public LayerMask ropeCollisionMask;

    ControllableBox physics;
    LineRenderer rope;
    
    bool ropePull = false;
    bool ropeGrapple = false;

    Vector2 startPosition;
    Vector2 ropeDirection;

    float ropeRadius = 0;
    float ropeAngle = 0;

    void Start() {
        physics = GetComponent<ControllableBox>();
        rope = GetComponentInChildren<LineRenderer>(true);
        startPosition = new Vector2(transform.position.x, transform.position.y);
    }

    void Update() {
        if(!ropePull) {
            if (Input.GetMouseButtonDown(0)) {
                float width = transform.GetComponent<Collider2D>().bounds.size.x;
                float height = transform.GetComponent<Collider2D>().bounds.size.y;
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 playerPosition = transform.position;
                playerPosition += new Vector2(width / 2, height / 2);
                RaycastHit2D raycastHit = Physics2D.Raycast(playerPosition, mousePosition - playerPosition, Mathf.Infinity, ropeCollisionMask);
                if (raycastHit) {
                    ropeDirection = raycastHit.point - playerPosition;
                    
                    rope.SetPosition(0, new Vector2(width / 2, height / 2));
                    rope.SetPosition(1, ropeDirection + new Vector2(width/2, height/2));
                    rope.enabled = true;
                    ropePull = true;
                }
            }
        }

        //if(!ropeGrapple) {
        //    if(Input.GetMouseButtonDown(1)) {
        //        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //        Vector2 playerPosition = transform.position;
        //        RaycastHit2D raycastHit = Physics2D.Raycast(playerPosition, mousePosition - playerPosition, ropeDistance, ropeCollisionMask);
        //        if (raycastHit) {
        //            ropeDirection = raycastHit.point - playerPosition;
        //            rope.SetPosition(0, Vector2.zero);
        //            rope.SetPosition(1, ropeDirection);
        //            rope.enabled = true;
        //            ropeGrapple = true;
        //        }
        //    }
        //}
    }

    void FixedUpdate() {
        float sideMovement = Input.GetAxisRaw("Horizontal");

        Vector2 vel = physics.GetVelocity();

        physics.CalculateCollisions();

        //Obstacle[] collisions = physics.GetCollisionObjects();
        //for (int i=0;i<collisions.Length;i++) {
        //    if (collisions[i].dieOnCollision) {
        //        physics.Accel(-physics.GetVelocity());
        //        physics.Move(startPosition - new Vector2(transform.position.x, transform.position.y));
        //        physics.runPhysics();
        //        return;
        //    } else if (collisions[i].winOnCollision) {
        //        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Win");
        //    }
        //}

        if(!ropeGrapple) {
            physics.Accel(0, gravity);
        }

        if(physics.IsOnGround()) {
            //physics.Accel(-vel.x, -vel.y);
            physics.Accel(vel.x * groundFriction, 0);
        }

        if (rope.enabled) {
            rope.SetPosition(0, Vector2.zero);
            float width = transform.GetComponent<Collider2D>().bounds.size.x;
            float height = transform.GetComponent<Collider2D>().bounds.size.y;
            rope.SetPosition(1, ropeDirection + new Vector2(width / 2, height / 2));
            if (vel.y <= 0 && !ropeGrapple) {
                rope.enabled = false;
            }
        }

        if (ropePull) {
            ropeDirection.Normalize();
            if(vel.y < 0) {
                physics.Accel(ropeDirection * (ropePullSpeed-gravity));
            } else {
                physics.Accel(ropeDirection * ropePullSpeed);
            }
            ropePull = false;
        }

        //if(ropeGrapple) {
        //    ropeDirection.Normalize();
        //    physics.Accel(-physics.GetVelocity());
        //    if(sideMovement != 0) {
        //        ropeRadius = (rope.GetPosition(1) - rope.GetPosition(0)).magnitude;
        //        ropeAngle += ropeSwingSpeed * sideMovement;
        //        Vector2 swingPlayer = new Vector2(Mathf.Sin(ropeAngle), Mathf.Cos(ropeAngle)) * ropeRadius;
        //        physics.Accel(swingPlayer);
        //    }
        //}

        Vector2 tv = physics.GetVelocity();
        if(tv.magnitude > 2) {
            physics.Accel(tv.normalized * 2f - tv);
        }

        physics.runPhysics();
    }
}
