using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0, 257)]public int resolution = 50;
    public GameObject terrainG;
    private DiamondSquareV2 ds;
    public float mapScalar;
    private float waterLevel = -1f;
    public Material waterTexture;
    public Texture2D texture;
    public Texture2D normalMap;
    Mesh water;
    public PointLight pointLight;
    private MeshRenderer meshrenderer;
    void Awake()
    {
        ds = terrainG.GetComponent<DiamondSquareV2>();
        mapScalar = ds.GetGridSize();
        water = new Mesh();
        water.name = "waterMesh";
        GenerateMesh();
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = water;
        meshrenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshrenderer.material = new Material(Shader.Find("Unlit/WaterShader"));
        // renderer.material.SetTexture("_BumpMap", normalMap);
    }
    void Update()
    {
        // Pass updated light positions to shader
        meshrenderer.material.SetColor("_PointLightColor", this.pointLight.color);
        meshrenderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
    }
    public void GenerateMesh()
    {
        water.vertices = GenVertices();
        water.triangles = GenTriangles(water);
        water.uv = GenUVs();
    }
    public void SetHeight(float height)
    {
        waterLevel = height;
    }
    private Vector3[] GenVertices()
    {
        var vertices = new Vector3[(resolution) * (resolution)];
        float scale = mapScalar / ((float)resolution - 1f);
        int idx = 0;
        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                vertices[idx] = new Vector3((float)(x * scale), waterLevel, (float)(z * scale));
                idx++;
            }
        }
        return vertices;
    }
    private Vector2[] GenUVs()
    {
        var uv = new Vector2[(resolution) * resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                uv[x * resolution + z] = new Vector2((float)(x % 2), (float)(z % 2));        
            }
        }
        return uv;
    }
    private int[] GenTriangles(Mesh mesh)
    {
        var triangles = new int[mesh.vertices.Length * 6];
        int idx = 0;
        for (int x = 0; x < resolution - 1; x++)
        {
            for (int z = 0; z < resolution - 1; z++)
            {
                //Triangle 1
                //
                //|\
                //| \
                //|__\
                triangles[idx] = (x * (resolution)) + z + 1;
                triangles[idx + 1] = (x + 1)*(resolution) + z;
                triangles[idx + 2] = (x * (resolution)) + z;
                //Triangle 2
                // __
                //\  |
                // \ |
                //  \|
                triangles[idx + 3] = (x * (resolution)) + z + 1;
                triangles[idx + 4] = ((x + 1) * (resolution)) + z + 1;
                triangles[idx + 5] = (x + 1)*(resolution) + z;
                idx += 6;
            }
        }
        return triangles;
    }
}