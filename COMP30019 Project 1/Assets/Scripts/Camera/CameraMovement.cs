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
		Ray ray = new Ray(player.position, velocity);
		Debug.DrawRay(player.position, velocity, Color.red);
		RaycastHit rch = new RaycastHit();
		if(Physics.Raycast(ray, dPosition.magnitude * collisionMultiplier, layerMask))
		{
			dPosition *= 0;
		}
		player.position += dPosition;
    }
}
