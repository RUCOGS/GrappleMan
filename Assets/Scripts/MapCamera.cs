using UnityEngine;
using System.Collections;

public class MapCamera : MonoBehaviour
{

    public Tiled2Unity.TiledMap map;

    //Offset in values between camera and player. 
    private Vector3 offset;

    private ControllableBox playerPhysics;

	// Use this for initialization
	void Start()
	{
        //Assume Camera and Player start in the desired position alignment.
        //The current offset between Camera and Player will be maintained.
        //offset = transform.position - player.transform.position;
        //playerPhysics = player.GetComponent<ControllableBox>();

    }

	// Update is called once per frame
	void Update()
	{
        //Every frame, move camera's position to player's position (Also accounting for offset).
        //transform.position = player.transform.position + offset;
        //if(playerPhysics.GetVelocity().y < 0) {
        //    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        //    transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
        //} else {
        //    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        //    transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
        //}
	}
}
