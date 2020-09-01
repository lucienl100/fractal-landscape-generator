using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{

    private float halfgrid;
    public GameObject terrainGenerator;

    // Start is called before the first frame update
    void Start()
    {
        halfgrid = terrainGenerator.GetComponent<DiamondSquareV2>().GetGridSize()/2;
        this.transform.localPosition = new Vector3(0.0f, halfgrid, 0.0f); 
    }
    
}
