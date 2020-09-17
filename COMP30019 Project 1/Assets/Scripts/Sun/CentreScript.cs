using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentreScript : MonoBehaviour
{
    public float spinSpeed = 40f;
    private float halfgrid;
    public GameObject terrainGenerator;
    public Transform sun;
    // Start is called before the first frame update
    void Start()
    {
        halfgrid = terrainGenerator.GetComponent<DiamondSquareV2>().GetGridSize()/2;
        this.transform.localPosition = new Vector3(halfgrid, 0.0f, halfgrid); 
    }

    // Update is called once per frame
    void Update()
    {
        if (sun.position.y < -40f)
        {
            spinSpeed = 30f;
        }
        else
        {
            spinSpeed = 20f;
        }
        this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * spinSpeed, new Vector3(1.0f, 0.0f, 0.0f));
    }
}
