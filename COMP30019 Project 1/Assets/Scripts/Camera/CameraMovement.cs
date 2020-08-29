﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraMovement : MonoBehaviour
{
    public float speed = 5.0f;
	public float sprintMultiplier = 1.5f;
	public Transform player;
	public GameObject terrainGenerator;
	public new Camera camera;
	public LayerMask layerMask;
	public bool test;
	public float collisionMultiplier;
	
	private float nextVertHeight;
	HeightGrid verts;
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
		List<Ray> rays = new List<Ray>();
		//rays.Add(new Ray(player.position, velocity));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x, velocity.y - 1, velocity.z)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x - 1, velocity.y - 1, velocity.z)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x - 1, velocity.y - 1, velocity.z - 1)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x + 1, velocity.y - 1, velocity.z - 1)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x + 1, velocity.y - 1, velocity.z + 1)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x - 1, velocity.y - 1, velocity.z + 1)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x, velocity.y - 1, velocity.z - 1)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x - 1, velocity.y - 1, velocity.z)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x, velocity.y - 1, velocity.z + 1)));
		//rays.Add(new Ray(player.position, new Vector3(velocity.x + 1, velocity.y - 1, velocity.z)));
		//Debug.DrawRay(player.position, velocity, Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x, velocity.y - 1, velocity.z), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x - 1, velocity.y - 1, velocity.z), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x - 1, velocity.y - 1, velocity.z - 1), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x + 1, velocity.y - 1, velocity.z - 1), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x + 1, velocity.y - 1, velocity.z + 1), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x - 1, velocity.y - 1, velocity.z + 1), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x, velocity.y - 1, velocity.z - 1), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x - 1, velocity.y - 1, velocity.z), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x, velocity.y - 1, velocity.z + 1), Color.red);
		//Debug.DrawRay(player.position, new Vector3(velocity.x + 1, velocity.y - 1, velocity.z), Color.red);
		//foreach(Ray ray in rays)
		//{
		//	if(Physics.Raycast(ray, dPosition.magnitude * collisionMultiplier, layerMask))
		//	{
		//	dPosition *= 0;
		//	}
		//}
		float i = 0.001f;
		while(Physics.CheckSphere((player.position + dPosition * 3f), 0.5f))
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
