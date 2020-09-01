using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
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
    private Vector3 playerStartingPos = new Vector3(10f, 40f, 10f);
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

            tT.position = new Vector3(0f, 0f, 0f);
            wT.position = new Vector3(0f, ds.GetAvgHeight(), 0f);
            player.position = playerStartingPos;
            isLoading = false;
            firstFrame = false;
        }
        if(Input.GetKeyDown(KeyCode.Space) && isLoading == false)
        {
                isLoading = true;
                player.position = playerStartingPos;
                ds.GenerateMesh();
                Debug.Log("SceneManager : " + ds.GetAvgHeight());
                wT.position = new Vector3(0f, ds.GetAvgHeight(), 0f);
                wv.GenMesh();
                isLoading = false;
        }
    }
    private IEnumerator WaitForInit()
    {
        yield return new WaitForSeconds(0.01f);
    }
}
