using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{

    public Player player;

    //Offset in values between camera and player. 
    private Vector3 offset;

	// Use this for initialization
	void Start()
	{
        //Assume Camera and Player start in the desired position alignment.
        //The current offset between Camera and Player will be maintained.
        offset = transform.position - player.transform.position;
	}

	// Update is called once per frame
	void Update()
	{
        //Every frame, move camera's position to player's position (Also accounting for offset).
        transform.position = player.transform.position + offset;
	}
}
