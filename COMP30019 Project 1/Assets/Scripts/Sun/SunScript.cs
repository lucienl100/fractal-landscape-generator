using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{

    private float height = 256;
    public GameObject terrainGenerator;

    // Start is called before the first frame update
    void Update()
    {
        this.transform.localPosition = new Vector3(0.0f, height, 0.0f); 
    }
    
}
