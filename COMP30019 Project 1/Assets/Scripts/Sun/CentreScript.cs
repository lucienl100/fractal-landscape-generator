using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentreScript : MonoBehaviour
{
    public float spinSpeed;
    private float halfgrid;
    public GameObject terrainGenerator;
    public Transform sun;
    public bool pauseSun;
    public bool lessNight;
    private float actualSpinSpeed;
    // Start is called before the first frame update
    void Start()
    {
        halfgrid = terrainGenerator.GetComponent<DiamondSquareV2>().GetGridSize()/2;
        this.transform.localPosition = new Vector3(halfgrid, 0.0f, halfgrid); 
    }

    // Update is called once per frame
    void Update()
    {
        // If C pressed stop sun
        if (Input.GetKeyDown(KeyCode.C))
        {
            pauseSun = !pauseSun;
        }

        if (!pauseSun && lessNight) {
            // Speed up sun if too low
            if (sun.position.y > -80f)
            {
                actualSpinSpeed = spinSpeed;
            }
            else
            {
                actualSpinSpeed = spinSpeed*2;
            }
            this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * actualSpinSpeed, new Vector3(1.0f, 0.0f, 0.0f));
        } else if (!pauseSun) {
            actualSpinSpeed = spinSpeed;
            this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * actualSpinSpeed, new Vector3(1.0f, 0.0f, 0.0f));
        }
    }
}
