using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class ControllableBox : MonoBehaviour {

    // Set in Unity
    public LayerMask collisionMask;
    public int raycastsPerDirection; // >=3
    public float raycastLookahead;
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

    void ResetCollisions() {
        lastHitAbove = null;
        lastHitBelow = null;
        lastHitLeft = null;
        lastHitRight = null;
    }

    public void CalculateCollisions() {
        ResetCollisions();

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
        return lastHitBelow != null;
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

        if ( shortestHitDirection > 0 ) {
            lastHitAbove = shortestHit;
        } else if (shortestHitDirection < 0) {
            lastHitBelow = shortestHit;
        }
    }

    void CalculateHorizontalCollisions() {
        if (velocity.x == 0) return;

        float sep = (collider.bounds.size.y - 2*skinWidth) / (float) (raycastsPerDirection - 1);
        Vector2 origin = new Vector2(collider.bounds.center.x, collider.bounds.min.y + skinWidth);

        float distance = collider.bounds.size.x / 2 + raycastLookahead;
        RaycastHit2D? shortestHit = null;

        for (int i=0;i<raycastsPerDirection;i++) {
            RaycastHit2D hit;

            hit = Physics2D.Raycast(origin, Mathf.Sign(velocity.x)* Vector2.right, distance, collisionMask);

            if (hit && hit.distance < distance) {
                shortestHit = hit;
                distance = hit.distance;
            }

            origin.y += sep;
        }

        if (velocity.x > 0) {
            lastHitRight = shortestHit;
        } else {
            lastHitLeft = shortestHit;
        }

        origin.y += sep;
    }

    public void runPhysics() {
        Debug.Log( (lastHitAbove==null?"":"Above,") + (lastHitBelow==null?"":"Below,") + (lastHitLeft==null?"":"Left,") + (lastHitRight==null?"":"Right") );

        if (lastHitAbove != null) {
            acceleration.y = -velocity.y;
            movement.y = lastHitAbove.Value.distance - collider.bounds.size.y / 2;
        } else if (lastHitBelow != null) {
            if (acceleration.y + velocity.y < 0)
                acceleration.y = -velocity.y;
            movement.y = collider.bounds.size.y / 2 - lastHitBelow.Value.distance;
        }

        if (lastHitRight != null) {
            if (acceleration.x + velocity.x > 0 || true)
                acceleration.x = -velocity.x;
            movement.x = lastHitRight.Value.distance - collider.bounds.size.x / 2;
        } else if (lastHitLeft != null) {
            if (acceleration.x + velocity.x < 0 || true)
                acceleration.x = -velocity.x;
            movement.x = collider.bounds.size.x / 2 - lastHitLeft.Value.distance + skinWidth;
        }

        velocity += acceleration;
        transform.Translate(velocity + movement);

        acceleration.Set(0, 0);
        movement.Set(0,0);
    }
}
