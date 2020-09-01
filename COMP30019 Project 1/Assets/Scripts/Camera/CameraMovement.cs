using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraMovement : MonoBehaviour
{
    public float speed = 10.0f;
	public float sprintMultiplier = 5.0f;
	public Transform player;
	public new Camera camera;
	public LayerMask layerMask;
	public bool test;
	public float collisionMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 velocity = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			velocity += camera.transform.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			velocity -= camera.transform.forward;
		}
		if (Input.GetKey(KeyCode.A))
		{
			velocity -= transform.right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			velocity += transform.right;
		}
		
		velocity.Normalize();
		if (Input.GetKey(KeyCode.LeftShift))
		{
			velocity *= sprintMultiplier;
		}
		Vector3 dPosition = velocity * Time.deltaTime * speed;
		
		float i = 0.001f;

		//	Check if the player will collide with the terrain by 'spawning' a sphere at the next position
		while(Physics.CheckSphere((player.position + dPosition), 0.5f))
		{
			if (dPosition.normalized == new Vector3 (0, -1, 0))
			{
				dPosition *= 0;
				break;
			}
			dPosition.y += i;
			Mathf.Max(0.00001f, i/2f);
		}
		player.position += dPosition;
    }
}
