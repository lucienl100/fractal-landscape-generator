using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    private float randHeightDiff = 1.5f;
    private float terrainMaxHeight;
    public GameObject terrainGenerator;
    public Transform player;
    public Camera cam;
    private Transform camT;
    public GameObject water;
    private IEnumerator coroutine;
    public DiamondSquareV2 ds;
    private Waves wv;
    Transform wT;
    Transform tT;
    private bool isLoading = true;
    private Vector3 playerStartingPos = new Vector3(10f, 120f, 10f);
    private Vector3 playerStartingRot = new Vector3(0f, 45f, 0f);
    private Vector3 cameraStartingRot = new Vector3(45f, 0f, 0f);
    void Start()
    {
        camT = cam.GetComponent<Transform>();
        wT = water.GetComponent<Transform>();
        tT = terrainGenerator.GetComponent<Transform>();
        ds = terrainGenerator.GetComponent<DiamondSquareV2>();
        wv = water.GetComponent<Waves>();

        ds.GenerateMesh();

        terrainMaxHeight = ds.baseMaxHeight;

        tT.position = new Vector3(0f, 0f, 0f);
        wT.position = new Vector3(0f, ds.GetAvgHeight(), 0f);

        wv.GenerateMesh();
        player.position = new Vector3(playerStartingPos.x, playerStartingPos.y + ds.GetAvgHeight(), playerStartingPos.z);
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
                
                ds.GenerateMesh();
                player.position = new Vector3(playerStartingPos.x, playerStartingPos.y + ds.GetAvgHeight(), playerStartingPos.z);
                player.localRotation = Quaternion.Euler(playerStartingRot);
                camT.localRotation = Quaternion.Euler(playerStartingPos);
                wT.position = new Vector3(0f, ds.GetAvgHeight(), 0f);
                
                wv.GenerateMesh();
                isLoading = false;
        }
    }
    private float rand(float range)
    {
        return Random.Range(-range, range);
    }
}
