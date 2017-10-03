using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Player : MonoBehaviour {

    public LayerMask collisionMask;

    public float gravity;
    public float friction;
    public float jumpVelocity;
    public float accelerationAir;
    public float accelerationGround;
    public float wallBounciness;
    public float floorBounciness;

    Vector2 velocity;
    new BoxCollider2D collider;

	void Start () {
        velocity = new Vector2(0, 0);
        collider = GetComponent<BoxCollider2D>();
	}
	
	void Update () {
        Vector2 center = collider.bounds.center;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector2 acceleration = new Vector2(0, gravity);
        Vector2 frameVel = new Vector2(0, 0);

        float height = collider.bounds.max.y - collider.bounds.min.y;
        float width = collider.bounds.max.x - collider.bounds.min.x;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            acceleration.y += jumpVelocity;
        }

        // Calculate Collisions
        RaycastHit2D hitBelow = Physics2D.Raycast(center, Vector2.down, height / 2, collisionMask);
        if (hitBelow)
        {
            frameVel.y += (height/2) - hitBelow.distance;
            acceleration.y = -velocity.y;

            if ( velocity.y < -0.1f)
            {
                acceleration.y -= velocity.y*floorBounciness;
            }

            if (friction > Mathf.Abs(velocity.x))
            {
                velocity.x = 0;
            } else
            {
                velocity.x -= Mathf.Sign(velocity.x) * friction;
            }

            

            acceleration.x += accelerationGround * input.x;
        } else
        {
            acceleration.x += accelerationAir * input.x;
        }

        if (velocity.x > 0)
        {
            RaycastHit2D hitRight = Physics2D.Raycast(center, Vector2.right, width / 2, collisionMask);
            if (hitRight)
            {
                frameVel.x += hitRight.distance - (width / 2);
                acceleration.x = -velocity.x * (1 + wallBounciness);
            }
        } else if (velocity.x < 0)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(center, Vector2.left, width / 2, collisionMask);
            if (hitLeft)
            {
                frameVel.x += (width / 2) - hitLeft.distance;
                acceleration.x = -velocity.x * (1 + wallBounciness);
            }
        }

        velocity += acceleration;
        transform.Translate(velocity + frameVel);
	}
}
