using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    private float terrainMaxHeight;
    private float avgHeight;
    public GameObject terrainGenerator;
    public GameObject water;

    public Transform player;
    public Camera cam;
    private Transform camT;
    
    
    public DiamondSquareV2 ds;
    private Waves wv;
    Transform wT;
    Transform tT;

    public Text text;
    private bool isLoading = true;
    private Vector3 playerStartingPos = new Vector3(10f, 120f, 10f);
    private Vector3 playerStartingRot = new Vector3(0f, 45f, 0f);
    private Vector3 cameraStartingRot = new Vector3(45f, 0f, 0f);
    void Start()
    {
        //Get the transform and DiamondSquareV2 of the terraingenerator and transform and WaterGenerator components of the water.
        camT = cam.GetComponent<Transform>();
        wT = water.GetComponent<Transform>();
        tT = terrainGenerator.GetComponent<Transform>();
        ds = terrainGenerator.GetComponent<DiamondSquareV2>();
        wv = water.GetComponent<Waves>();
        //Generate the terrain mesh
        ds.GenerateMesh();
        avgHeight = ds.GetAvgHeight();
        tT.position = new Vector3(0f, -avgHeight, 0f);
        //Set the water height to the average terrain height
        wT.position = new Vector3(0f, 0f, 0f);
        //Generate the water mesh
        wv.GenerateMesh();
        //Set player starting position and rotation
        player.position = new Vector3(playerStartingPos.x, playerStartingPos.y, playerStartingPos.z);
        player.localRotation = Quaternion.Euler(playerStartingRot);
        camT.localRotation = Quaternion.Euler(playerStartingPos);
        isLoading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isLoading == false)
        {
                isLoading = true;
                //Generate terrain mesh
                ds.GenerateMesh();
                avgHeight = ds.GetAvgHeight();
                //Set player starting position and rotation
                player.position = new Vector3(playerStartingPos.x, playerStartingPos.y, playerStartingPos.z);
                player.localRotation = Quaternion.Euler(playerStartingRot);
                camT.localRotation = Quaternion.Euler(playerStartingPos);
                tT.position = new Vector3(0f, -avgHeight, 0f);
                //Set the water height to the average terrain height
                wT.position = new Vector3(0f, 0f, 0f);
                //Generate water mesh
                wv.GenerateMesh();
                isLoading = false;
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            text.enabled = !text.enabled;
        }
    }
}