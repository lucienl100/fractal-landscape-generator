using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    private float randHeightDiff = 1.5f;
    private float terrainMaxHeight;
    private float terrainMaxHeightCheckDist = 5f;
    public GameObject terrainGenerator;
    public Transform player;
    public GameObject water;
    private IEnumerator coroutine;
    private DiamondSquareV2 ds;
    private Waves wv;
    Transform wT;
    Transform tT;
    private bool isLoading = true;
    private bool firstFrame = true;
    private Vector3 playerStartingPos = new Vector3(10f, 80f, 10f);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (firstFrame)
        {
            
            wT = water.GetComponent<Transform>();
            tT = terrainGenerator.GetComponent<Transform>();
            ds = terrainGenerator.GetComponent<DiamondSquareV2>();
            wv = water.GetComponent<Waves>();

            terrainMaxHeight = ds.baseMaxHeight;
            tT.position = new Vector3(0f, 0f, 0f);
            wT.position = new Vector3(0f, ds.GetAvgHeight() + rand(randHeightDiff), 0f);
            player.position = playerStartingPos;
            isLoading = false;
            firstFrame = false;
        }
        if(Input.GetKeyDown(KeyCode.Space) && isLoading == false)
        {
                isLoading = true;
                
                ds.GenerateMesh();
                player.position = new Vector3(playerStartingPos.x, 1.1f * (ds.GetAvgHeight()), playerStartingPos.z);
                wT.position = new Vector3(0f, ds.GetAvgHeight() + ds.randomTHeight + rand(randHeightDiff), 0f);
                wv.GenMesh();
                isLoading = false;
        }
    }
    private float rand(float range)
    {
        return Random.Range(-range, range);
    }
    private IEnumerator WaitForInit()
    {
        yield return new WaitForSeconds(0.01f);
    }
}
