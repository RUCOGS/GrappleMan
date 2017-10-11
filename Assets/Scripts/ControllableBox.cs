using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent (typeof (BoxCollider2D))]
public class ControllableBox : MonoBehaviour {

    // Set in Unity
    public LayerMask collisionMask;
    public int raycastsPerDirection; // >=3
    public float raycastLookahead;
    public float collisionDistance;
    public float skinWidth;

    // Components
    new BoxCollider2D collider;

    // Persistent
    Vector2 velocity;

    // Reset Each Frame
    Vector2 acceleration;
    Vector2 movement;

    // Collision Information
    RaycastHit2D? lastHitAbove = null;
    RaycastHit2D? lastHitBelow = null;
    RaycastHit2D? lastHitLeft = null;
    RaycastHit2D? lastHitRight = null;

    void Start() {
        collider = GetComponent<BoxCollider2D>();
        velocity = new Vector2(0, 0);
    }

    public void CalculateCollisions() {
        CalculateVerticalCollisions();
        CalculateHorizontalCollisions();
    }

    public void Accel(Vector2 acc) {
        acceleration += acc;
    }

    public void Accel(float x, float y) {
        acceleration.x += x;
        acceleration.y += y;
    }

    public void Move(Vector2 mov) {
        movement += mov;
    }

    public void Move(float x, float y) {
        movement.x += x;
        movement.y += y;
    }

    public bool IsOnGround() {
        return lastHitBelow != null && lastHitBelow.Value.distance < collider.bounds.size.y/2 + collisionDistance;
    }

    public bool IsOnCeiling() {
        return lastHitAbove != null && lastHitAbove.Value.distance < collider.bounds.size.y/2 + collisionDistance;
    }

    public bool IsOnLeftWall()
	{
		return lastHitLeft != null && lastHitLeft.Value.distance < collider.bounds.size.x / 2 + collisionDistance;
	}

    public bool IsOnRightWall()
    {
        return lastHitRight != null && lastHitRight.Value.distance < collider.bounds.size.x / 2 + collisionDistance;
    }

    public Obstacle[] GetCollisionObjects() {
        int collisions = 0;
        Obstacle[] obs = new Obstacle[2];
        if (IsOnGround()) {
            obs[collisions++] = lastHitBelow.Value.collider.GetComponent<Obstacle>();
        } else if (IsOnCeiling()) {
            obs[collisions++] = lastHitAbove.Value.collider.GetComponent<Obstacle>();
        }
        if (IsOnLeftWall()) {
            obs[collisions++] = lastHitLeft.Value.collider.GetComponent<Obstacle>();
        } else if (IsOnRightWall()) {
            obs[collisions++] = lastHitRight.Value.collider.GetComponent<Obstacle>();
        }

        Obstacle[] ret = new Obstacle[collisions];
        for (int i = 0; i < collisions; i++) ret[i] = obs[i];
        return ret;
    }

    public Vector2 GetVelocity() {
        return velocity;
    }

    void CalculateVerticalCollisions() {
        float sep = (collider.bounds.size.x - 2*skinWidth) / (float)(raycastsPerDirection - 1);
        float centerY = (collider.bounds.min.y + collider.bounds.max.y) / 2;
        Vector2 origin = new Vector2(collider.bounds.min.x + skinWidth, collider.bounds.center.y);

        float distance = collider.bounds.size.y / 2 + raycastLookahead;

        RaycastHit2D? shortestHit = null;
        int shortestHitDirection = 0;

        for (int i=0;i<raycastsPerDirection;i++) {
            RaycastHit2D hit;

            hit = Physics2D.Raycast(origin, Vector2.down, distance, collisionMask);
            if (hit && hit.distance < distance) {
                shortestHit = hit;
                shortestHitDirection = -1;
                distance = hit.distance;
            }

            hit = Physics2D.Raycast(origin, Vector2.up, distance, collisionMask);
            if (hit && hit.distance < distance) {
                shortestHit = hit;
                shortestHitDirection = 1;
                distance = hit.distance;
            }

            origin.x += sep;
        }

        if (shortestHitDirection > 0) {
            lastHitAbove = shortestHit;
            lastHitBelow = null;
        } else if (shortestHitDirection < 0) {
            lastHitAbove = null;
            lastHitBelow = shortestHit;
        } else {
            lastHitAbove = null;
            lastHitBelow = null;
        }
    }

    void CalculateHorizontalCollisions() {
        float sep = (collider.bounds.size.y - 2*skinWidth) / (float) (raycastsPerDirection - 1);
        Vector2 origin = new Vector2(collider.bounds.center.x, collider.bounds.min.y + skinWidth);

        float distance = collider.bounds.size.x / 2 + raycastLookahead;

        RaycastHit2D? shortestHit = null;
        int shortestHitDirection = 0;

        for (int i=0;i<raycastsPerDirection;i++) {
            RaycastHit2D hit;

            hit = Physics2D.Raycast(origin, Vector2.right, distance, collisionMask);
            if (hit && hit.distance < distance) {
                shortestHit = hit;
                shortestHitDirection = 1;
                distance = hit.distance;
            }

            hit = Physics2D.Raycast(origin, Vector2.left, distance, collisionMask);
            if (hit && hit.distance < distance) {
                shortestHit = hit;
                shortestHitDirection = -1;
                distance = hit.distance;
            }

            origin.y += sep;
        }

        if (shortestHitDirection > 0) {
            lastHitRight = shortestHit;
            lastHitLeft = null;
        } else if (shortestHitDirection < 0) {
            lastHitRight = null;
            lastHitLeft = shortestHit;
        } else {
            lastHitRight = null;
            lastHitLeft = null;
        }
    }

    public void runPhysics() {
        Vector2 nextTransform = velocity + acceleration + movement;

        if (lastHitRight != null && nextTransform.x > lastHitRight.Value.distance - collider.bounds.size.x / 2 - collisionDistance) {
            float distanceFromEdge = lastHitRight.Value.distance - collider.bounds.size.x / 2;

            if (acceleration.x + velocity.x > 0)
                acceleration.x = -velocity.x;

            movement.x = distanceFromEdge;
        } else if (lastHitLeft != null && nextTransform.x < collider.bounds.size.x / 2 - lastHitLeft.Value.distance + collisionDistance) {
            if (acceleration.x + velocity.x < 0)
                acceleration.x = -velocity.x;
            movement.x = collider.bounds.size.x / 2 - lastHitLeft.Value.distance;
        }

        if (lastHitAbove != null && nextTransform.y > lastHitAbove.Value.distance - collider.bounds.size.y / 2 - collisionDistance) {
            acceleration.y = -velocity.y;
            movement.y = lastHitAbove.Value.distance - collider.bounds.size.y / 2;
        } else if (lastHitBelow != null && nextTransform.y < collider.bounds.size.y / 2 - lastHitBelow.Value.distance + collisionDistance) {
            if (acceleration.y + velocity.y < 0)
                acceleration.y = -velocity.y;
            movement.y = collider.bounds.size.y / 2 - lastHitBelow.Value.distance;
        }

        velocity += acceleration;
        transform.Translate(velocity + movement);

        acceleration.Set(0, 0);
        movement.Set(0,0);
    }
}
