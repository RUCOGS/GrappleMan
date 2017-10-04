using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Goal : MonoBehaviour {

    public LayerMask collisionMask;
    public int raycastsPerDirection;
	public float raycastLookahead;
	public float collisionDistance;
	public float skinWidth;

	new BoxCollider2D collider;

	// Collision Information
	RaycastHit2D? lastHitAbove = null;
	RaycastHit2D? lastHitBelow = null;
	RaycastHit2D? lastHitLeft = null;
	RaycastHit2D? lastHitRight = null;

	// Use this for initialization
	void Start () {
		collider = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        CalculateCollisions();

        if (Hit(ref lastHitAbove) || Hit(ref lastHitBelow) || Hit(ref lastHitLeft) || Hit(ref lastHitRight)) {
            Destroy(this.gameObject);
        }
	}

	public bool Hit(ref RaycastHit2D? raycast)
	{
		return raycast != null && raycast.Value.distance < collider.bounds.size.y / 2 + collisionDistance;
	}

	public void CalculateCollisions()
	{
		CalculateVerticalCollisions();
		CalculateHorizontalCollisions();
	}

	void CalculateVerticalCollisions()
	{
		float sep = (collider.bounds.size.x - 2 * skinWidth) / (float)(raycastsPerDirection - 1);
		float centerY = (collider.bounds.min.y + collider.bounds.max.y) / 2;
		Vector2 origin = new Vector2(collider.bounds.min.x + skinWidth, collider.bounds.center.y);

		float distance = collider.bounds.size.y / 2 + raycastLookahead;

		RaycastHit2D? shortestHit = null;
		int shortestHitDirection = 0;

		for (int i = 0; i < raycastsPerDirection; i++)
		{
			RaycastHit2D hit;

			hit = Physics2D.Raycast(origin, Vector2.down, distance, collisionMask);
			if (hit && hit.distance < distance)
			{
				shortestHit = hit;
				shortestHitDirection = -1;
				distance = hit.distance;
			}

			hit = Physics2D.Raycast(origin, Vector2.up, distance, collisionMask);
			if (hit && hit.distance < distance)
			{
				shortestHit = hit;
				shortestHitDirection = 1;
				distance = hit.distance;
			}

			origin.x += sep;
		}

		if (shortestHitDirection > 0)
		{
			lastHitAbove = shortestHit;
			lastHitBelow = null;
		}
		else if (shortestHitDirection < 0)
		{
			lastHitAbove = null;
			lastHitBelow = shortestHit;
		}
		else
		{
			lastHitAbove = null;
			lastHitBelow = null;
		}
	}

	void CalculateHorizontalCollisions()
	{
		float sep = (collider.bounds.size.y - 2 * skinWidth) / (float)(raycastsPerDirection - 1);
		Vector2 origin = new Vector2(collider.bounds.center.x, collider.bounds.min.y + skinWidth);

		float distance = collider.bounds.size.x / 2 + raycastLookahead;

		RaycastHit2D? shortestHit = null;
		int shortestHitDirection = 0;

		for (int i = 0; i < raycastsPerDirection; i++)
		{
			RaycastHit2D hit;

			hit = Physics2D.Raycast(origin, Vector2.right, distance, collisionMask);
			if (hit && hit.distance < distance)
			{
				shortestHit = hit;
				shortestHitDirection = 1;
				distance = hit.distance;
			}

			hit = Physics2D.Raycast(origin, Vector2.left, distance, collisionMask);
			if (hit && hit.distance < distance)
			{
				shortestHit = hit;
				shortestHitDirection = -1;
				distance = hit.distance;
			}

			origin.y += sep;
		}

		if (shortestHitDirection > 0)
		{
			lastHitRight = shortestHit;
			lastHitLeft = null;
		}
		else if (shortestHitDirection < 0)
		{
			lastHitRight = null;
			lastHitLeft = shortestHit;
		}
		else
		{
			lastHitRight = null;
			lastHitLeft = null;
		}
	}

}
